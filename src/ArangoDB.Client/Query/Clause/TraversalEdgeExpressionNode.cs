using ArangoDB.Client.Data;
using ArangoDB.Client.Query;
using ArangoDB.Client.Query.Clause;
using Remotion.Linq;
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
    public class TraversalEdgeExpressionNode : MethodCallExpressionNodeBase, IQuerySourceExpressionNode
    {
        public static readonly MethodInfo[] SupportedMethods = new[]
                                                           {
                                                                LinqUtility.GetSupportedMethod(()=>TraversalQueryableExtensions.Edge<object,object>(null,null)),
                                                                LinqUtility.GetSupportedMethod(()=>TraversalQueryableExtensions.Edge<object,object>(null,null,EdgeDirection.Any)),
                                                                LinqUtility.GetSupportedMethod(()=>ShortestPathQueryableExtensions.Edge<object,object>(null,null)),
                                                                LinqUtility.GetSupportedMethod(()=>ShortestPathQueryableExtensions.Edge<object,object>(null,null,EdgeDirection.Any))
                                                           };

        public ConstantExpression CollectionName { get; private set; }

        public ConstantExpression Direction { get; private set; }

        public TraversalEdgeExpressionNode(MethodCallExpressionParseInfo parseInfo
            , ConstantExpression collectionName
            , ConstantExpression direction)
            : base(parseInfo)
        {
            CollectionName = collectionName;
            Direction = direction;
        }

        public override Expression Resolve(ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
        {
            LinqUtility.CheckNotNull("inputParameter", inputParameter);
            LinqUtility.CheckNotNull("expressionToBeResolved", expressionToBeResolved);

            return Source.Resolve(inputParameter, expressionToBeResolved, clauseGenerationContext);
        }

        protected override void ApplyNodeSpecificSemantics(QueryModel queryModel, ClauseGenerationContext clauseGenerationContext)
        {
            LinqUtility.CheckNotNull("queryModel", queryModel);

            var traversalClause = queryModel.BodyClauses.Last(b => b is ITraversalClause) as ITraversalClause;

            traversalClause.EdgeCollections.Add(new TraversalEdgeDefinition
            {
                CollectionName = CollectionName.Value.ToString(),
                Direction = Direction != null ? Direction.Value as EdgeDirection? : null
            });
        }
    }
}
