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
    public class RemoveAndReturnExpressionNode : MethodCallExpressionNodeBase
    {
        public static readonly MethodInfo[] SupportedMethods = new[]
                                                           {
                                                               GetSupportedMethod(() => QueryableExtensions.Remove<object>(null,o=>null,null))
                                                           };

        private readonly ResolvedExpressionCache<Expression> _cachedKeySelector;

        public RemoveAndReturnExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression keySelector
            ,ConstantExpression collection)
            : base(parseInfo)
        {
            var keyConstant = keySelector.Body as ConstantExpression;
            if (keyConstant == null || keyConstant.Value != null)
            {
                KeySelector = keySelector;
                _cachedKeySelector = new ResolvedExpressionCache<Expression>(this);
            }

            Collection = collection;
        }

        public LambdaExpression KeySelector { get; private set; }

        public ConstantExpression Collection { get; private set; }

        public Expression GetResolvedKeyPredicate(ClauseGenerationContext clauseGenerationContext)
        {
            return _cachedKeySelector.GetOrCreate(r => r.GetResolvedExpression(KeySelector.Body, KeySelector.Parameters[0], clauseGenerationContext));
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

            queryModel.BodyClauses.Add(new RemoveAndReturnClause(
                queryModel.MainFromClause.ItemName,
                (Type)Collection.Value,
                KeySelector != null ? GetResolvedKeyPredicate(clauseGenerationContext) : null));

            return queryModel;
        }
    }
}
