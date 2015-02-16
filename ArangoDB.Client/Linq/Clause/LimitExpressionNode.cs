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
    public class LimitExpressionNode : MethodCallExpressionNodeBase
    {
            public static readonly MethodInfo[] SupportedMethods = new[]
                                                           {
                                                               GetSupportedMethod (() => QueryableExtensions.Limit<object> (null, 0,0)),
                                                               GetSupportedMethod (() => QueryableExtensions.Limit<object> (null, 0))
                                                           };

            private readonly ResolvedExpressionCache<Expression> _cachedSelector;

            public LimitExpressionNode(MethodCallExpressionParseInfo parseInfo,ConstantExpression take, ConstantExpression skip)
                : base(parseInfo)
            {
                Utils.CheckNotNull("keySelector", take);
                Utils.CheckNotNull("keySelector", skip);

                Take = take;
                Skip = skip;
                _cachedSelector = new ResolvedExpressionCache<Expression>(this);
            }
            public ConstantExpression Take { get; private set; }
            public ConstantExpression Skip { get; private set; }

            public override Expression Resolve(
                ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
            {
                Utils.CheckNotNull("inputParameter", inputParameter);
                Utils.CheckNotNull("expressionToBeResolved", expressionToBeResolved);

                // this simply streams its input data to the output without modifying its structure, so we resolve by passing on the data to the previous node
                return Source.Resolve(inputParameter, expressionToBeResolved, clauseGenerationContext);
            }

            protected override QueryModel ApplyNodeSpecificSemantics(QueryModel queryModel, ClauseGenerationContext clauseGenerationContext)
            {
                Utils.CheckNotNull("queryModel", queryModel);

                queryModel.BodyClauses.Add(new SkipTakeClause(Skip, Take));

                return queryModel;
            }
        
    }
}
