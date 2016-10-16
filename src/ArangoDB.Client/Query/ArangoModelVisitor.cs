using ArangoDB.Client.Data;
using ArangoDB.Client.Query;
using ArangoDB.Client.Query.Clause;
using ArangoDB.Client.Utility;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Query
{
    public class ArangoModelVisitor : QueryModelVisitorBase
    {
        static Dictionary<Type, string> aggregateResultOperatorFunctions;

        public Dictionary<string, string> MemberNamesMapping = new Dictionary<string, string>();

        public IArangoDatabase Db;

        public StringBuilder QueryText { get; set; }

        public QueryData QueryData { get; set; }

        public QueryModel QueryModel { get; set; }

        public ArangoModelVisitor ParnetModelVisitor { get; set; }

        public int ParameterNameCounter { get; set; }

        public int GroupByNameCounter { get; set; }

        public bool DontReturn { get; set; }

        public bool IgnoreFromClause { get; set; }

        public string DefaultAssociatedIdentifier { get; set; }

        public ArangoModelVisitor(IArangoDatabase db)
        {
            QueryText = new StringBuilder();
            QueryData = new QueryData();
            //CrudState = new VisitorCrudState();

            this.Db = db;
        }

        static ArangoModelVisitor()
        {
            aggregateResultOperatorFunctions = new Dictionary<Type, string>
            {
                {typeof(CountResultOperator),"length"},
                {typeof(LongCountResultOperator),"length"},
                {typeof(SumResultOperator),"sum"},
                {typeof(MinResultOperator),"min"},
                {typeof(MaxResultOperator),"max"},
                {typeof(AverageResultOperator),"average"}
            };
        }

        public override void VisitQueryModel(QueryModel queryModel)
        {
            if (DefaultAssociatedIdentifier != null)
                queryModel.MainFromClause.ItemName = DefaultAssociatedIdentifier;

            QueryModel = queryModel;

            var resultOperator = queryModel.ResultOperators.Count != 0 ? queryModel.ResultOperators[0] : null;
            var aggregateFunction = resultOperator != null && aggregateResultOperatorFunctions.ContainsKey(resultOperator.GetType())
                ? aggregateResultOperatorFunctions[resultOperator.GetType()] : null;

            if (resultOperator is FirstResultOperator)
                queryModel.BodyClauses.Add(new SkipTakeClause(Expression.Constant(0), Expression.Constant(1)));

            if (aggregateFunction != null)
                QueryText.AppendFormat(" return {0} (( ", aggregateFunction);

            if (!IgnoreFromClause && queryModel.MainFromClause.ItemType != typeof(AQL))
                queryModel.MainFromClause.Accept(this, queryModel);

            VisitBodyClauses(queryModel.BodyClauses, queryModel);

            var modificationClause = queryModel.BodyClauses.NextBodyClause<IModificationClause>();

            if (modificationClause != null && modificationClause.IgnoreSelect)
                DontReturn = true;

            if (DontReturn == false)
                queryModel.SelectClause.Accept(this, queryModel);

            if (aggregateFunction != null)
                QueryText.Append(" )) ");
        }

        public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
        {
            if (resultOperator as GroupResultOperator != null)
                throw new NotSupportedException("GroupResultOperator");

            if (resultOperator as TakeResultOperator != null)
                throw new NotSupportedException("TakeResultOperator");

            base.VisitResultOperator(resultOperator, queryModel, index);
        }

        public override void VisitAdditionalFromClause(AdditionalFromClause fromClause, QueryModel queryModel, int index)
        {
            var subQuery = fromClause.FromExpression as SubQueryExpression;
            if (subQuery != null)
            {
                this.DontReturn = true;
                GetAqlExpression(subQuery, queryModel, handleJoin: true);
            }
            else if (fromClause.FromExpression.Type.Name == "ArangoQueryable`1")
            {
                string fromName = LinqUtility.ResolveCollectionName(this.Db, fromClause.ItemType);
                QueryText.AppendFormat(" for {0} in {1} ", LinqUtility.ResolvePropertyName(fromClause.ItemName), fromName);
            }
            else
            {
                QueryText.AppendFormat(" for {0} in ", LinqUtility.ResolvePropertyName(fromClause.ItemName));
                GetAqlExpression(fromClause.FromExpression, queryModel);
            }
        }

        public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
        {
            if (fromClause.FromExpression as SubQueryExpression != null)
                throw new Exception("MainFromClause.FromExpression cant be SubQueryExpression because group by is handle in itself clause");

            string fromName = LinqUtility.ResolveCollectionName(this.Db, fromClause.ItemType);

            //  .Select(g => g.Select(gList => gList.Age)) subquery select, changes the fromName to group name (C1 for example)
            if (fromClause.FromExpression.Type.Name == "IGrouping`2")
            {
                var groupByClause = LinqUtility.PriorGroupBy(ParnetModelVisitor);

                var parentMVisitor = LinqUtility.FindParentModelVisitor(this);
                //parentMVisitor.GroupByNameCounter++;

                fromName = groupByClause[0].TranslateIntoName();

                fromName = LinqUtility.ResolvePropertyName(fromName);
            }

            //  == "IGrouping`2" => .Select(g => g.Select(gList => gList.Age)) subquery select
            if (fromClause.FromExpression.NodeType == ExpressionType.Constant
                || fromClause.FromExpression.Type.Name == "IGrouping`2")
            {
                if (fromClause.FromExpression.Type.Name == "ArangoQueryable`1" || fromClause.FromExpression.Type.Name == "IGrouping`2")
                    QueryText.AppendFormat(" for {0} in {1} ", LinqUtility.ResolvePropertyName(fromClause.ItemName), fromName);
                else
                {
                    QueryText.AppendFormat(" for {0} in ", LinqUtility.ResolvePropertyName(fromClause.ItemName));
                    GetAqlExpression(fromClause.FromExpression, queryModel);
                }
            }
            else
            {
                QueryText.AppendFormat(" for {0} in ", LinqUtility.ResolvePropertyName(fromClause.ItemName));
                GetAqlExpression(fromClause.FromExpression, queryModel);
            }

            base.VisitMainFromClause(fromClause, queryModel);
        }

        public void VisitLetClause(LetClause letClause, QueryModel queryModel, Type lhsType)
        {
            QueryText.AppendFormat(" let {0} = ", LinqUtility.ResolvePropertyName(letClause.ItemName));
            GetAqlExpression(letClause.LetExpression, queryModel);

            var subQuery = letClause.SubqueryExpression as SubQueryExpression;
            if (subQuery != null)
            {
                GetAqlExpression(subQuery, queryModel, handleLet: true);
                this.DontReturn = true;
            }
        }

        public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
        {
            QueryText.Append(" return ");
            GetAqlExpression(selectClause.Selector, queryModel);
        }

        public void VisitUpsertClause(UpsertClause upsertClause, QueryModel queryModel)
        {
            QueryText.Append(" upsert ");
            GetAqlExpression(upsertClause.SearchSelector, queryModel);

            QueryText.Append(" insert ");
            GetAqlExpression(upsertClause.InsertSelector, queryModel);

            QueryText.Append(" update ");
            GetAqlExpression(upsertClause.UpdateSelector, queryModel);

            QueryText.AppendFormat(" in {0} ", LinqUtility.ResolveCollectionName(Db, upsertClause.CollectionType));
        }

        public void VisitUpdateReplaceClause(UpdateReplaceClause updateReplaceClause, QueryModel queryModel)
        {
            if (updateReplaceClause.KeySelector != null)
            {
                QueryText.AppendFormat(" {0} ", updateReplaceClause.Command);

                GetAqlExpression(updateReplaceClause.KeySelector, queryModel);

                QueryText.AppendFormat(" with ");
            }
            else
            {
                QueryText.AppendFormat(" {0} {1} with ", updateReplaceClause.Command, LinqUtility.ResolvePropertyName(updateReplaceClause.ItemName));
            }

            GetAqlExpression(updateReplaceClause.WithSelector, queryModel);

            QueryText.AppendFormat(" in {0} ", LinqUtility.ResolveCollectionName(Db, updateReplaceClause.CollectionType));
        }

        public void VisitInsertClause(InsertClause insertClause, QueryModel queryModel)
        {
            if (insertClause.WithSelector != null)
            {
                QueryText.Append(" insert ");

                GetAqlExpression(insertClause.WithSelector, queryModel);
            }
            else
            {
                QueryText.AppendFormat(" insert {0} ", LinqUtility.ResolvePropertyName(insertClause.ItemName));
            }

            QueryText.AppendFormat(" in {0} ", LinqUtility.ResolveCollectionName(Db, insertClause.CollectionType));
        }

        public void VisitRemoveClause(RemoveClause removeClause, QueryModel queryModel)
        {
            if (removeClause.KeySelector != null)
            {
                QueryText.Append(" remove ");

                GetAqlExpression(removeClause.KeySelector, queryModel);
            }
            else
            {
                QueryText.AppendFormat(" remove {0} ", LinqUtility.ResolvePropertyName(removeClause.ItemName));
            }

            QueryText.AppendFormat(" in {0} ", LinqUtility.ResolveCollectionName(Db, removeClause.CollectionType));
        }

        public void VisitFilterClause(FilterClause filterClause, QueryModel queryModel, int index)
        {
            QueryText.Append(" filter ");
            GetAqlExpression(filterClause.Predicate, queryModel);
        }

        public void VisitSkipTakeClause(SkipTakeClause skipTakeClause, QueryModel queryModel, int index)
        {
            if (skipTakeClause.TakeCount == null)
                throw new InvalidOperationException("in limit[skip & take functions] count[take function] should be specified");

            QueryText.AppendFormat("  limit ");
            if (skipTakeClause != null && skipTakeClause.SkipCount != null)
            {
                GetAqlExpression(skipTakeClause.SkipCount, queryModel);
                QueryText.AppendFormat("  , ");
            }
            GetAqlExpression(skipTakeClause.TakeCount, queryModel);
        }

        public void VisitTraversalClause(ITraversalClause traversalClause, QueryModel queryModel, int index)
        {
            string prefix = LinqUtility.MemberNameFromMap(traversalClause.Identifier, "graph", this);

            QueryText.AppendFormat(" for {0}, {1}, {2} in ",
                LinqUtility.ResolvePropertyName($"{prefix}_Vertex"),
                LinqUtility.ResolvePropertyName($"{prefix}_Edge"),
                LinqUtility.ResolvePropertyName($"{prefix}_Path"));

            if (traversalClause.Min != null && traversalClause.Max != null)
                QueryText.AppendFormat(" {0}..{1} ", traversalClause.Min.Value, traversalClause.Max);

            QueryText.AppendFormat(" {0} ", traversalClause.Direction != null
                ? traversalClause.Direction.Value
                : Utils.EdgeDirectionToString(EdgeDirection.Any));

            GetAqlExpression(traversalClause.StartVertex, queryModel);

            if (traversalClause.TraversalContext.Type == typeof(string))
            {
                QueryText.AppendFormat("  graph \"{0}\" ", traversalClause.TraversalContext.Value.ToString());
            }
            else if (traversalClause.TraversalContext.Type == typeof(string[]))
            {
                string[] collections = traversalClause.TraversalContext.Value as string[];
                QueryText.Append(string.Join(", ", collections.Select(c => LinqUtility.ResolvePropertyName(c))));
            }
            else if (traversalClause.TraversalContext.Type == typeof(TraversalEdgeDefinition))
            {
                var edgeDefinition = traversalClause.TraversalContext.Value as TraversalEdgeDefinition;
                StringBuilder edges = new StringBuilder();

                for (int i = 0; i < edgeDefinition.Edges.Count; i++)
                {
                    var e = edgeDefinition.Edges[i];

                    if (i != 0)
                        edges.Append(", ");

                    if (e.Direction.HasValue)
                        edges.AppendFormat("{0} ", Utils.EdgeDirectionToString(e.Direction.Value));

                    if (e.GetCollection().GetType() == typeof(string))
                        edges.Append(LinqUtility.ResolvePropertyName(e.GetCollection() as string));
                    else
                        edges.Append(LinqUtility.ResolveCollectionName(Db, e.GetCollection() as Type));
                }

                QueryText.Append(edges);
            }

        }

        public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
        {
            QueryText.Append(" filter ");
            GetAqlExpression(whereClause.Predicate, queryModel);
        }

        public void VisitGroupByClause(GroupByClause groupByClause, QueryModel queryModel, int index)
        {
            var parentMVisitor = LinqUtility.FindParentModelVisitor(this);
            parentMVisitor.GroupByNameCounter++;
            groupByClause.IntoName = "C" + parentMVisitor.GroupByNameCounter;
            groupByClause.FuncIntoName = Db.Setting.Linq.TranslateGroupByIntoName;
            groupByClause.FromParameterName = queryModel.MainFromClause.ItemName;

            QueryText.Append(" collect ");

            if (groupByClause.Selector.NodeType != ExpressionType.New)
            {
                groupByClause.CollectVariableName = "CV" + parentMVisitor.GroupByNameCounter;
                QueryText.AppendFormat(" {0} = ", LinqUtility.ResolvePropertyName(groupByClause.CollectVariableName));
            }

            GetAqlExpression(groupByClause.Selector, queryModel, true);

            QueryText.AppendFormat(" into {0} ", LinqUtility.ResolvePropertyName(groupByClause.TranslateIntoName()));

            groupByClause.Visited = true;
        }

        public override void VisitOrderByClause(OrderByClause orderByClause, QueryModel queryModel, int index)
        {
            QueryText.Append(" sort ");
            for (int i = 0; i < orderByClause.Orderings.Count; i++)
            {
                GetAqlExpression(orderByClause.Orderings[i].Expression, queryModel);
                QueryText.AppendFormat(" {0} {1} ",
                    orderByClause.Orderings[i].OrderingDirection == OrderingDirection.Asc ? "asc" : "desc",
                    i != orderByClause.Orderings.Count - 1 ? " , " : string.Empty);
            }

            base.VisitOrderByClause(orderByClause, queryModel, index);
        }

        public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, int index)
        {
            base.VisitJoinClause(joinClause, queryModel, index);
        }

        public override void VisitGroupJoinClause(GroupJoinClause groupJoinClause, QueryModel queryModel, int index)
        {

        }

        private void GetAqlExpression(Expression expression, QueryModel queryModel,
            bool? treatNewWithoutBracket = false, bool? handleJoin = false, bool? handleLet = false)
        {
            var visitor = new ArangoExpressionTreeVisitor(this);
            visitor.TreatNewWithoutBracket = treatNewWithoutBracket.Value;
            visitor.QueryModel = queryModel;
            visitor.HandleJoin = handleJoin.Value;
            visitor.HandleLet = handleLet.Value;
            visitor.Visit(expression);
        }

    }
}
