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
    public class RemoveExpressionNode : MethodCallExpressionNodeBase
    {
        public static readonly MethodInfo[] SupportedMethods = new[]
                                                           {
                                                               LinqUtility.GetSupportedMethod(() => QueryableExtensions.Remove<object>(null,o=>null,null))
                                                           };

        private readonly ResolvedExpressionCache<Expression> _cachedKeySelector;

        public LambdaExpression KeySelector { get; private set; }

        public ConstantExpression Collection { get; private set; }

        public RemoveExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression keySelector
            , ConstantExpression collection)
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
        
        public Expression GetResolvedKeyPredicate(ClauseGenerationContext clauseGenerationContext)
        {
            return _cachedKeySelector.GetOrCreate(r => r.GetResolvedExpression(KeySelector.Body, KeySelector.Parameters[0], clauseGenerationContext));
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

            queryModel.BodyClauses.Add(new RemoveClause(
                queryModel.MainFromClause.ItemName,
                (Type)Collection.Value,
                KeySelector != null ? GetResolvedKeyPredicate(clauseGenerationContext) : null));
        }
    }
}
