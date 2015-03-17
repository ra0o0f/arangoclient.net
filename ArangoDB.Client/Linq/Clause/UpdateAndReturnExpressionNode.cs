using ArangoDB.Client.Common.Remotion.Linq;
using ArangoDB.Client.Common.Remotion.Linq.Parsing.ExpressionTreeVisitors;
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
    public class UpdateAndReturnExpressionNode : MethodCallExpressionNodeBase
    {
        public static readonly MethodInfo[] SupportedMethods = new[]
                                                           {
                                                               GetSupportedMethod(() => QueryableExtensions.UpdateReplace<object>(null,o=>null,o=>null,null))
                                                           };

        private readonly ResolvedExpressionCache<Expression> _cachedWithSelector;
        private readonly ResolvedExpressionCache<Expression> _cachedKeySelector;

        public UpdateAndReturnExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression withSelector
            , LambdaExpression keySelector,ConstantExpression command)
            : base(parseInfo)
        {
            Utils.CheckNotNull("withSelector", withSelector);

            if (withSelector.Parameters.Count != 1)
                throw new ArgumentException("Selector must have exactly one parameter.", "selector");

            WithSelector = withSelector;
            _cachedWithSelector = new ResolvedExpressionCache<Expression>(this);

            var keyConstant = keySelector.Body as ConstantExpression;
            if (keyConstant ==null || keyConstant.Value != null)
            {
                KeySelector = keySelector;
                _cachedKeySelector = new ResolvedExpressionCache<Expression>(this);
            }

            Command = command;
        }

        public LambdaExpression WithSelector { get; private set; }
        public LambdaExpression KeySelector { get; private set; }

        public ConstantExpression Command { get; private set; }

        public Expression GetResolvedPredicate(ClauseGenerationContext clauseGenerationContext)
        {
            return _cachedWithSelector.GetOrCreate(r => r.GetResolvedExpression(WithSelector.Body, WithSelector.Parameters[0], clauseGenerationContext));
        }

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

            queryModel.BodyClauses.Add(new UpdateAndReturnClause(GetResolvedPredicate(clauseGenerationContext),
                queryModel.MainFromClause.ItemName,
                WithSelector.Parameters[0].Type,
                KeySelector != null ? GetResolvedKeyPredicate(clauseGenerationContext) : null,
                Command.Value.ToString()));
            
            return queryModel;
        }
    }
}