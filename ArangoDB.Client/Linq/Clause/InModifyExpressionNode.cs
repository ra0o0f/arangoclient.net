using ArangoDB.Client.Common.Remotion.Linq;
using ArangoDB.Client.Common.Remotion.Linq.Parsing.Structure.IntermediateModel;
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
    public class InModifyExpressionNode : MethodCallExpressionNodeBase
    {
        public static readonly MethodInfo[] SupportedMethods = new[]
                                                                {
                                                                    GetSupportedMethod(() => QueryableExtensions.In<object>(null))
                                                                };

        public Type CollectionToModify;

        public InModifyExpressionNode(MethodCallExpressionParseInfo parseInfo)
            : base(parseInfo)
        {
            CollectionToModify = parseInfo.ParsedExpression.Type.GenericTypeArguments[0];
        }

        public override Expression Resolve(ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
        {
            Utils.CheckNotNull("inputParameter", inputParameter);
            Utils.CheckNotNull("expressionToBeResolved", expressionToBeResolved);

            return Source.Resolve(inputParameter, expressionToBeResolved, clauseGenerationContext);
        }

        protected override QueryModel ApplyNodeSpecificSemantics(QueryModel queryModel, ClauseGenerationContext clauseGenerationContext)
        {
            Utils.CheckNotNull("queryModel", queryModel);

            var updateClause = queryModel.BodyClauses.First(x => x as IModifyExpressionNode != null) as IModifyExpressionNode;
            updateClause.CollectionType = CollectionToModify;

            return queryModel;
        }
    }
}
