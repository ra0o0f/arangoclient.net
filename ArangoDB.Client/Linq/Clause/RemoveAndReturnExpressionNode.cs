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
    //public class RemoveAndReturnExpressionNode : MethodCallExpressionNodeBase
    //{
    //    public static readonly MethodInfo[] SupportedMethods = new[]
    //                                                            {
    //                                                                GetSupportedMethod (() => QueryableExtensions.Remove<object,object>(null, o => null,false,null)),
    //                                                            };

    //    private readonly ResolvedExpressionCache<Expression> _cachedKeySelector;

    //    public RemoveAndReturnExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression keySelector,
    //        Expression returnModifiedResult, Expression inCollection)
    //        : base(parseInfo)
    //    {
    //        var keyConstant = keySelector.Body as ConstantExpression;
    //        if (keyConstant ==null || keyConstant.Value != null)
    //        {
    //            KeySelector = keySelector;
    //            _cachedKeySelector = new ResolvedExpressionCache<Expression>(this);
    //        }

    //        ReturnModifiedResult = returnModifiedResult;
    //        InCollection = inCollection;
    //    }

    //    public LambdaExpression KeySelector { get; private set; }
    //    public Expression ReturnModifiedResult { get; private set; }
    //    public Expression InCollection { get; private set; }

    //    public Expression GetResolvedKeyPredicate(ClauseGenerationContext clauseGenerationContext)
    //    {
    //        return _cachedKeySelector.GetOrCreate(r => r.GetResolvedExpression(KeySelector.Body, KeySelector.Parameters[0], clauseGenerationContext));
    //    }

    //    public override Expression Resolve(ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
    //    {
    //        Utils.CheckNotNull("inputParameter", inputParameter);
    //        Utils.CheckNotNull("expressionToBeResolved", expressionToBeResolved);

    //        return Source.Resolve(inputParameter, expressionToBeResolved, clauseGenerationContext);
    //    }

    //    protected override QueryModel ApplyNodeSpecificSemantics(QueryModel queryModel, ClauseGenerationContext clauseGenerationContext)
    //    {
    //        Utils.CheckNotNull("queryModel", queryModel);

    //        queryModel.BodyClauses.Add(new RemoveAndReturnClause(GetResolvedPredicate(clauseGenerationContext), WithSelector.Parameters[0].Name,
    //             KeySelector != null ? GetResolvedKeyPredicate(clauseGenerationContext) : null
    //             , ReturnModifiedResult, ReturnNewResult, InCollection));


    //        return queryModel;
    //    }
    //}
}
