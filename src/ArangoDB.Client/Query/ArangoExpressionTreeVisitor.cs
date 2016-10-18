using ArangoDB.Client.Data;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Query
{
    public partial class ArangoExpressionTreeVisitor : RelinqExpressionVisitor, INamedExpressionVisitor
    {
        public ArangoModelVisitor ModelVisitor { get; set; }
        public QueryModel QueryModel { get; set; }

        public bool TreatNewWithoutBracket { get; set; }
        public bool HandleLet { get; set; }
        public bool HandleJoin { get; set; }

        public ArangoExpressionTreeVisitor(ArangoModelVisitor modelVisitor)
        {
            this.ModelVisitor = modelVisitor;
        }

        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            if (expression.Method.Name == "As")
            {
                Visit(expression.Arguments[0]);
                return expression;
            }

            if(expression.Method.Name == "get_Item" && expression.Method.DeclaringType.Name == "IList`1")
            {
                Visit(expression.Object);
                ModelVisitor.QueryText.Append(" [ ");
                Visit(expression.Arguments[0]);
                ModelVisitor.QueryText.Append(" ] ");
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
                throw new InvalidOperationException($"Method {expression.Method.Name} is not supported in ArangoLinqProvider");

            string argumentSeprator = null;
            bool noParenthesis = methodsWithNoParenthesis.TryGetValue(methodName, out argumentSeprator);

            if (!noParenthesis)
            {
                ModelVisitor.QueryText.AppendFormat(" {0}( ", methodName);
                argumentSeprator = " , ";
            }

            Type[] genericArguments = null;
            if (methodsWithFirstGenericArgument.Contains(methodName))
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
                Visit(expression.Arguments[i]);

                if (i != expression.Arguments.Count - 1)
                    ModelVisitor.QueryText.Append(argumentSeprator);
            }

            if (!noParenthesis)
                ModelVisitor.QueryText.Append(" ) ");

            return expression;
        }
        
        protected override Expression VisitParameter(ParameterExpression expression)
        {
            string name = expression.Name;

            if(expression.Type.Name == "TraversalData`2")
            {
                string prefix = LinqUtility.MemberNameFromMap(name, "graph", ModelVisitor);

                ModelVisitor.QueryText.AppendFormat(" {{ {0} : {1}, {2} : {3}, {4} : {5} }} ",
                LinqUtility.ResolvePropertyName($"vertex"),
                LinqUtility.ResolvePropertyName($"{prefix}_Vertex"),
                LinqUtility.ResolvePropertyName($"edge"),
                LinqUtility.ResolvePropertyName($"{prefix}_Edge"),
                LinqUtility.ResolvePropertyName($"path"),
                LinqUtility.ResolvePropertyName($"{prefix}_Path"));

                return expression;
            }

            if(expression.Type.Name == "ShortestPathData`2")
            {
                string prefix = LinqUtility.MemberNameFromMap(name, "graph", ModelVisitor);

                ModelVisitor.QueryText.AppendFormat(" {{ {0} : {1}, {2} : {3} }} ",
                LinqUtility.ResolvePropertyName($"vertex"),
                LinqUtility.ResolvePropertyName($"{prefix}_Vertex"),
                LinqUtility.ResolvePropertyName($"edge"),
                LinqUtility.ResolvePropertyName($"{prefix}_Edge"));

                return expression;
            }

            ModelVisitor.QueryText.AppendFormat(" {0} ", LinqUtility.ResolvePropertyName(name));

            return expression;
        }

        protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression expression)
        {
            ModelVisitor.QueryText.AppendFormat(" {0}", LinqUtility.ResolvePropertyName(expression.ReferencedQuerySource.ItemName));

            var mainFromClause = expression.ReferencedQuerySource as MainFromClause;
            //  .Select(g => g.Select(gList => gList.Age)) subquery select, handle prior groupby source parameter names
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

        protected override Expression VisitUnary(UnaryExpression expression)
        {
            if (expression.NodeType == ExpressionType.Not)
            {
                ModelVisitor.QueryText.Append(expressionTypes[expression.NodeType]);

                var operand = Visit(expression.Operand);

                if (operand != null)
                {
                    return Expression.Not(operand);
                }
            }
            if (expression.NodeType == ExpressionType.Convert)
            {
                return Visit(expression.Operand);
            }

            return base.VisitUnary(expression);
        }

        protected override Expression VisitBinary(BinaryExpression expression)
        {
            if (expression.NodeType == ExpressionType.ArrayIndex)
            {
                Visit(expression.Left);
                var arrayIndex = (expression.Right as ConstantExpression).Value;
                ModelVisitor.QueryText.AppendFormat("[{0}] ", arrayIndex);

                return expression;
            }
            else
            {
                ModelVisitor.QueryText.Append(" ( ");
                var leftExp = Visit(expression.Left);

                ModelVisitor.QueryText.Append(expressionTypes[expression.NodeType]);

                var rightExp = Visit(expression.Right);
                ModelVisitor.QueryText.Append(" ) ");

                return expression;
            }
        }

        protected override Expression VisitMember(MemberExpression expression)
        {
            var member = expression.Expression as MemberExpression;
            if ((member != null && member.Expression.Type.Name == "IGrouping`2"))
            {
                ModelVisitor.QueryText.AppendFormat(" {0} ", LinqUtility.ResolvePropertyName(expression.Member.Name));
            }
            // Select(g=>g.Key)
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
            else if (expression.Expression.Type.Name == "TraversalData`2"
                || expression.Expression.Type.Name == "ShortestPathData`2")
            {
                var parameterExpression = expression.Expression as ParameterExpression;
                if (parameterExpression == null)
                    throw new InvalidOperationException("[TraversalData`2|ShortestPathData`2] VisitMember, expected a ParameterExpression");

                string prefix = LinqUtility.MemberNameFromMap(parameterExpression.Name, "graph", ModelVisitor);

                ModelVisitor.QueryText.AppendFormat(LinqUtility.ResolvePropertyName($"{prefix}_{expression.Member.Name}"));
            }
            else
            {
                Visit(expression.Expression);
                ModelVisitor.QueryText.AppendFormat(".{0} ", LinqUtility.ResolveMemberName(ModelVisitor.Db, expression.Member));
            }

            return expression;
        }

        protected override Expression VisitConstant(ConstantExpression expression)
        {
            var parentModelVisitor = LinqUtility.FindParentModelVisitor(this.ModelVisitor);
            parentModelVisitor.ParameterNameCounter++;
            string parameterName = "P" + parentModelVisitor.ParameterNameCounter;

            parentModelVisitor.QueryData.BindVars.Add(new QueryParameter() { Name = parameterName, Value = expression.Value });

            ModelVisitor.QueryText.AppendFormat(" @{0} ", parameterName);

            return expression;
        }

        protected override Expression VisitNewArray(NewArrayExpression expression)
        {
            ModelVisitor.QueryText.Append(" [ ");
            int i = 0;
            foreach (var n in expression.Expressions)
            {
                Visit(n);
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

        protected override Expression VisitMemberInit(MemberInitExpression node)
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

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            var namedExpression = NamedExpression.WrapIntoNamedExpression(ModelVisitor.Db, node.Member, node.Expression);

            Expression e = Visit(namedExpression);
            if (e != node.Expression)
            {
                return Expression.Bind(node.Member, e);
            }
            return node;
        }

        protected override Expression VisitNew(NewExpression expression)
        {
            // Select(g=>g)
            if (expression.Type.Name == "Grouping`2")
            {
                var groupByClause = LinqUtility.PriorGroupBy(ModelVisitor)[0];
                ModelVisitor.QueryText.AppendFormat(" {0} ", LinqUtility.ResolvePropertyName(groupByClause.TranslateIntoName()));
                return expression;
            }

            if (!TreatNewWithoutBracket)
                ModelVisitor.QueryText.Append(" { ");
            var e = (NewExpression)NamedExpression.CreateNewExpressionWithNamedArguments(ModelVisitor.Db, expression);
            for (int i = 0; i < e.Arguments.Count; i++)
            {
                Visit(e.Arguments[i]);
                if (i != e.Arguments.Count - 1)
                    ModelVisitor.QueryText.Append(" , ");
            }
            if (!TreatNewWithoutBracket)
                ModelVisitor.QueryText.Append(" } ");

            return e;
        }

        protected override Expression VisitSubQuery(SubQueryExpression expression)
        {
            if (!HandleJoin && !HandleLet)
                ModelVisitor.QueryText.Append(" ( ");

            var visitor = new ArangoModelVisitor(ModelVisitor.Db);

            if (HandleLet)
                visitor.DefaultAssociatedIdentifier = QueryModel.MainFromClause.ItemName;

            visitor.QueryText = ModelVisitor.QueryText;
            visitor.ParnetModelVisitor = ModelVisitor;
            visitor.IgnoreFromClause = HandleLet;

            visitor.VisitQueryModel(expression.QueryModel);

            if (!HandleJoin && !HandleLet)
                ModelVisitor.QueryText.Append(" ) ");

            return expression;
        }

        public Expression VisitNamed(NamedExpression expression)
        {
            ModelVisitor.QueryText.AppendFormat(" `{0}` {1} ", expression.Name, TreatNewWithoutBracket ? "= " : ": ");
            return Visit(expression.Expression);
        }
    }
}
