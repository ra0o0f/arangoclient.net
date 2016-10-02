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
    public class InsertExpressionNode : MethodCallExpressionNodeBase
    {
        public static readonly MethodInfo[] SupportedMethods = new[]
                                                           {
                                                               LinqUtility.GetSupportedMethod(() => QueryableExtensions.Insert<object>(null,o=>null,null))
                                                           };

        private readonly ResolvedExpressionCache<Expression> _cachedWithSelector;

        public LambdaExpression WithSelector { get; private set; }

        public ConstantExpression Collection { get; private set; }

        public InsertExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression withSelector
            , ConstantExpression collection)
            : base(parseInfo)
        {
            var withConstant = withSelector.Body as ConstantExpression;
            if (withConstant == null || withConstant.Value != null)
            {
                WithSelector = withSelector;
                _cachedWithSelector = new ResolvedExpressionCache<Expression>(this);
            }

            Collection = collection;
        }


        public Expression GetResolvedPredicate(ClauseGenerationContext clauseGenerationContext)
        {
            return _cachedWithSelector.GetOrCreate(r => r.GetResolvedExpression(WithSelector.Body, WithSelector.Parameters[0], clauseGenerationContext));
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

            queryModel.BodyClauses.Add(new InsertClause(
                WithSelector != null ? GetResolvedPredicate(clauseGenerationContext) : null,
                queryModel.MainFromClause.ItemName,
                (Type)Collection.Value
                ));
        }
    }
}
