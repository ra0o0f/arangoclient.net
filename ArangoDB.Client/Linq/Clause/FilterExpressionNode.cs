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

    public class FilterExpressionNode : MethodCallExpressionNodeBase
    {
        public static readonly MethodInfo[] SupportedMethods = new[]
                                                           {
                                                               GetSupportedMethod (() => QueryableExtensions.Filter<object> (null, o => true))
                                                           };

        private readonly ResolvedExpressionCache<Expression> _cachedPredicate;

        public FilterExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression predicate)
            : base(parseInfo)
        {
            if (predicate.Parameters.Count != 1)
                throw new ArgumentException("Predicate must have exactly one parameter.", "predicate");

            Predicate = predicate;
            _cachedPredicate = new ResolvedExpressionCache<Expression>(this);
        }

        public LambdaExpression Predicate { get; private set; }

        public Expression GetResolvedPredicate(ClauseGenerationContext clauseGenerationContext)
        {
            return _cachedPredicate.GetOrCreate(r => r.GetResolvedExpression(Predicate.Body, Predicate.Parameters[0], clauseGenerationContext));
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

            queryModel.BodyClauses.Add(new FilterClause(GetResolvedPredicate(clauseGenerationContext)));

            return queryModel;
        }
    }
}
