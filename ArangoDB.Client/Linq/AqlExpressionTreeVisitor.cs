using ArangoDB.Client.Common.Remotion.Linq;
using ArangoDB.Client.Common.Remotion.Linq.Clauses;
using ArangoDB.Client.Common.Remotion.Linq.Clauses.Expressions;
using ArangoDB.Client.Common.Remotion.Linq.Clauses.ExpressionTreeVisitors;
using ArangoDB.Client.Common.Remotion.Linq.Parsing;
using ArangoDB.Client.Data;
using ArangoDB.Client.Linq.Clause;
using ArangoDB.Client.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Linq
{
    public partial class AqlExpressionTreeVisitor : ThrowingExpressionTreeVisitor, INamedExpressionVisitor
    {
        public AqlModelVisitor ModelVisitor { get; set; }
        public QueryModel QueryModel { get; set; }

        public bool TreatNewWithoutBracket { get; set; }
        public bool HandleLet { get; set; }
        public bool HandleJoin { get; set; }

        public AqlExpressionTreeVisitor(AqlModelVisitor modelVisitor)
        {
            this.ModelVisitor = modelVisitor;
        }

        protected override Expression VisitMethodCallExpression(MethodCallExpression expression)
        {
            if (expression.Method.Name == "As")
            {
                VisitExpression(expression.Arguments[0]);
                return expression;
            }
            
            string methodName;
            var userFunction = ModelVisitor.Db.SharedSetting.AqlFunctions.FindFunctionAttribute(expression.Method);
            bool methodExists = userFunction != null;
            if (methodExists)
                methodName = userFunction.Name;
            else
                methodExists = aqlMethods.TryGetValue(expression.Method.Name, out methodName);

            if (!methodExists)
                throw new InvalidOperationException($"Method {expression.Method.Name} is not supported in AqlLinqProvider");
            
            string argumentSeprator = null;
            bool noParenthesis = methodsWithNoParenthesis.TryGetValue(methodName,out argumentSeprator);

            if (!noParenthesis)
            {
                ModelVisitor.QueryText.AppendFormat(" {0}( ", methodName);
                argumentSeprator = " , ";
            }

            Type[] genericArguments = null;
            if(methodsWithFirstGenericArgument.Contains(methodName))
            {
                genericArguments = expression.Method.GetGenericArguments();
                var collection = LinqUtility.ResolveCollectionName(ModelVisitor.Db, genericArguments[0]);
                ModelVisitor.QueryText.AppendFormat(" {0}{1}", collection, argumentSeprator);
            }
            
            if (methodsWithSecondGenericArgument.Contains(methodName))
            {
                var collection = LinqUtility.ResolveCollectionName(ModelVisitor.Db, genericArguments[1]);
                ModelVisitor.QueryText.AppendFormat(" {0}{1}", collection, argumentSeprator);
            }
            
            for (int i = 0; i < expression.Arguments.Count; i++)
            {
                VisitExpression(expression.Arguments[i]);

                if (i != expression.Arguments.Count - 1)
                    ModelVisitor.QueryText.Append(argumentSeprator);
            }

            if (!noParenthesis)
                ModelVisitor.QueryText.Append(" ) ");

            return expression;
        }

        // Called when a LINQ expression type is not handled above.
        protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod)
        {
            string itemText = FormatUnhandledItem(unhandledItem);
            var message = string.Format("The expression '{0}' (type: {1}) is not supported by ArangoDB LINQ provider.", itemText, typeof(T));
            return new NotSupportedException(message);
        }

        protected override Expression VisitParameterExpression(ParameterExpression expression)
        {
            ModelVisitor.QueryText.AppendFormat(" {0} ", LinqUtility.ResolvePropertyName(expression.Name));

            return expression;
        }

        protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
        {
            ModelVisitor.QueryText.AppendFormat(" {0}", LinqUtility.ResolvePropertyName(expression.ReferencedQuerySource.ItemName));

            var mainFromClause = expression.ReferencedQuerySource as MainFromClause;
            if (mainFromClause != null && mainFromClause.FromExpression.Type.Name == "IGrouping`2")
            {
                var groupByClauses = LinqUtility.PriorGroupBy(ModelVisitor);

                for (int i = 0; i < groupByClauses.Count; i++)
                {
                    ModelVisitor.QueryText.AppendFormat("{0}{1}{2}"
                        , i == 0 ? "." : ""
                        , LinqUtility.ResolvePropertyName(groupByClauses[i].FromParameterName)
                        , i != groupByClauses.Count - 1 ? "." : "");
                }
            }

            return expression;
        }

        protected override Expression VisitUnaryExpression(UnaryExpression expression)
        {
            if (expression.NodeType == ExpressionType.Not)
            {
                ModelVisitor.QueryText.Append(expressionTypes[expression.NodeType]);

                var operand = VisitExpression(expression.Operand);

                if (operand != null)
                {
                    return Expression.Not(operand);
                }
            }
            if (expression.NodeType == ExpressionType.Convert)
            {
                return VisitExpression(expression.Operand);
            }

            return base.VisitUnaryExpression(expression);
        }

        protected override Expression VisitBinaryExpression(BinaryExpression expression)
        {
            if (expression.NodeType == ExpressionType.ArrayIndex)
            {
                VisitExpression(expression.Left);
                var arrayIndex = (expression.Right as ConstantExpression).Value;
                ModelVisitor.QueryText.AppendFormat("[{0}] ", arrayIndex);

                return expression;
            }
            else
            {
                ModelVisitor.QueryText.Append(" ( ");
                var leftExp = VisitExpression(expression.Left);

                ModelVisitor.QueryText.Append(expressionTypes[expression.NodeType]);

                var rightExp = VisitExpression(expression.Right);
                ModelVisitor.QueryText.Append(" ) ");

                return expression;
            }
        }

        protected override Expression VisitMemberExpression(MemberExpression expression)
        {
            var member = expression.Expression as MemberExpression;
            if ((member != null && member.Expression.Type.Name == "IGrouping`2"))
            {
                ModelVisitor.QueryText.AppendFormat(" {0} ", LinqUtility.ResolvePropertyName(expression.Member.Name));
            }
            else if (expression.Expression.Type.Name == "IGrouping`2")
            {
                var groupByClause = LinqUtility.PriorGroupBy(ModelVisitor)[0];

                var newExpression = groupByClause.Selector as NewExpression;
                if (newExpression != null)
                {
                    ModelVisitor.QueryText.Append(" { ");
                    for (int i = 0; i < newExpression.Members.Count; i++)
                    {
                        string memberName = newExpression.Members[i].Name;
                        ModelVisitor.QueryText.AppendFormat(" {0} : {1} ",
                            LinqUtility.ResolvePropertyName(memberName),
                            LinqUtility.ResolvePropertyName(memberName));

                        if (i != newExpression.Members.Count - 1)
                            ModelVisitor.QueryText.Append(" , ");
                    }
                    ModelVisitor.QueryText.Append(" } ");
                }

                if (groupByClause.Selector.NodeType != ExpressionType.New)
                    ModelVisitor.QueryText.AppendFormat(" {0} ", LinqUtility.ResolvePropertyName(groupByClause.CollectVariableName));
            }
            else
            {
                VisitExpression(expression.Expression);
                ModelVisitor.QueryText.AppendFormat(".{0} ", LinqUtility.ResolveMemberName(ModelVisitor.Db, expression.Member));
            }

            return expression;
        }

        protected override Expression VisitConstantExpression(ConstantExpression expression)
        {
            var parentModelVisitor = LinqUtility.FindParentModelVisitor(this.ModelVisitor);
            parentModelVisitor.ParameterNameCounter++;
            string parameterName = "P" + parentModelVisitor.ParameterNameCounter;
            
            parentModelVisitor.QueryData.BindVars.Add(new QueryParameter() { Name = parameterName, Value = expression.Value });

            ModelVisitor.QueryText.AppendFormat(" @{0} ", parameterName);

            return expression;
        }

        protected override Expression VisitNewArrayExpression(NewArrayExpression expression)
        {
            ModelVisitor.QueryText.Append(" [ ");
            int i = 0;
            foreach (var n in expression.Expressions)
            {
                VisitExpression(n);
                if (i != expression.Expressions.Count - 1)
                    ModelVisitor.QueryText.Append(" , ");
                i++;
            }
            ModelVisitor.QueryText.Append(" ] ");

            return expression;
        }

        void VisitConstantValue(object value, Type type)
        {
            if (value == null)
            {
                ModelVisitor.QueryText.Append(" null ");
                return;
            }

            if (type == typeof(string) || type == typeof(char))
            {
                ModelVisitor.QueryText.AppendFormat(" '{0}' ", value);
                return;
            }

            if (type == typeof(bool))
            {
                ModelVisitor.QueryText.AppendFormat(" {0} ", ((bool)value) == true ? "true" : "false");
                return;
            }

            if (type == typeof(int) || type == typeof(short) || type == typeof(long) || type == typeof(decimal)
                 || type == typeof(decimal) || type == typeof(double))
            {
                ModelVisitor.QueryText.AppendFormat(" {0} ", value);
                return;
            }

            var dic = value as IDictionary;
            if (dic != null)
            {
                ModelVisitor.QueryText.Append(" { ");

                object[] dicKeys = new object[dic.Keys.Count];
                dic.Keys.CopyTo(dicKeys, 0);

                object[] dicValues = new object[dic.Values.Count];
                dic.Values.CopyTo(dicValues, 0);

                for (int i = 0; i < dicKeys.Length; i++)
                {
                    ModelVisitor.QueryText.AppendFormat(" '{0}' :  ", dicKeys[i].ToString());
                    var v = dicValues[i];
                    VisitConstantValue(v, v != null ? v.GetType() : null);

                    if (i != dicKeys.Length - 1)
                        ModelVisitor.QueryText.Append(" , ");
                }

                ModelVisitor.QueryText.Append(" } ");
                return;
            }

            var enumerable = value as IEnumerable;
            if (enumerable != null)
            {
                ModelVisitor.QueryText.Append(" [ ");

                foreach (var v in enumerable)
                {
                    VisitConstantValue(v, v != null ? v.GetType() : null);
                    ModelVisitor.QueryText.Append(" , ");
                }

                ModelVisitor.QueryText.Remove(ModelVisitor.QueryText.Length - 2, 2);
                ModelVisitor.QueryText.Append(" ] ");
                return;
            }

            var nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType != null)
            {
                VisitConstantValue(value, nullableType);
                return;
            }

            throw new NotSupportedException(string.Format("Constant value of type {0} cant be translate to aql", type.ToString()));
        }

        protected override Expression VisitMemberInitExpression(MemberInitExpression node)
        {
            NewExpression n = Expression.New(node.NewExpression.Constructor, node.NewExpression.Arguments);

            if (!TreatNewWithoutBracket)
                ModelVisitor.QueryText.Append(" { ");

            int bindingIndex = -1;
            foreach (var b in node.Bindings)
            {
                bindingIndex++;

                VisitMemberBinding(b);

                if (bindingIndex != node.Bindings.Count - 1)
                    ModelVisitor.QueryText.Append(" , ");
            }

            if (!TreatNewWithoutBracket)
                ModelVisitor.QueryText.Append(" } ");

            return node;
        }

        protected override MemberBinding VisitMemberAssignment(MemberAssignment node)
        {
            var namedExpression = NamedExpression.WrapIntoNamedExpression(node.Member.Name, node.Expression);
            Expression e = VisitExpression(namedExpression);
            if (e != node.Expression)
            {
                return Expression.Bind(node.Member, e);
            }
            return node;
        }

        protected override Expression VisitNewExpression(NewExpression expression)
        {
            if (expression.Type.Name == "Grouping`2")
            {
                var groupByClause = LinqUtility.PriorGroupBy(ModelVisitor)[0];
                ModelVisitor.QueryText.AppendFormat(" {0} ", LinqUtility.ResolvePropertyName(groupByClause.TranslateIntoName()));
                return expression;
            }

            if (!TreatNewWithoutBracket)
                ModelVisitor.QueryText.Append(" { ");
            var e = (NewExpression)NamedExpression.CreateNewExpressionWithNamedArguments(expression);
            for (int i = 0; i < e.Arguments.Count; i++)
            {
                VisitExpression(e.Arguments[i]);
                if (i != e.Arguments.Count - 1)
                    ModelVisitor.QueryText.Append(" , ");
            }
            if (!TreatNewWithoutBracket)
                ModelVisitor.QueryText.Append(" } ");

            return e;
        }

        protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
        {
            if (!HandleJoin && !HandleLet)
                ModelVisitor.QueryText.Append(" ( ");

            var visitor = new AqlModelVisitor(ModelVisitor.Db);

            if (HandleLet)
                visitor.DefaultAssociatedIdentifier = QueryModel.MainFromClause.ItemName;

            visitor.QueryText = this.ModelVisitor.QueryText;
            visitor.ParnetModelVisitor = this.ModelVisitor;
            visitor.IgnoreFromClause = HandleLet;

            visitor.VisitQueryModel(expression.QueryModel);

            if (!HandleJoin && !HandleLet)
                ModelVisitor.QueryText.Append(" ) ");

            return expression;
        }

        public Expression VisitNamedExpression(NamedExpression expression)
        {
            ModelVisitor.QueryText.AppendFormat(" `{0}` {1} ", expression.Name, TreatNewWithoutBracket ? "= " : ": ");
            return VisitExpression(expression.Expression);
        }

        private string FormatUnhandledItem<T>(T unhandledItem)
        {
            var itemAsExpression = unhandledItem as Expression;
            return itemAsExpression != null ? FormattingExpressionTreeVisitor.Format(itemAsExpression) : unhandledItem.ToString();
        }
    }
}
