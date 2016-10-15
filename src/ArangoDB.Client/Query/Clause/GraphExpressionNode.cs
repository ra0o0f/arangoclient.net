using ArangoDB.Client.Data;
using ArangoDB.Client.Utility;
using Remotion.Linq;
using Remotion.Linq.Parsing.ExpressionVisitors;
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
    public class TraversalExpressionNode : MethodCallExpressionNodeBase, IQuerySourceExpressionNode
    {
        public static readonly MethodInfo[] SupportedMethods = new[]
                                                           {
                                                                LinqUtility.GetSupportedMethod(()=>QueryableExtensions.Graph<object,object>(null, null, null, null)),
                                                                LinqUtility.GetSupportedMethod(()=>QueryableExtensions.Graph<object,object>(null,null)),
                                                                LinqUtility.GetSupportedMethod(()=>QueryableExtensions.InternalEdges<object,object>(null, null, null, null)),
                                                                LinqUtility.GetSupportedMethod(()=>QueryableExtensions.Edges<object,object>(null,null))
                                                           };

        public ConstantExpression TraversalContext { get; private set; }
        public ConstantExpression VertextType { get; private set; }
        public ConstantExpression EdgeType { get; private set; }

        string identifier;

        public TraversalExpressionNode(MethodCallExpressionParseInfo parseInfo, 
            ConstantExpression traversalContext,
            ConstantExpression vertexType,
            ConstantExpression edgeType)
            : base(parseInfo)
        {
            TraversalContext = traversalContext;
            if(vertexType != null && edgeType != null)
            {
                VertextType = vertexType;
                EdgeType = edgeType;
            }
            else
            {
                var genericTypes = parseInfo.ParsedExpression.Type.GenericTypeArguments[0].GenericTypeArguments;
                VertextType = Expression.Constant(genericTypes[0]);
                EdgeType = Expression.Constant(genericTypes[1]);
            }

            identifier = Guid.NewGuid().ToString("N");

            _resolvedAdaptedSelector = new ResolvedExpressionCache<Expression>(this);
        }

        public override Expression Resolve(ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
        {
            LinqUtility.CheckNotNull("inputParameter", inputParameter);
            LinqUtility.CheckNotNull("expressionToBeResolved", expressionToBeResolved);

            var resolvedSelector = GetResolvedAdaptedSelector(clauseGenerationContext);
            var resolved = ReplacingExpressionVisitor.Replace(inputParameter, Expression.Parameter(resolvedSelector.Type, identifier), expressionToBeResolved);
            return resolved;
        }

        protected override void ApplyNodeSpecificSemantics(QueryModel queryModel, ClauseGenerationContext clauseGenerationContext)
        {
            LinqUtility.CheckNotNull("queryModel", queryModel);

            var traversalClause = new TraversalClause(TraversalContext, identifier);

            queryModel.BodyClauses.Add(traversalClause);

            clauseGenerationContext.AddContextInfo(this, traversalClause);

            //queryModel.SelectClause.Selector = GetResolvedAdaptedSelector(clauseGenerationContext);
        }

        private readonly ResolvedExpressionCache<Expression> _resolvedAdaptedSelector;

        public Expression GetResolvedAdaptedSelector(ClauseGenerationContext clauseGenerationContext)
        {
            return _resolvedAdaptedSelector.GetOrCreate(
                r =>
                {
                    var traversalDataType = typeof(TraversalData<,>).MakeGenericType(new Type[] { VertextType.Value as Type, EdgeType.Value as Type });
                    
                    var constr = ReflectionUtils.GetConstructors(traversalDataType).ToList()[0];
                    var newExpression =
                        Expression.Convert(
                        Expression.New(constr), traversalDataType)
                        ;

                    return r.GetResolvedExpression(newExpression, Expression.Parameter(traversalDataType,"dummy"), clauseGenerationContext);
                });
        }
    }
}
