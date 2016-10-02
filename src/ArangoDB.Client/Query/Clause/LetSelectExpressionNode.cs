using Remotion.Linq;
using Remotion.Linq.Clauses.Expressions;
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
    public sealed class LetSelectExpressionNode : SelectExpressionNode, IQuerySourceExpressionNode
    {
        public static readonly MethodInfo[] SupportedMethods = new[]
                                                           {
                                                               LinqUtility.GetSupportedMethod (() => Queryable.Select<object, object> (null, o => null)),
                                                               LinqUtility.GetSupportedMethod (() => Enumerable.Select<object, object> (null, o => null)),
                                                               //GetSupportedMethod (() => QueryableExtensions.Let<object, object> (null, o => null)),
                                                               LinqUtility.GetSupportedMethod (() => QueryableExtensions.Return<object, object> (null, o => null))
                                                           };

        private readonly NewExpression _letConstruction;

        private readonly ResolvedExpressionCache<Expression> _resolvedAdaptedSelector;
        private readonly ResolvedExpressionCache<Expression> _resolvedLetExpression;

        public LetSelectExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression selector)
            : base(LinqUtility.CheckNotNull("parseInfo", parseInfo), LinqUtility.CheckNotNull("selector", selector))
        {
            // Heuristically detect "let" constructs
            // Assume a Select call is a "let" construct if its selector looks like this: <x> => new { <x> = <x>, <y> = ... }
            var selectorBody = selector.Body as NewExpression;
            if (selectorBody != null
                && selectorBody.Arguments.Count == 2
                && selectorBody.Arguments[0] == selector.Parameters[0]
                //&& selectorBody.Constructor.DeclaringType.IsDefined(typeof(CompilerGeneratedAttribute), false)
                && selectorBody.Members != null
                && selectorBody.Members.Count == 2)
            {
                _letConstruction = selectorBody;

                _resolvedAdaptedSelector = new ResolvedExpressionCache<Expression>(this);
                _resolvedLetExpression = new ResolvedExpressionCache<Expression>(this);
            }
            else
            {
                _letConstruction = null;

                _resolvedAdaptedSelector = null;
                _resolvedLetExpression = null;
            }
        }

        public bool IsLetNode
        {
            get { return _letConstruction != null; }
        }

        public Expression GetResolvedAdaptedSelector(ClauseGenerationContext clauseGenerationContext)
        {
            if (!IsLetNode)
                throw new InvalidOperationException("This node is not a 'let' node.");

            // Adapt the selector so that it contains a reference to the associated LetClause, i.e., instead of "x => new { x = x, y = expr }",
            // make it "x => new { x = x, y = [letClause] }"
            return _resolvedAdaptedSelector.GetOrCreate(
                r =>
                {
                    var letClause = QuerySourceExpressionNodeUtility.GetQuerySourceForNode(this, clauseGenerationContext);

                    var adaptedSelectorBody = Expression.New(
                        _letConstruction.Constructor,
                        new[] { _letConstruction.Arguments[0], new QuerySourceReferenceExpression(letClause) },
                        _letConstruction.Members);
                    return r.GetResolvedExpression(adaptedSelectorBody, Selector.Parameters[0], clauseGenerationContext);
                });
        }

        public Expression GetResolvedLetExpression(ClauseGenerationContext clauseGenerationContext)
        {
            if (!IsLetNode)
                throw new InvalidOperationException("This node is not a 'let' node.");

            return
                _resolvedLetExpression.GetOrCreate(
                    r => r.GetResolvedExpression(_letConstruction.Arguments[1], Selector.Parameters[0], clauseGenerationContext));
        }

        public override Expression Resolve(
            ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
        {
            LinqUtility.CheckNotNull("inputParameter", inputParameter);
            LinqUtility.CheckNotNull("expressionToBeResolved", expressionToBeResolved);

            if (IsLetNode)
            {
                // We modify the structure of the stream of data coming into this node by our selector,
                // so we first resolve the selector (adapted to include a reference to the let clause), then we substitute the result for the inputParameter 
                // in the expressionToBeResolved.
                var resolvedSelector = GetResolvedAdaptedSelector(clauseGenerationContext);
                return ReplacingExpressionVisitor.Replace(inputParameter, resolvedSelector, expressionToBeResolved);
            }

            return base.Resolve(inputParameter, expressionToBeResolved, clauseGenerationContext);
        }

        protected override void ApplyNodeSpecificSemantics(QueryModel queryModel, ClauseGenerationContext clauseGenerationContext)
        {
            LinqUtility.CheckNotNull("queryModel", queryModel);

            if (IsLetNode)
            {
                // For let nodes, we insert a LetClause instance, then adapt the SelectClause to select the data bubbling out of the LetClause.
                var resolvedLetExpression = GetResolvedLetExpression(clauseGenerationContext);
                var letClause = new LetClause(_letConstruction.Constructor.GetParameters()[1].Name, resolvedLetExpression);
                queryModel.BodyClauses.Add(letClause);

                clauseGenerationContext.AddContextInfo(this, letClause);

                queryModel.SelectClause.Selector = GetResolvedAdaptedSelector(clauseGenerationContext);
            }
            else
                base.ApplyNodeSpecificSemantics(queryModel, clauseGenerationContext);
        }
    }
}
