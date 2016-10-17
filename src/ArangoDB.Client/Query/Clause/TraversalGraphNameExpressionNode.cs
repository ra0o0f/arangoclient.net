using ArangoDB.Client.Data;
using ArangoDB.Client.Utility;
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
    public class TraversalGraphNameExpressionNode : MethodCallExpressionNodeBase, IQuerySourceExpressionNode
    {
        public static readonly MethodInfo[] SupportedMethods = new[]
                                                           {
                                                                LinqUtility.GetSupportedMethod(()=>TraversalQueryableExtensions.Graph<object,object>(null,null)),
                                                                LinqUtility.GetSupportedMethod(()=>ShortestPathQueryableExtensions.Graph<object,object>(null,null))
                                                           };

        public ConstantExpression GraphName { get; private set; }

        public TraversalGraphNameExpressionNode(MethodCallExpressionParseInfo parseInfo, ConstantExpression graphName)
            : base(parseInfo)
        {
            GraphName = graphName;
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

            traversalClause.GraphName = GraphName.Value.ToString();
        }
    }
}
