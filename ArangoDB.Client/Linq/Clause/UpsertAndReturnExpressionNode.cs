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
    public class UpsertAndReturnExpressionNode : MethodCallExpressionNodeBase
    {
        public static readonly MethodInfo[] SupportedMethods = new[]
                                                           {
                                                               GetSupportedMethod(() => QueryableExtensions.InternalUpsert<object,object>(null,o=>null,o=>null,(o,old)=>null,null))
                                                           };

        private readonly ResolvedExpressionCache<Expression> _cachedSearchSelector;
        private readonly ResolvedExpressionCache<Expression> _cachedInsertSelector;
        private readonly ResolvedExpressionCache<Expression> _cachedUpdateSelector;

        public LambdaExpression SearchSelector { get; private set; }
        public LambdaExpression InsertSelector { get; private set; }
        public LambdaExpression UpdateSelector { get; private set; }
        public ConstantExpression UpdateType { get; private set; }

        public UpsertAndReturnExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression searchSelector
            , LambdaExpression insertSelector, LambdaExpression updateSelector, ConstantExpression type)
            : base(parseInfo)
        {
            Utils.CheckNotNull("searchSelector", searchSelector);
            Utils.CheckNotNull("insertSelector", insertSelector);
            Utils.CheckNotNull("updateSelector", updateSelector);
            Utils.CheckNotNull("type", type);

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
            Utils.CheckNotNull("inputParameter", inputParameter);
            Utils.CheckNotNull("expressionToBeResolved", expressionToBeResolved);

            return Source.Resolve(inputParameter, expressionToBeResolved, clauseGenerationContext);
        }

        protected override QueryModel ApplyNodeSpecificSemantics(QueryModel queryModel, ClauseGenerationContext clauseGenerationContext)
        {
            Utils.CheckNotNull("queryModel", queryModel);

            queryModel.BodyClauses.Add(new UpsertAndReturnClause(
                GetResolvedSearchPredicate(clauseGenerationContext),
                GetResolvedInsertPredicate(clauseGenerationContext),
                GetResolvedUpdatePredicate(clauseGenerationContext),
                queryModel.MainFromClause.ItemName,
                UpdateType.Value as Type
                ));

            return queryModel;
        }
    }
    
}
