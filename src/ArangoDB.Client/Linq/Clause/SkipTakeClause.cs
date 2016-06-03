using ArangoDB.Client.Common.Remotion.Linq;
using ArangoDB.Client.Common.Remotion.Linq.Clauses;
using ArangoDB.Client.Common.Remotion.Linq.Clauses.ExpressionTreeVisitors;
using ArangoDB.Client.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ArangoDB.Client.Linq.Clause
{
    public class SkipTakeClause : IBodyClause
    {
        private Expression _skipCount;
        private Expression _takeCount;

        public SkipTakeClause(Expression skipCount, Expression takeCount)
        {
            if(skipCount!=null)
                SkipCount = skipCount;

            if (takeCount != null)
                TakeCount = takeCount;
        }

        public Expression SkipCount
        {
            get { return _skipCount; }
            set
            {
                Utils.CheckNotNull("value", value);

                var lambda = value as LambdaExpression;
                if (lambda==null && value.Type != typeof(int))
                {
                    var message = string.Format("The value expression returns '{0}', an expression returning 'System.Int32' was expected.", value.Type);
                    throw new ArgumentException(message, "value");
                }

                _skipCount = value;
            }
        }

        public Expression TakeCount
        {
            get { return _takeCount; }
            set
            {
                Utils.CheckNotNull("value", value);

                var lambda = value as LambdaExpression;
                if (lambda == null && value.Type != typeof(int))
                {
                    var message = string.Format("The value expression returns '{0}', an expression returning 'System.Int32' was expected.", value.Type);
                    throw new ArgumentException(message, "value");
                }

                _takeCount = value;
            }
        }

        public int? GetConstantTakeCount()
        {
            return TakeCount == null ? null : new Nullable<int>(GetConstantValueFromExpression<int>("count", TakeCount));
        }

        public int? GetConstantSkipCount()
        {
            return SkipCount == null ? null : new Nullable<int>(GetConstantValueFromExpression<int>("count", SkipCount));
        }

        T GetConstantValueFromExpression<T>(string expressionName, Expression expression)
        {
            Utils.CheckNotNull("expression", expression);
            if (!typeof(T).GetTypeInfo().IsAssignableFrom(expression.Type.GetTypeInfo()))
            {
                var message = string.Format(
                    "The value stored by the {0} expression ('{1}') is not of type '{2}', it is of type '{3}'.",
                    expressionName,
                    FormattingExpressionTreeVisitor.Format(expression),
                    typeof(T),
                    expression.Type);
                throw new ArgumentException(message, "expression");
            }

            var itemAsConstantExpression = expression as ConstantExpression;
            if (itemAsConstantExpression != null)
            {
                return (T)itemAsConstantExpression.Value;
            }
            else
            {
                var message = string.Format(
                    "The {0} expression ('{1}') is no ConstantExpression, it is a {2}.",
                    expressionName,
                    FormattingExpressionTreeVisitor.Format(expression),
                    expression.GetType().Name);
                throw new ArgumentException(message, "expression");
            }
        }

        public virtual void Accept(IQueryModelVisitor visitor, QueryModel queryModel, int index)
        {
            Utils.CheckNotNull("visitor", visitor);
            Utils.CheckNotNull("queryModel", queryModel);

            var aqlVisotor = visitor as AqlModelVisitor;

            if (aqlVisotor == null)
                throw new Exception("QueryModelVisitor should be type of AqlModelVisitor");

            aqlVisotor.VisitSkipTakeClause(this, queryModel, index);
        }

        public virtual SkipTakeClause Clone(CloneContext cloneContext)
        {
            Utils.CheckNotNull("cloneContext", cloneContext);

            var clone = new SkipTakeClause(SkipCount,TakeCount);
            return clone;
        }

        IBodyClause IBodyClause.Clone(CloneContext cloneContext)
        {
            return Clone(cloneContext);
        }

        public void TransformExpressions(Func<Expression, Expression> transformation)
        {
            Utils.CheckNotNull("transformation", transformation);
            if(SkipCount!=null)
                SkipCount = transformation(SkipCount);
            TakeCount = transformation(TakeCount);
        }

        public override string ToString()
        {
            return string.Format("limit {0},{1}",FormattingExpressionTreeVisitor.Format(SkipCount)
                , FormattingExpressionTreeVisitor.Format(TakeCount));
        }
    }
}
