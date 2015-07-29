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
using ArangoDB.Client.Common.Remotion.Linq.Clauses;

namespace ArangoDB.Client.Linq.Clause
{
    public class InModifyExpressionNode : MethodCallExpressionNodeBase
    {
        public enum InModifiableClause
        {
            Main,
            Body
        }

        public static readonly MethodInfo[] SupportedMethods = new[]
                                                                {
                                                                    GetSupportedMethod(() => QueryableExtensions.In<object>((IAqlModifiable)null, null))
                                                                };

        public InModifiableClause ModifiableClause;
        public string CollectionName;
        public Type CollectionToModify;

        public InModifyExpressionNode(MethodCallExpressionParseInfo parseInfo, Expression collectionNameExpression)
            : base(parseInfo)
        {
            if (parseInfo.Source is MainSourceExpressionNode)
            {
                ModifiableClause = InModifiableClause.Main;
            }
            else
            {
                ModifiableClause = InModifiableClause.Body;
            }

            var constantExpression = collectionNameExpression as ConstantExpression;
            if (constantExpression != null)
            {
                var collectionName = constantExpression.Value as string;
                if (!string.IsNullOrEmpty(collectionName))
                {
                    CollectionName = collectionName;
                }
            }

            if (parseInfo.ParsedExpression.Type.GenericTypeArguments.Any())
            {
                CollectionToModify = parseInfo.ParsedExpression.Type.GenericTypeArguments[0];
            }
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

            if (ModifiableClause == InModifiableClause.Main)
            {
                if (!string.IsNullOrEmpty(CollectionName))
                {
                    queryModel.MainFromClause = new NamedCollectionMainFromClause(queryModel.MainFromClause, CollectionName);
                }
                queryModel.MainFromClause.ItemType = CollectionToModify;
            }
            else
            {
                var clauseToModify = queryModel.BodyClauses.FirstOrDefault(x => x is IModifyExpressionNode || x is AdditionalFromClause);
                var clauseIndex = queryModel.BodyClauses.IndexOf(clauseToModify);
                if (clauseToModify != null)
                {
                    if (clauseToModify is IModifyExpressionNode)
                    {
                        if (!string.IsNullOrEmpty(CollectionName))
                        {
                            if (clauseToModify is InsertAndReturnClause)
                            {
                                queryModel.BodyClauses[clauseIndex] = new NamedCollectionInsertAndReturnClause((InsertAndReturnClause)clauseToModify, CollectionName);
                            }
                            else if (clauseToModify is UpdateAndReturnClause)
                            {
                                queryModel.BodyClauses[clauseIndex] = new NamedCollectionUpdateAndReturnClause((UpdateAndReturnClause)clauseToModify, CollectionName);
                            }
                            else if (clauseToModify is RemoveAndReturnClause)
                            {
                                queryModel.BodyClauses[clauseIndex] = new NamedCollectionRemoveAndReturnClause((RemoveAndReturnClause)clauseToModify, CollectionName);
                            }
                        }

                        ((IModifyExpressionNode)queryModel.BodyClauses[clauseIndex]).CollectionType = CollectionToModify;
                    }
                    else if (clauseToModify is AdditionalFromClause)
                    {
                        if (!string.IsNullOrEmpty(CollectionName))
                        {
                            queryModel.BodyClauses[clauseIndex] = new NamedCollectionAdditionalFromClause((AdditionalFromClause)clauseToModify, CollectionName);
                        }

                        ((AdditionalFromClause)queryModel.BodyClauses[clauseIndex]).ItemType = CollectionToModify;
                    }
                }
            }

            return queryModel;
        }
    }
}
