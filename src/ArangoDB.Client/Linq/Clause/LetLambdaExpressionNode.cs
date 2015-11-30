using ArangoDB.Client.Common.Remotion.Linq;
using ArangoDB.Client.Common.Remotion.Linq.Parsing.ExpressionTreeVisitors;
using ArangoDB.Client.Common.Remotion.Linq.Parsing.Structure.IntermediateModel;
using ArangoDB.Client.Common.Utility;
using ArangoDB.Client.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Linq.Clause
{
    public class LetLambdaExpressionNode : MethodCallExpressionNodeBase, IQuerySourceExpressionNode
    {
        public static readonly MethodInfo[] SupportedMethods = new[]
                                                           {
                                                                GetSupportedMethod(()=>QueryableExtensions.Let<object,object,object>(null, o => null,(x,y)=>null)),
                                                                //GetSupportedMethod(()=>QueryableExtensions.Let<object,object,object>(null, o => null,(x)=>null))
                                                           };

        private readonly ResolvedExpressionCache<Expression> _cachedCollectionSelector;
        private readonly ResolvedExpressionCache<Expression> _cachedLetSelector;
        private readonly ResolvedExpressionCache<Expression> _cachedResultSelector;

        public LetLambdaExpressionNode(
        MethodCallExpressionParseInfo parseInfo, LambdaExpression letSelector, LambdaExpression collectionSelector)
            : base(parseInfo)
        {
            Utils.CheckNotNull("collectionSelector", collectionSelector);

            CollectionSelector = collectionSelector;

            LetSelector = letSelector;

            var parameter1 = Expression.Parameter(collectionSelector.Parameters[0].Type, collectionSelector.Parameters[0].Name);
            var itemType = CommonUtility.GetItemTypeOfClosedGenericIEnumerable(CollectionSelector.Body.Type, "collectionSelector");
            var parameter2 = Expression.Parameter(itemType, parseInfo.AssociatedIdentifier);
            ResultSelector = Expression.Lambda(parameter2, parameter1, parameter2);

            _cachedCollectionSelector = new ResolvedExpressionCache<Expression>(this);
            _cachedResultSelector = new ResolvedExpressionCache<Expression>(this);
            _cachedLetSelector = new ResolvedExpressionCache<Expression>(this);
        }

        public LambdaExpression CollectionSelector { get; private set; }
        public LambdaExpression ResultSelector { get; private set; }
        public LambdaExpression LetSelector { get; private set; }

        public Expression GetResolvedCollectionSelector(ClauseGenerationContext clauseGenerationContext)
        {
            return _cachedCollectionSelector.GetOrCreate(
                r => r.GetResolvedExpression(CollectionSelector.Body, CollectionSelector.Parameters[0], clauseGenerationContext));
        }

        public Expression GetResolvedLetSelector(ClauseGenerationContext clauseGenerationContext)
        {
            return _cachedLetSelector.GetOrCreate(
                r => r.GetResolvedExpression(LetSelector.Body, LetSelector.Parameters[0], clauseGenerationContext));
        }

        public Expression GetResolvedResultSelector(ClauseGenerationContext clauseGenerationContext)
        {
            // our result selector usually looks like this: (i, j) => new { i = i, j = j }
            // with the data for i coming from the previous node and j identifying the data from this node

            // we resolve the selector by first substituting j by a QuerySourceReferenceExpression pointing back to us, before asking the previous node 
            // to resolve i

            return _cachedResultSelector.GetOrCreate(
                r => r.GetResolvedExpression(
                         QuerySourceExpressionNodeUtility.ReplaceParameterWithReference
                         (this, ResultSelector.Parameters[0],
                         ResultSelector.Body,
                         clauseGenerationContext),
                         ResultSelector.Parameters[0],
                         clauseGenerationContext));

          //  return _cachedResultSelector.GetOrCreate(
          //r => r.GetResolvedExpression(
          //         QuerySourceExpressionNodeUtility.ReplaceParameterWithReference(this, ResultSelector.Parameters[1], ResultSelector.Body, clauseGenerationContext),
          //         ResultSelector.Parameters[0],
          //         clauseGenerationContext));
        }

        public override Expression Resolve(
            ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
        {
            Utils.CheckNotNull("inputParameter", inputParameter);
            Utils.CheckNotNull("expressionToBeResolved", expressionToBeResolved);

            // we modify the structure of the stream of data coming into this node by our result selector,
            // so we first resolve the result selector, then we substitute the result for the inputParameter in the expressionToBeResolved
            var resolvedResultSelector = GetResolvedResultSelector(clauseGenerationContext);
            return ReplacingExpressionTreeVisitor.Replace(inputParameter, resolvedResultSelector, expressionToBeResolved);
        }

        protected override QueryModel ApplyNodeSpecificSemantics(QueryModel queryModel, ClauseGenerationContext clauseGenerationContext)
        {
            Utils.CheckNotNull("queryModel", queryModel);

            var resolvedCollectionSelector = GetResolvedCollectionSelector(clauseGenerationContext);
            var resolvedLetSelector = GetResolvedLetSelector(clauseGenerationContext);

            var clause = new LetClause(CollectionSelector.Parameters[1].Name, resolvedLetSelector, resolvedCollectionSelector);
            queryModel.BodyClauses.Add(clause);

            //var clause = new AdditionalFromClause(ResultSelector.Parameters[1].Name, ResultSelector.Parameters[1].Type, resolvedCollectionSelector);
            //queryModel.BodyClauses.Add(clause);

            clauseGenerationContext.AddContextInfo(this, clause);

            var selectClause = queryModel.SelectClause;
            selectClause.Selector = 
                //GetResolvedCollectionSelector(clauseGenerationContext);
                GetResolvedResultSelector(clauseGenerationContext);

            return queryModel;
        }
    }
}
