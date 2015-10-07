using ArangoDB.Client.Common.Remotion.Linq.Clauses.Expressions;
using ArangoDB.Client.Common.Remotion.Linq.Clauses.ExpressionTreeVisitors;
using ArangoDB.Client.Common.Remotion.Linq.Parsing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Linq
{
    public class NamedExpression : ExtensionExpression
    {
        public static T CheckNotNull<T>(string argumentName, T actualValue)
        {
            // ReSharper disable CompareNonConstrainedGenericWithNull
            if (actualValue == null)
                // ReSharper restore CompareNonConstrainedGenericWithNull
                throw new ArgumentNullException(argumentName);

            return actualValue;
        }

        public const string DefaultName = "value";

        private readonly string _name;
        private readonly Expression _expression;



        public static Expression CreateNewExpressionWithNamedArguments(NewExpression expression)
        {
            CheckNotNull("expression", expression);

            return CreateNewExpressionWithNamedArguments(expression, expression.Arguments);
        }

        public static Expression CreateNewExpressionWithNamedArguments(NewExpression expression, IEnumerable<Expression> processedArguments)
        {
            var newArguments = processedArguments.Select((e, i) => WrapIntoNamedExpression(GetMemberName(expression.Members, i), e)).ToArray();
            
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

        public static NamedExpression CreateFromMemberName(string memberName, Expression innerExpression)
        {
            CheckNotNull("memberName", memberName);
            CheckNotNull("innerExpression", innerExpression);

            return new NamedExpression(memberName, innerExpression);
        }
        public static Expression WrapIntoNamedExpression(string memberName, Expression argumentExpression)
        {
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

        public NamedExpression(string name, Expression expression)
            : base(expression.Type)
        {
            CheckNotNull("expression", expression);

            _name = name;
            _expression = expression;
        }

        public string Name
        {
            get { return _name; }
        }

        public Expression Expression
        {
            get { return _expression; }
        }

        protected override Expression VisitChildren(ExpressionTreeVisitor visitor)
        {
            CheckNotNull("visitor", visitor);

            var newExpression = visitor.VisitExpression(_expression);
            if (newExpression != _expression)
                return new NamedExpression(_name, newExpression);
            else
                return this;
        }

        public override Expression Accept(ExpressionTreeVisitor visitor)
        {
            CheckNotNull("visitor", visitor);

            var specificVisitor = visitor as INamedExpressionVisitor;
            if (specificVisitor != null)
                return specificVisitor.VisitNamedExpression(this);
            else
                return base.Accept(visitor);
        }

        public override string ToString()
        {
            return String.Format("{0}={1}", FormattingExpressionTreeVisitor.Format(_expression), _name ?? DefaultName);
        }
    }

    public interface INamedExpressionVisitor
    {
        Expression VisitNamedExpression(NamedExpression expression);
    }
}
