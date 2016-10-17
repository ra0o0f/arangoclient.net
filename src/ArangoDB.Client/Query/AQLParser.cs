using ArangoDB.Client.Query.Clause;
using Remotion.Linq;
using Remotion.Linq.Parsing.ExpressionVisitors.Transformation;
using Remotion.Linq.Parsing.Structure;
using Remotion.Linq.Parsing.Structure.NodeTypeProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Query
{
    public class AQLParser
    {
        internal static readonly MethodInfo[] OrderBySupportedMethods = new[]
                                                           {
                                                               LinqUtility.GetSupportedMethod (() => Queryable.OrderBy<object, object> (null, null)),
                                                               LinqUtility.GetSupportedMethod (() => Enumerable.OrderBy<object, object> (null, null)),
                                                               LinqUtility.GetSupportedMethod (() => QueryableExtensions.Sort<object, object> (null, null))
                                                           };

        internal static readonly MethodInfo[] OrderByDescendingSupportedMethods = new[]
                                                           {
                                                               LinqUtility.GetSupportedMethod (() => Queryable.OrderByDescending<object, object> (null, null)),
                                                               LinqUtility.GetSupportedMethod (() => Enumerable.OrderByDescending<object, object> (null, null)),
                                                               LinqUtility.GetSupportedMethod (() => QueryableExtensions.SortDescending<object, object> (null, null))
                                                           };

        public static readonly MethodInfo[] SelectManySupportedMethods = new[]
                                                           {
                                                               LinqUtility.GetSupportedMethod (
                                                                   () => Queryable.SelectMany<object, object[], object> (null, o => null, null)),
                                                               LinqUtility.GetSupportedMethod (
                                                                   () => Enumerable.SelectMany<object, object[], object> (null, o => null, null)),
                                                               LinqUtility.GetSupportedMethod (
                                                                   () => Queryable.SelectMany<object, object[]> (null, o => null)),
                                                               LinqUtility.GetSupportedMethod (
                                                                   () => Enumerable.SelectMany<object, object[]> (null, o => null)),
                                                               LinqUtility.GetSupportedMethod (
                                                                   () => QueryableExtensions.For<object, object> (null, o => null))
                                                           };

        IArangoDatabase db;

        public AQLParser(IArangoDatabase db)
        {
            this.db = db;
        }

        private IQueryParser CreateQueryParser()
        {
            var customNodeTypeRegistry = new MethodInfoBasedNodeTypeRegistry();

            //customNodeTypeRegistry.Register(new[] { typeof(EntitySet<>).GetMethod("Contains") }, typeof(ContainsExpressionNode));
            customNodeTypeRegistry.Register(GroupByExpressionNode.GetSupportedMethods, typeof(GroupByExpressionNode));
            customNodeTypeRegistry.Register(FilterExpressionNode.GetSupportedMethods, typeof(FilterExpressionNode));
            customNodeTypeRegistry.Register(LetSelectExpressionNode.SupportedMethods, typeof(LetSelectExpressionNode));
            customNodeTypeRegistry.Register(LetLambdaExpressionNode.SupportedMethods, typeof(LetLambdaExpressionNode));
            customNodeTypeRegistry.Register(TakeExpressionNode.SupportedMethods, typeof(TakeExpressionNode));
            customNodeTypeRegistry.Register(SkipExpressionNode.SupportedMethods, typeof(SkipExpressionNode));
            customNodeTypeRegistry.Register(OrderBySupportedMethods, typeof(Remotion.Linq.Parsing.Structure.IntermediateModel.OrderByExpressionNode));
            customNodeTypeRegistry.Register(OrderByDescendingSupportedMethods, typeof(Remotion.Linq.Parsing.Structure.IntermediateModel.OrderByDescendingExpressionNode));
            customNodeTypeRegistry.Register(SelectManySupportedMethods, typeof(Remotion.Linq.Parsing.Structure.IntermediateModel.SelectManyExpressionNode));
            customNodeTypeRegistry.Register(RemoveExpressionNode.SupportedMethods, typeof(RemoveExpressionNode));
            customNodeTypeRegistry.Register(InsertExpressionNode.SupportedMethods, typeof(InsertExpressionNode));
            customNodeTypeRegistry.Register(UpdateReplaceExpressionNode.SupportedMethods, typeof(UpdateReplaceExpressionNode));
            customNodeTypeRegistry.Register(UpsertExpressionNode.SupportedMethods, typeof(UpsertExpressionNode));
            customNodeTypeRegistry.Register(SelectModificationExpressionNode.SupportedMethods, typeof(SelectModificationExpressionNode));
            customNodeTypeRegistry.Register(InModificationExpressionNode.SupportedMethods, typeof(InModificationExpressionNode));
            customNodeTypeRegistry.Register(IgnoreModificationSelectExpressionNode.SupportedMethods, typeof(IgnoreModificationSelectExpressionNode));
            customNodeTypeRegistry.Register(TraversalExpressionNode.SupportedMethods, typeof(TraversalExpressionNode));
            customNodeTypeRegistry.Register(TraversalDepthExpressionNode.SupportedMethods, typeof(TraversalDepthExpressionNode));
            customNodeTypeRegistry.Register(TraversalDirectionExpressionNode.SupportedMethods, typeof(TraversalDirectionExpressionNode));
            customNodeTypeRegistry.Register(TraversalGraphNameExpressionNode.SupportedMethods, typeof(TraversalGraphNameExpressionNode));
            customNodeTypeRegistry.Register(TraversalEdgeExpressionNode.SupportedMethods, typeof(TraversalEdgeExpressionNode));
            customNodeTypeRegistry.Register(TraversalOptionsExpressionNode.SupportedMethods, typeof(TraversalOptionsExpressionNode));
            customNodeTypeRegistry.Register(ShortestPathExpressionNode.SupportedMethods, typeof(ShortestPathExpressionNode));

            var nodeTypeProvider = ExpressionTreeParser.CreateDefaultNodeTypeProvider();
            nodeTypeProvider.InnerProviders.Insert(0, customNodeTypeRegistry);

            var transformerRegistry = ExpressionTransformerRegistry.CreateDefault();
            var processor = ExpressionTreeParser.CreateDefaultProcessor(transformerRegistry);
            var expressionTreeParser = new ExpressionTreeParser(nodeTypeProvider, processor);
            return new QueryParser(expressionTreeParser);
        }
        
        private IQueryExecutor CreateQueryExecuter()
        {
            return new ArangoQueryExecuter(db);
        }

        public ArangoQueryable<T> CreateQueryable<T>()
        {
            var queryParser = CreateQueryParser();
            var queryExecuter = CreateQueryExecuter();

            return new ArangoQueryable<T>(queryParser, queryExecuter, db);
        }
    }
}
