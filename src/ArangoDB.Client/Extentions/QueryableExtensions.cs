using ArangoDB.Client.Data;
using ArangoDB.Client.Query;
using ArangoDB.Client.Query.Clause;
using ArangoDB.Client.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{

    public static class QueryableExtensions
    {
        static QueryableExtensions()
        {
            var extention = typeof(QueryableExtensions).GetRuntimeMethods()
                .Union(typeof(TraversalQueryableExtensions).GetRuntimeMethods())
                .Union(typeof(ShortestPathQueryableExtensions).GetRuntimeMethods())
                .Where(x => x.GetCustomAttribute<ExtentionIdentifierAttribute>() != null)
                .GroupBy(x => x.GetCustomAttribute<ExtentionIdentifierAttribute>().Identifier)
                .Select(g => new { g.Key, Count = g.Count() })
                .Where(x => x.Count > 1)
                .FirstOrDefault();

            if (extention != null)
                throw new InvalidOperationException($"Multiple extention identifier {extention.Key} found");
        }

        public static ArangoQueryable<T> AsArangoQueryable<T>(this IQueryable<T> source)
        {
            if (source == null)
                throw new InvalidCastException("Queryable source is null");

            var queryable = (source) as ArangoQueryable<T>;

            if (queryable == null)
                throw new InvalidCastException("Queryable source is not type of ArangoQueryable");

            return queryable;
        }

        public static QueryData GetQueryData<T>(this IQueryable<T> source)
        {
            return source.AsArangoQueryable().GetQueryData();
        }

        public static Task<TSource> FirstAsync<TSource>(this IQueryable<TSource> source)
        {
            return FirstOrDefaultAsync(source, false, null);
        }

        public static Task<TSource> FirstAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            return FirstOrDefaultAsync(source, false, predicate);
        }

        public static Task<TSource> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source)
        {
            return FirstOrDefaultAsync(source, true, null);
        }

        public static Task<TSource> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            return FirstOrDefaultAsync(source, true, predicate);
        }

        private static Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> source, bool returnDefaultWhenEmpty, Expression<Func<T, bool>> predicate)
        {
            if (predicate != null)
                source = source.Where(predicate);
            var cursor = source.Take(1).AsCursor() as Cursor<T>;
            return cursor.ExecuteScalar(returnDefaultWhenEmpty: returnDefaultWhenEmpty);
        }

        public static Task<TSource> SingleAsync<TSource>(this IQueryable<TSource> source)
        {
            return SingleOrDefaultAsync(source, false, null);
        }

        public static Task<TSource> SingleAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            return SingleOrDefaultAsync(source, false, predicate);
        }

        public static Task<TSource> SingleOrDefaultAsync<TSource>(this IQueryable<TSource> source)
        {
            return SingleOrDefaultAsync(source, true, null);
        }

        public static Task<TSource> SingleOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            return SingleOrDefaultAsync(source, true, predicate);
        }

        private static Task<T> SingleOrDefaultAsync<T>(this IQueryable<T> source, bool returnDefaultWhenEmpty, Expression<Func<T, bool>> predicate)
        {
            if (predicate != null)
                source = source.Where(predicate);

            var cursor = source.AsCursor() as Cursor<T>;
            return cursor.ExecuteScalar(returnDefaultWhenEmpty: returnDefaultWhenEmpty, throwIfNotSingle: true);
        }

        public static Task<List<T>> ToListAsync<T>(this IQueryable<T> source)
        {
            return source.AsArangoQueryable<T>().ToListAsync();
        }

        public static void ForEach<T>(this IQueryable<T> source, Action<T> action)
        {
            source.AsArangoQueryable<T>().ForEach(x => action(x));
        }

        public static async Task ForEachAsync<T>(this IQueryable<T> source, Action<T> action)
        {
            await source.AsArangoQueryable<T>().ForEachAsync(x => action(x)).ConfigureAwait(false);
        }

        public static ICursor<T> AsCursor<T>(this IQueryable<T> source)
        {
            return source.AsArangoQueryable<T>().AsCursor();
        }

        public static void Execute<T>(this IAqlModifiable<T> source)
        {
            // AsCursor is needed for executing query by ArangoQueryable methods instead of ArangoQueryExecuter
            source.IgnoreModificationSelect().AsCursor().ToList();
        }

        /*Relinq Extentions*/

        private static ConcurrentDictionary<string, MethodInfo> cachedExtentions = new ConcurrentDictionary<string, MethodInfo>();

        internal static MethodInfo FindExtention(string identifier, params Type[] arguments)
        {
            string key = $"{identifier}_{string.Join("_", arguments.Select(x => x.FullName))}";

            return cachedExtentions
                .GetOrAdd(key,
                typeof(QueryableExtensions).GetRuntimeMethods()
                .Union(typeof(TraversalQueryableExtensions).GetRuntimeMethods())
                .Union(typeof(ShortestPathQueryableExtensions).GetRuntimeMethods())
                .ToList()
                .First(x => x.GetCustomAttribute<ExtentionIdentifierAttribute>()?.Identifier == identifier)
                .MakeGenericMethod(arguments));
        }

        [ExtentionIdentifier("For")]
        public static IQueryable<TResult> For<TSource, TResult>(this IEnumerable<TSource> source, Expression<Func<TSource, IEnumerable<TResult>>> selector)
        {
            var queryableSource = source.AsQueryable();

            return queryableSource.Provider.CreateQuery<TResult>(
                Expression.Call(
                    FindExtention("For", typeof(TSource), typeof(TResult)),
                    queryableSource.Expression,
                    Expression.Quote(selector)
                    ));
        }

        [ExtentionIdentifier("Collect")]
        public static IQueryable<IGrouping<TKey, TSource>> Collect<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            return source.Provider.CreateQuery<IGrouping<TKey, TSource>>(
                Expression.Call(
                    FindExtention("Collect", typeof(TSource), typeof(TKey)),
                    source.Expression,
                    Expression.Quote(keySelector)));
        }

        [ExtentionIdentifier("Limit")]
        public static IQueryable<TSource> Limit<TSource>(this IQueryable<TSource> source, int offset, int count)
        {
            return source.Provider.CreateQuery<TSource>(
                Expression.Call(
                    FindExtention("Limit", typeof(TSource)),
                    source.Expression,
                    Expression.Constant(count),
                    Expression.Constant(offset)));
        }

        public static IQueryable<TSource> Limit<TSource>(this IQueryable<TSource> source, int count)
        {
            return Limit<TSource>(source, 0, count);
        }

        public static IAqlModifiable<TSource> Update<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, object>> withSelector)
        {
            return Update<TSource>(source, withSelector, null);
        }

        public static IAqlModifiable<TSource> Update<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, object>> withSelector
            , Expression<Func<TSource, object>> keySelector)
        {
            return UpdateReplace(source, withSelector, keySelector, "update");
        }

        public static IAqlModifiable<TSource> Replace<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, object>> withSelector)
        {
            return Replace<TSource>(source, withSelector, null);
        }

        public static IAqlModifiable<TSource> Replace<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, object>> withSelector
            , Expression<Func<TSource, object>> keySelector)
        {
            return UpdateReplace(source, withSelector, keySelector, "replace");
        }

        [ExtentionIdentifier("UpdateReplace")]
        internal static IAqlModifiable<TSource> UpdateReplace<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, object>> withSelector
            , Expression<Func<TSource, object>> keySelector, string command)
        {
            if (keySelector == null)
                keySelector = x => null;

            return source.Provider.CreateQuery<TSource>(
                Expression.Call(
                    FindExtention("UpdateReplace", typeof(TSource)),
                    source.Expression,
                    Expression.Quote(withSelector),
                    Expression.Quote(keySelector),
                    Expression.Constant(command)
                    )) as IAqlModifiable<TSource>;
        }

        public static IAqlModifiable<AQL> Upsert<TSource>(this IQueryable source, Expression<Func<AQL, object>> searchExpression,
            Expression<Func<AQL, object>> insertExpression, Expression<Func<AQL, TSource, object>> updateExpression)
        {
            return InternalUpsert(source.OfType<AQL>(), searchExpression, insertExpression, updateExpression, typeof(TSource));
        }

        public static IAqlModifiable<TSource> Upsert<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, object>> searchExpression,
            Expression<Func<TSource, object>> insertExpression, Expression<Func<TSource, TSource, object>> updateExpression)
        {
            return InternalUpsert(source, searchExpression, insertExpression, updateExpression, typeof(TSource));
        }

        [ExtentionIdentifier("InternalUpsert")]
        internal static IAqlModifiable<TSource> InternalUpsert<TSource, TOld>(this IQueryable<TSource> source, Expression<Func<TSource, object>> searchExpression,
            Expression<Func<TSource, object>> insertExpression, Expression<Func<TSource, TOld, object>> updateExpression, Type updateType)
        {
            var newUpdateExp = ExpressionParameterRewriter.RewriteParameterAt(updateExpression, 1, "OLD");

            return source.Provider.CreateQuery<TSource>(
                Expression.Call(
                    FindExtention("InternalUpsert", typeof(TSource), typeof(TOld)),
                    source.Expression,
                    Expression.Quote(searchExpression),
                    Expression.Quote(insertExpression),
                    Expression.Quote(newUpdateExp),
                    Expression.Constant(updateType)
                    )) as IAqlModifiable<TSource>;
        }

        public static IAqlModifiable<TSource> Insert<TSource>(this IQueryable<TSource> source)
        {
            return Insert(source, null);
        }

        public static IAqlModifiable<TSource> Insert<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, object>> selector)
        {
            return Insert(source, selector, typeof(TSource));
        }

        [ExtentionIdentifier("Insert")]
        internal static IAqlModifiable<TSource> Insert<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, object>> selector, Type type)
        {
            if (selector == null)
                selector = x => null;

            return source.Provider.CreateQuery<TSource>(
                Expression.Call(
                    FindExtention("Insert", typeof(TSource)),
                    source.Expression,
                    Expression.Quote(selector),
                    Expression.Constant(type)
                    )) as IAqlModifiable<TSource>;
        }

        public static IAqlModifiable<TSource> Remove<TSource>(this IQueryable<TSource> source)
        {
            return Remove(source, null);
        }

        public static IAqlModifiable<TSource> Remove<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, object>> keySelector)
        {
            return Remove(source, keySelector, typeof(TSource));
        }

        [ExtentionIdentifier("Remove")]
        internal static IAqlModifiable<TSource> Remove<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, object>> keySelector, Type type)
        {
            if (keySelector == null)
                keySelector = x => null;

            return source.Provider.CreateQuery<TSource>(
                Expression.Call(
                    FindExtention("Remove", typeof(TSource)),
                    source.Expression,
                    Expression.Quote(keySelector),
                    Expression.Constant(type)
                    )) as IAqlModifiable<TSource>;
        }

        [ExtentionIdentifier("In")]
        public static IAqlModifiable<TResult> In<TResult>(this IAqlModifiable source)
        {
            return source.Provider.CreateQuery<TResult>(
                Expression.Call(
                    FindExtention("In", typeof(TResult)),
                    source.Expression
                    )) as IAqlModifiable<TResult>;
        }

        [ExtentionIdentifier("SelectModification")]
        public static IQueryable<TResult> Select<TSource, TResult>(this IAqlModifiable<TSource> source, Expression<Func<TSource, TSource, TResult>> selector)
        {
            var newSelector = ExpressionParameterRewriter.RewriteParameters(selector, "NEW", "OLD");

            return source.Provider.CreateQuery<TResult>(
                Expression.Call(
                    FindExtention("SelectModification", typeof(TSource), typeof(TResult)),
                    source.Expression,
                    Expression.Quote(newSelector)));
        }

        [ExtentionIdentifier("IgnoreModificationSelect")]
        internal static IQueryable<TResult> IgnoreModificationSelect<TResult>(this IAqlModifiable<TResult> source)
        {
            return source.Provider.CreateQuery<TResult>(
                Expression.Call(
                    FindExtention("IgnoreModificationSelect", typeof(TResult)),
                    source.Expression));
        }

        [ExtentionIdentifier("Return")]
        public static IQueryable<TResult> Return<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
        {
            return source.Provider.CreateQuery<TResult>(
                Expression.Call(
                    FindExtention("Return", typeof(TSource), typeof(TResult)),
                    source.Expression,
                    Expression.Quote(selector)));
        }

        [ExtentionIdentifier("Sort")]
        public static IOrderedQueryable<TSource> Sort<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            return (IOrderedQueryable<TSource>)source.Provider.CreateQuery<TSource>(
                Expression.Call(
                    FindExtention("Sort", typeof(TSource), typeof(TKey)),
                    source.Expression,
                    Expression.Quote(keySelector)));
        }

        public static IOrderedQueryable<TSource> SortDescending<TSource, TKey>(this IEnumerable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            return source.AsQueryable().SortDescending(keySelector);
        }

        [ExtentionIdentifier("SortDescending")]
        public static IOrderedQueryable<TSource> SortDescending<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            return (IOrderedQueryable<TSource>)source.Provider.CreateQuery<TSource>(
                Expression.Call(
                    FindExtention("SortDescending", typeof(TSource), typeof(TKey)),
                    source.Expression,
                    Expression.Quote(keySelector)));
        }

        [ExtentionIdentifier("Let")]
        public static IQueryable<TResult> Let<TSource, TLet, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TLet>> letSelector, Expression<Func<IQueryable<TSource>, TLet, IQueryable<TResult>>> querySelector)
        {
            return source.Provider.CreateQuery<TResult>(
                Expression.Call(
                    FindExtention("Let", typeof(TSource), typeof(TLet), typeof(TResult)),
                    source.Expression,
                    Expression.Quote(letSelector),
                    Expression.Quote(querySelector)));
        }

        [ExtentionIdentifier("Filter")]
        public static IQueryable<TSource> Filter<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            return source.Provider.CreateQuery<TSource>(
                Expression.Call(
                    FindExtention("Filter", typeof(TSource)),
                    source.Expression,
                    Expression.Quote(predicate)));
        }
    }

    public static class TraversalQueryableExtensions
    {
        [ExtentionIdentifier("Traversal_Selector")]
        public static ITraversalQueryable<TraversalData<TVertex, TEdge>> Traversal<TVertex, TEdge>(this IQueryable source, Expression<Func<string>> startVertex)
        {
            return source.Provider.CreateQuery<TraversalData<TVertex, TEdge>>(
                Expression.Call(
                    QueryableExtensions.FindExtention("Traversal_Selector", typeof(TVertex), typeof(TEdge)),
                    source.Expression,
                    Expression.Quote(startVertex))) as ITraversalQueryable<TraversalData<TVertex, TEdge>>;
        }

        [ExtentionIdentifier("Traversal_Constant")]
        public static ITraversalQueryable<TraversalData<TVertex, TEdge>> Traversal<TVertex, TEdge>(this IQueryable source, string startVertex)
        {
            return source.Provider.CreateQuery<TraversalData<TVertex, TEdge>>(
                Expression.Call(
                    QueryableExtensions.FindExtention("Traversal_Constant", typeof(TVertex), typeof(TEdge)),
                    source.Expression,
                    Expression.Constant(startVertex))) as ITraversalQueryable<TraversalData<TVertex, TEdge>>;
        }

        [ExtentionIdentifier("Traversal_Graph")]
        public static ITraversalQueryable<TraversalData<TVertex, TEdge>> Graph<TVertex, TEdge>(this ITraversalQueryable<TraversalData<TVertex, TEdge>> source, string graphName)
        {
            return source.Provider.CreateQuery<TraversalData<TVertex, TEdge>>(
                Expression.Call(
                    QueryableExtensions.FindExtention("Traversal_Graph", typeof(TVertex), typeof(TEdge)),
                    source.Expression,
                    Expression.Constant(graphName)
                    )) as ITraversalQueryable<TraversalData<TVertex, TEdge>>;
        }

        [ExtentionIdentifier("Traversal_Edge_Direction")]
        public static ITraversalQueryable<TraversalData<TVertex, TEdge>> Edge<TVertex, TEdge>(this ITraversalQueryable<TraversalData<TVertex, TEdge>> source, string collectionName, EdgeDirection direction)
        {
            return source.Provider.CreateQuery<TraversalData<TVertex, TEdge>>(
                Expression.Call(
                    QueryableExtensions.FindExtention("Traversal_Edge_Direction", typeof(TVertex), typeof(TEdge)),
                    source.Expression,
                    Expression.Constant(collectionName),
                    Expression.Constant(direction)
                    )) as ITraversalQueryable<TraversalData<TVertex, TEdge>>;
        }

        [ExtentionIdentifier("Traversal_Edge")]
        public static ITraversalQueryable<TraversalData<TVertex, TEdge>> Edge<TVertex, TEdge>(this ITraversalQueryable<TraversalData<TVertex, TEdge>> source, string collectionName)
        {
            return source.Provider.CreateQuery<TraversalData<TVertex, TEdge>>(
                Expression.Call(
                    QueryableExtensions.FindExtention("Traversal_Edge", typeof(TVertex), typeof(TEdge)),
                    source.Expression,
                    Expression.Constant(collectionName)
                    )) as ITraversalQueryable<TraversalData<TVertex, TEdge>>;
        }

        [ExtentionIdentifier("Traversal_Depth")]
        public static ITraversalQueryable<TraversalData<TVertex, TEdge>> Depth<TVertex, TEdge>(this ITraversalQueryable<TraversalData<TVertex, TEdge>> source, int min, int max)
        {
            return source.Provider.CreateQuery<TraversalData<TVertex, TEdge>>(
                Expression.Call(
                    QueryableExtensions.FindExtention("Traversal_Depth", typeof(TVertex), typeof(TEdge)),
                    source.Expression,
                    Expression.Constant(min),
                    Expression.Constant(max))) as ITraversalQueryable<TraversalData<TVertex, TEdge>>;
        }

        [ExtentionIdentifier("Traversal_InBound")]
        public static ITraversalQueryable<TraversalData<TVertex, TEdge>> InBound<TVertex, TEdge>(this ITraversalQueryable<TraversalData<TVertex, TEdge>> source)
        {
            return source.Provider.CreateQuery<TraversalData<TVertex, TEdge>>(
                Expression.Call(
                    QueryableExtensions.FindExtention("Traversal_InBound", typeof(TVertex), typeof(TEdge)),
                    source.Expression)) as ITraversalQueryable<TraversalData<TVertex, TEdge>>;
        }

        [ExtentionIdentifier("Traversal_OutBound")]
        public static ITraversalQueryable<TraversalData<TVertex, TEdge>> OutBound<TVertex, TEdge>(this ITraversalQueryable<TraversalData<TVertex, TEdge>> source)
        {
            return source.Provider.CreateQuery<TraversalData<TVertex, TEdge>>(
                Expression.Call(
                    QueryableExtensions.FindExtention("Traversal_OutBound", typeof(TVertex), typeof(TEdge)),
                    source.Expression)) as ITraversalQueryable<TraversalData<TVertex, TEdge>>;
        }

        [ExtentionIdentifier("Traversal_AnyDirection")]
        public static ITraversalQueryable<TraversalData<TVertex, TEdge>> AnyDirection<TVertex, TEdge>(this ITraversalQueryable<TraversalData<TVertex, TEdge>> source)
        {
            return source.Provider.CreateQuery<TraversalData<TVertex, TEdge>>(
                Expression.Call(
                    QueryableExtensions.FindExtention("Traversal_AnyDirection", typeof(TVertex), typeof(TEdge)),
                    source.Expression)) as ITraversalQueryable<TraversalData<TVertex, TEdge>>;
        }

        [ExtentionIdentifier("Traversal_Options")]
        public static ITraversalQueryable<TraversalData<TVertex, TEdge>> Options<TVertex, TEdge>(this ITraversalQueryable<TraversalData<TVertex, TEdge>> source, object options)
        {
            return source.Provider.CreateQuery<TraversalData<TVertex, TEdge>>(
                Expression.Call(
                    QueryableExtensions.FindExtention("Traversal_Options", typeof(TVertex), typeof(TEdge)),
                    source.Expression,
                    Expression.Constant(options))) as ITraversalQueryable<TraversalData<TVertex, TEdge>>;
        }
    }

    public static class ShortestPathQueryableExtensions
    {
        [ExtentionIdentifier("ShortestPath_Selector")]
        public static IShortestPathQueryable<ShortestPathData<TVertex, TEdge>> ShortestPath<TVertex, TEdge>(this IQueryable source, Expression<Func<string>> startVertex, Expression<Func<string>> targetVertex)
        {
            return source.Provider.CreateQuery<ShortestPathData<TVertex, TEdge>>(
                Expression.Call(
                    QueryableExtensions.FindExtention("ShortestPath_Selector", typeof(TVertex), typeof(TEdge)),
                    source.Expression,
                    Expression.Quote(startVertex),
                    Expression.Quote(targetVertex)
                    )) as IShortestPathQueryable<ShortestPathData<TVertex, TEdge>>;
        }

        [ExtentionIdentifier("ShortestPath_Constant")]
        public static IShortestPathQueryable<ShortestPathData<TVertex, TEdge>> ShortestPath<TVertex, TEdge>(this IQueryable source, string startVertex, string targetVertex)
        {
            return source.Provider.CreateQuery<ShortestPathData<TVertex, TEdge>>(
                Expression.Call(
                    QueryableExtensions.FindExtention("ShortestPath_Constant", typeof(TVertex), typeof(TEdge)),
                    source.Expression,
                    Expression.Constant(startVertex),
                    Expression.Constant(targetVertex)
                    )) as IShortestPathQueryable<ShortestPathData<TVertex, TEdge>>;
        }

        [ExtentionIdentifier("ShortestPath_Graph")]
        public static IShortestPathQueryable<ShortestPathData<TVertex, TEdge>> Graph<TVertex, TEdge>(this IShortestPathQueryable<ShortestPathData<TVertex, TEdge>> source, string graphName)
        {
            return source.Provider.CreateQuery<ShortestPathData<TVertex, TEdge>>(
                Expression.Call(
                    QueryableExtensions.FindExtention("ShortestPath_Graph", typeof(TVertex), typeof(TEdge)),
                    source.Expression,
                    Expression.Constant(graphName)
                    )) as IShortestPathQueryable<ShortestPathData<TVertex, TEdge>>;
        }

        [ExtentionIdentifier("ShortestPath_Edge")]
        public static IShortestPathQueryable<ShortestPathData<TVertex, TEdge>> Edge<TVertex, TEdge>(this IShortestPathQueryable<ShortestPathData<TVertex, TEdge>> source, string collectionName)
        {
            return source.Provider.CreateQuery<ShortestPathData<TVertex, TEdge>>(
               Expression.Call(
                   QueryableExtensions.FindExtention("ShortestPath_Edge", typeof(TVertex), typeof(TEdge)),
                   source.Expression,
                   Expression.Constant(collectionName)
                   )) as IShortestPathQueryable<ShortestPathData<TVertex, TEdge>>;
        }

        [ExtentionIdentifier("ShortestPath_Edge_Direction")]
        public static IShortestPathQueryable<ShortestPathData<TVertex, TEdge>> Edge<TVertex, TEdge>(this IShortestPathQueryable<ShortestPathData<TVertex, TEdge>> source, string collectionName, EdgeDirection direction)
        {
            return source.Provider.CreateQuery<ShortestPathData<TVertex, TEdge>>(
                Expression.Call(
                    QueryableExtensions.FindExtention("ShortestPath_Edge_Direction", typeof(TVertex), typeof(TEdge)),
                    source.Expression,
                    Expression.Constant(collectionName),
                    Expression.Constant(direction)
                    )) as IShortestPathQueryable<ShortestPathData<TVertex, TEdge>>;
        }

        [ExtentionIdentifier("ShortestPath_InBound")]
        public static IShortestPathQueryable<ShortestPathData<TVertex, TEdge>> InBound<TVertex, TEdge>(this IShortestPathQueryable<ShortestPathData<TVertex, TEdge>> source)
        {
            return source.Provider.CreateQuery<ShortestPathData<TVertex, TEdge>>(
                Expression.Call(
                    QueryableExtensions.FindExtention("ShortestPath_InBound", typeof(TVertex), typeof(TEdge)),
                    source.Expression)) as IShortestPathQueryable<ShortestPathData<TVertex, TEdge>>;
        }

        [ExtentionIdentifier("ShortestPath_OutBound")]
        public static IShortestPathQueryable<ShortestPathData<TVertex, TEdge>> OutBound<TVertex, TEdge>(this IShortestPathQueryable<ShortestPathData<TVertex, TEdge>> source)
        {
            return source.Provider.CreateQuery<ShortestPathData<TVertex, TEdge>>(
                Expression.Call(
                    QueryableExtensions.FindExtention("ShortestPath_OutBound", typeof(TVertex), typeof(TEdge)),
                    source.Expression)) as IShortestPathQueryable<ShortestPathData<TVertex, TEdge>>;
        }

        [ExtentionIdentifier("ShortestPath_AnyDirection")]
        public static IShortestPathQueryable<ShortestPathData<TVertex, TEdge>> AnyDirection<TVertex, TEdge>(this IShortestPathQueryable<ShortestPathData<TVertex, TEdge>> source)
        {
            return source.Provider.CreateQuery<ShortestPathData<TVertex, TEdge>>(
                Expression.Call(
                    QueryableExtensions.FindExtention("ShortestPath_AnyDirection", typeof(TVertex), typeof(TEdge)),
                    source.Expression)) as IShortestPathQueryable<ShortestPathData<TVertex, TEdge>>;
        }

        [ExtentionIdentifier("ShortestPath_Options")]
        public static IShortestPathQueryable<ShortestPathData<TVertex, TEdge>> Options<TVertex, TEdge>(this IShortestPathQueryable<ShortestPathData<TVertex, TEdge>> source, object options)
        {
            return source.Provider.CreateQuery<ShortestPathData<TVertex, TEdge>>(
                Expression.Call(
                    QueryableExtensions.FindExtention("ShortestPath_Options", typeof(TVertex), typeof(TEdge)),
                    source.Expression,
                    Expression.Constant(options)
                    )) as IShortestPathQueryable<ShortestPathData<TVertex, TEdge>>;
        }
    }
}
