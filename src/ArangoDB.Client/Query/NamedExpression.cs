using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Query
{
    public class NamedExpression : Expression
    {
        public const string DefaultName = "value";

        public static NamedExpression CreateFromMemberName(string memberName, Expression innerExpression)
        {
            LinqUtility.CheckNotNull("memberName", memberName);
            LinqUtility.CheckNotNull("innerExpression", innerExpression);

            return new NamedExpression(memberName, innerExpression);
        }

        public static Expression CreateNewExpressionWithNamedArguments(IArangoDatabase db, NewExpression expression)
        {
            LinqUtility.CheckNotNull("expression", expression);

            return CreateNewExpressionWithNamedArguments(db, expression, expression.Arguments);
        }

        public static Expression CreateNewExpressionWithNamedArguments(IArangoDatabase db, NewExpression expression, IEnumerable<Expression> processedArguments)
        {
            //var newArguments = processedArguments.Select((e, i) => WrapIntoNamedExpression(GetMemberName(expression.Members, i), e)).ToArray();
            var newArguments = processedArguments.Select((e, i) => WrapIntoNamedExpression(db, expression.Members[i], e)).ToArray();

            if (!newArguments.SequenceEqual(expression.Arguments))
            {
                // ReSharper disable ConditionIsAlwaysTrueOrFalse - ReSharper is wrong, expression.Members can be null
                // ReSharper disable HeuristicUnreachableCode
                if (expression.Members != null)
                    return New(expression.Constructor, newArguments, expression.Members);
                else
                    return New(expression.Constructor, newArguments);
                // ReSharper restore HeuristicUnreachableCode
                // ReSharper restore ConditionIsAlwaysTrueOrFalse
            }

            return expression;
        }

        //public static Expression WrapIntoNamedExpression(string memberName, Expression argumentExpression)
        //{
        //    var expressionAsNamedExpression = argumentExpression as NamedExpression;
        //    if (expressionAsNamedExpression != null && expressionAsNamedExpression.Name == memberName)
        //        return expressionAsNamedExpression;

        //    return CreateFromMemberName(memberName, argumentExpression);
        //}

        public static Expression WrapIntoNamedExpression(IArangoDatabase db, MemberInfo memberInfo, Expression argumentExpression)
        {
            var memberName = LinqUtility.ResolveMemberNameRaw(db, memberInfo);

            var expressionAsNamedExpression = argumentExpression as NamedExpression;
            if (expressionAsNamedExpression != null && expressionAsNamedExpression.Name == memberName)
                return expressionAsNamedExpression;

            var namedExpression = CreateFromMemberName(memberName, argumentExpression);

            return namedExpression;
        }

        public static string GetMemberName(ReadOnlyCollection<MemberInfo> members, int index)
        {
            if (members == null || members.Count <= index)
                return "m" + index;
            return StripGetPrefix(members[index].Name);
        }

        private static string StripGetPrefix(string memberName)
        {
            if (memberName.StartsWith("get_") && memberName.Length > 4)
                memberName = memberName.Substring(4);
            return memberName;
        }

        private readonly string _name;
        private readonly Expression _expression;

        public NamedExpression(string name, Expression expression)
        {
            LinqUtility.CheckNotNull("expression", expression);

            _name = name;
            _expression = expression;
        }

        public override ExpressionType NodeType
        {
            get { return ExpressionType.Extension; }
        }

        public override Type Type
        {
            get { return _expression.Type; }
        }

        public string Name
        {
            get { return _name; }
        }

        public Expression Expression
        {
            get { return _expression; }
        }

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            LinqUtility.CheckNotNull("visitor", visitor);

            var newExpression = visitor.Visit(_expression);
            if (newExpression != _expression)
                return new NamedExpression(_name, newExpression);
            else
                return this;
        }

        protected override Expression Accept(ExpressionVisitor visitor)
        {
            LinqUtility.CheckNotNull("visitor", visitor);

            var specificVisitor = visitor as INamedExpressionVisitor;
            if (specificVisitor != null)
                return specificVisitor.VisitNamed(this);
            else
                return base.Accept(visitor);
        }

        public override string ToString()
        {
            return String.Format("{0} = {1}", _expression, _name ?? DefaultName);
        }
    }

    public interface INamedExpressionVisitor
    {
        Expression VisitNamed(NamedExpression expression);
    }

}
