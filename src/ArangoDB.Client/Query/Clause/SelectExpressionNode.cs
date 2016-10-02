using Remotion.Linq;
using Remotion.Linq.Parsing.ExpressionVisitors;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Query.Clause
{
    public class SelectExpressionNode : MethodCallExpressionNodeBase
    {
        private const int c_indexSelectorParameterPosition = 1;

        // no direct use of SelectExpressionNode
        //public static IEnumerable<MethodInfo> GetSupportedMethods()
        //{
        //    return ReflectionUtility.EnumerableAndQueryableMethods.WhereNameMatches("Select").WithoutIndexSelector(c_indexSelectorParameterPosition);
        //}

        private readonly ResolvedExpressionCache<Expression> _cachedSelector;
        private readonly LambdaExpression _selector;

        public SelectExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression selector)
            : base(parseInfo)
        {
            LinqUtility.CheckNotNull("selector", selector);

            if (selector.Parameters.Count != 1)
                throw new ArgumentException("Selector must have exactly one parameter.", "selector");

            _selector = selector;
            _cachedSelector = new ResolvedExpressionCache<Expression>(this);
        }

        public LambdaExpression Selector
        {
            get { return _selector; }
        }

        public Expression GetResolvedSelector(ClauseGenerationContext clauseGenerationContext)
        {
            return _cachedSelector.GetOrCreate(r => r.GetResolvedExpression(Selector.Body, Selector.Parameters[0], clauseGenerationContext));
        }

        public override Expression Resolve(
            ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
        {
            LinqUtility.CheckNotNull("inputParameter", inputParameter);
            LinqUtility.CheckNotNull("expressionToBeResolved", expressionToBeResolved);

            // we modify the structure of the stream of data coming into this node by our selector,
            // so we first resolve the selector, then we substitute the result for the inputParameter in the expressionToBeResolved
            var resolvedSelector = GetResolvedSelector(clauseGenerationContext);
            return ReplacingExpressionVisitor.Replace(inputParameter, resolvedSelector, expressionToBeResolved);
        }

        protected override void ApplyNodeSpecificSemantics(QueryModel queryModel, ClauseGenerationContext clauseGenerationContext)
        {
            LinqUtility.CheckNotNull("queryModel", queryModel);
            queryModel.SelectClause.Selector = GetResolvedSelector(clauseGenerationContext);
        }
    }
}
