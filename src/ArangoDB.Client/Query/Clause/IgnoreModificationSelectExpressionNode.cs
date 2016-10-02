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
    public class IgnoreModificationSelectExpressionNode : MethodCallExpressionNodeBase
    {
        public static readonly MethodInfo[] SupportedMethods = new[]
                                                                {
                                                                    LinqUtility.GetSupportedMethod(() => QueryableExtensions.IgnoreModificationSelect<object>(null))
                                                                };
        
        public IgnoreModificationSelectExpressionNode(MethodCallExpressionParseInfo parseInfo)
            : base(parseInfo)
        {
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

            var modificationClause = queryModel.BodyClauses.NextBodyClause<IModificationClause>();
            modificationClause.IgnoreSelect = true;
        }
    }
}
