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
    public class InsertAndReturnExpressionNode : MethodCallExpressionNodeBase
    {
        public static readonly MethodInfo[] SupportedMethods = new[]
                                                           {
                                                               GetSupportedMethod(() => QueryableExtensions.Insert<object>(null,o=>null,null))
                                                           };

        private readonly ResolvedExpressionCache<Expression> _cachedWithSelector;

        public ConstantExpression Collection { get; private set; }

        public InsertAndReturnExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression withSelector
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

        public LambdaExpression WithSelector { get; private set; }

        public Expression GetResolvedPredicate(ClauseGenerationContext clauseGenerationContext)
        {
            return _cachedWithSelector.GetOrCreate(r => r.GetResolvedExpression(WithSelector.Body, WithSelector.Parameters[0], clauseGenerationContext));
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

            queryModel.BodyClauses.Add(new InsertAndReturnClause(
                WithSelector != null ? GetResolvedPredicate(clauseGenerationContext) : null,
                queryModel.MainFromClause.ItemName,
                (Type)Collection.Value
                ));

            return queryModel;
        }
    }
}
