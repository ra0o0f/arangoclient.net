using ArangoDB.Client.Common.Remotion.Linq;
using ArangoDB.Client.Common.Remotion.Linq.Clauses.Expressions;
using ArangoDB.Client.Common.Remotion.Linq.Parsing.ExpressionTreeVisitors;
using ArangoDB.Client.Common.Remotion.Linq.Parsing.Structure.IntermediateModel;
using ArangoDB.Client.Common.Utility;
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
    public class GroupByExpressionNode : SelectExpressionNode, IQuerySourceExpressionNode
    {
        public static readonly MethodInfo[] GroupBySupportedMethods = new[]
                                                           {
                                                               GetSupportedMethod (() => Queryable.GroupBy<object, object> (null, o => null)),
                                                               GetSupportedMethod (() => Enumerable.GroupBy<object, object> (null, o => null)),
                                                               GetSupportedMethod (() => Queryable.GroupBy<object, object, object> (null, o => null, o => null)),
                                                               GetSupportedMethod (() => Enumerable.GroupBy<object, object, object> (null, o => null, o => null)),
                                                               GetSupportedMethod (() => QueryableExtensions.Collect<object, object> (null, o => null)),
                                                           };

        private readonly ResolvedExpressionCache<Expression> _resolvedAdaptedSelector;

        string intoIdentifier;

        public GroupByExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression selector)
            : base(Utils.CheckNotNull("parseInfo", parseInfo), Utils.CheckNotNull("selector", selector))
        {
            intoIdentifier = parseInfo.AssociatedIdentifier;
            _resolvedAdaptedSelector = new ResolvedExpressionCache<Expression>(this);
        }

        public override Expression Resolve(
            ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
        {
            Utils.CheckNotNull("inputParameter", inputParameter);
            Utils.CheckNotNull("expressionToBeResolved", expressionToBeResolved);

            var resolvedSelector = GetResolvedAdaptedSelector(clauseGenerationContext);
            return ReplacingExpressionTreeVisitor.Replace(inputParameter, resolvedSelector, expressionToBeResolved);
        }

        protected override QueryModel ApplyNodeSpecificSemantics(QueryModel queryModel, ClauseGenerationContext clauseGenerationContext)
        {
            Utils.CheckNotNull("queryModel", queryModel);

            var groupByClause = new GroupByClause(GetResolvedSelector(clauseGenerationContext),Selector,intoIdentifier);
            queryModel.BodyClauses.Add(groupByClause);

            clauseGenerationContext.AddContextInfo(this, groupByClause);

            queryModel.SelectClause.Selector = GetResolvedAdaptedSelector(clauseGenerationContext);


            return queryModel;
        }

        public Expression GetResolvedAdaptedSelector(ClauseGenerationContext clauseGenerationContext)
        {
            return _resolvedAdaptedSelector.GetOrCreate(
                r =>
                {
                    var groupingType = typeof(Grouping<,>).MakeGenericType(new Type[] { Selector.Body.Type, Selector.Parameters[0].Type });
                    var interface_groupingType = typeof(IGrouping<,>).MakeGenericType(new Type[] { Selector.Body.Type, Selector.Parameters[0].Type });

                    var constr = CommonUtility.GetConstructors(groupingType).ToList()[0];
                    var parameters = constr.GetParameters().Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToArray();
                    var newExpression =
                        Expression.Convert(
                        Expression.New(constr, parameters
                        , new MemberInfo[] { CommonUtility.GetMember(groupingType,"Key")[0] })
                        , interface_groupingType)
                        ;

                    return r.GetResolvedExpression(newExpression, Selector.Parameters[0], clauseGenerationContext);
                });
        }
    }
}
