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
    public class UpsertExpressionNode : MethodCallExpressionNodeBase
    {
        public static readonly MethodInfo[] SupportedMethods = new[]
                                                           {
                                                               LinqUtility.GetSupportedMethod(() => QueryableExtensions.InternalUpsert<object,object>(null,o=>null,o=>null,(o,old)=>null,null))
                                                           };

        private readonly ResolvedExpressionCache<Expression> _cachedSearchSelector;
        private readonly ResolvedExpressionCache<Expression> _cachedInsertSelector;
        private readonly ResolvedExpressionCache<Expression> _cachedUpdateSelector;

        public LambdaExpression SearchSelector { get; private set; }
        public LambdaExpression InsertSelector { get; private set; }
        public LambdaExpression UpdateSelector { get; private set; }
        public ConstantExpression UpdateType { get; private set; }

        public UpsertExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression searchSelector
            , LambdaExpression insertSelector, LambdaExpression updateSelector, ConstantExpression type)
            : base(parseInfo)
        {
            LinqUtility.CheckNotNull("searchSelector", searchSelector);
            LinqUtility.CheckNotNull("insertSelector", insertSelector);
            LinqUtility.CheckNotNull("updateSelector", updateSelector);
            LinqUtility.CheckNotNull("type", type);

            SearchSelector = searchSelector;
            InsertSelector = insertSelector;
            UpdateSelector = updateSelector;

            UpdateType = type;

            _cachedSearchSelector = new ResolvedExpressionCache<Expression>(this);
            _cachedInsertSelector = new ResolvedExpressionCache<Expression>(this);
            _cachedUpdateSelector = new ResolvedExpressionCache<Expression>(this);
        }

        public Expression GetResolvedSearchPredicate(ClauseGenerationContext clauseGenerationContext)
        {
            return _cachedSearchSelector.GetOrCreate(r => r.GetResolvedExpression(SearchSelector.Body, SearchSelector.Parameters[0], clauseGenerationContext));
        }
        public Expression GetResolvedInsertPredicate(ClauseGenerationContext clauseGenerationContext)
        {
            return _cachedInsertSelector.GetOrCreate(r => r.GetResolvedExpression(InsertSelector.Body, InsertSelector.Parameters[0], clauseGenerationContext));
        }
        public Expression GetResolvedUpdatePredicate(ClauseGenerationContext clauseGenerationContext)
        {
            return _cachedUpdateSelector.GetOrCreate(r => r.GetResolvedExpression(UpdateSelector.Body, UpdateSelector.Parameters[0], clauseGenerationContext));
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

            queryModel.BodyClauses.Add(new UpsertClause(
                    GetResolvedSearchPredicate(clauseGenerationContext),
                    GetResolvedInsertPredicate(clauseGenerationContext),
                    GetResolvedUpdatePredicate(clauseGenerationContext),
                    queryModel.MainFromClause.ItemName,
                    UpdateType.Value as Type
                ));
        }
    }
}
