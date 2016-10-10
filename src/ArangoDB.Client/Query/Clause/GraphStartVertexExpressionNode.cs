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
    public class GraphStartVertexExpressionNode : MethodCallExpressionNodeBase
    {
        public static readonly MethodInfo[] SupportedMethods = new[]
                                                           {
                                                                LinqUtility.GetSupportedMethod(()=>QueryableExtensions.StartVertex<object,object>(null, "")),
                                                                LinqUtility.GetSupportedMethod(()=>QueryableExtensions.StartVertex<object,object>(null, ()=>""))
                                                           };

        public Expression StartVertex { get; private set; }

        public GraphStartVertexExpressionNode(MethodCallExpressionParseInfo parseInfo,
            Expression startVertex)
            : base(parseInfo)
        {
            StartVertex = startVertex;
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

            traversalClause.StartVertex = StartVertex;
        }
    }
}
