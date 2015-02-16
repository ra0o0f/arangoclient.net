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

    public class SkipExpressionNode : MethodCallExpressionNodeBase
    {
        public static readonly MethodInfo[] SupportedMethods = new[]
                                                           {
                                                               GetSupportedMethod (() => Queryable.Skip<object> (null,0)),
                                                               GetSupportedMethod (() => Enumerable.Skip<object> (null,0))
                                                           };

        public SkipExpressionNode(MethodCallExpressionParseInfo parseInfo, Expression count)
            : base(parseInfo)
        {
            Count = count;
        }

        public Expression Count { get; private set; }


        public override Expression Resolve(ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
        {
            Utils.CheckNotNull("inputParameter", inputParameter);
            Utils.CheckNotNull("expressionToBeResolved", expressionToBeResolved);

            return Source.Resolve(inputParameter, expressionToBeResolved, clauseGenerationContext);
        }

        protected override QueryModel ApplyNodeSpecificSemantics(QueryModel queryModel, ClauseGenerationContext clauseGenerationContext)
        {
            Utils.CheckNotNull("queryModel", queryModel);

            var lastClause = queryModel.BodyClauses.LastOrDefault();
            SkipTakeClause skipTakeClause = lastClause as SkipTakeClause;
            if(skipTakeClause!=null)
                skipTakeClause.SkipCount = Count;
            else
                queryModel.BodyClauses.Add(new SkipTakeClause(Count, null));

            return queryModel;
        }
    }
}
