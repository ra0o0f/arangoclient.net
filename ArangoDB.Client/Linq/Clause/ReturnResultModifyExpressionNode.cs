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
    public class ReturnResultModifyExpressionNode : MethodCallExpressionNodeBase
    {
        public static readonly MethodInfo[] SupportedMethods = new[]
                                                                {
                                                                    GetSupportedMethod(() => QueryableExtensions.ReturnResult<object>(null,false))
                                                                };

        ConstantExpression returnNewResult;

        public ReturnResultModifyExpressionNode(MethodCallExpressionParseInfo parseInfo, ConstantExpression returnNewResult)
            : base(parseInfo)
        {
            this.returnNewResult = returnNewResult;
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
            updateClause.ReturnResult = true;
            updateClause.ReturnNewResult = (bool)returnNewResult.Value;

            return queryModel;
        }
    }
}
