using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Linq
{
    public class PredicateRewriter
    {
        public static Expression<Func<T1, T2, object>> Rewrite<T1, T2>(Expression<Func<T1, T2, object>> exp, string newParamName)
        {
            var param = Expression.Parameter(exp.Parameters[1].Type, newParamName);
            var unchangedParams = exp.Parameters.Take(1).ToList();
            var newExpression = new PredicateRewriterVisitor(param, unchangedParams).Visit(exp);
            return (Expression<Func<T1, T2, object>>)newExpression;
        }

        private class PredicateRewriterVisitor : ExpressionVisitor
        {
            private readonly IList<ParameterExpression> _unchangedParametersExpressions;
            private readonly ParameterExpression _newParameterExpression;

            public PredicateRewriterVisitor(ParameterExpression parameterExpression,
                IList<ParameterExpression> unchangedParametersExpressions)
            {
                _newParameterExpression = parameterExpression;
                _unchangedParametersExpressions = unchangedParametersExpressions;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (_unchangedParametersExpressions.Count(p => p.Name == node.Name)==0)
                    return _newParameterExpression;
                else
                    return node;
            }
        }
    }
}
