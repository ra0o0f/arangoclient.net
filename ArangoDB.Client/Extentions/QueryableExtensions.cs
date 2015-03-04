using ArangoDB.Client.Common.Remotion.Linq.Parsing.ExpressionTreeVisitors;
using ArangoDB.Client.Common.Remotion.Linq.Parsing.Structure.IntermediateModel;
using ArangoDB.Client.Common.Remotion.Linq.Parsing.Structure.NodeTypeProviders;
using ArangoDB.Client.Common.Remotion.Linq.Utilities;
using ArangoDB.Client.Data;
using ArangoDB.Client.Linq;
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
        internal static readonly MethodInfo[] OrderBySupportedMethods = new[]
                                                           {
                                                               GetSupportedMethod (() => Queryable.OrderBy<object, object> (null, null)),
                                                               GetSupportedMethod (() => Enumerable.OrderBy<object, object> (null, null)),
                                                               GetSupportedMethod (() => Sort<object, object> (null, null))
                                                           };

        internal static readonly MethodInfo[] OrderByDescendingSupportedMethods = new[]
                                                           {
                                                               GetSupportedMethod (() => Queryable.OrderByDescending<object, object> (null, null)),
                                                               GetSupportedMethod (() => Enumerable.OrderByDescending<object, object> (null, null)),
                                                               GetSupportedMethod (() => SortDescending<object, object> (null, null))
                                                           };

        public static readonly MethodInfo[] SelectManySupportedMethods = new[]
                                                           {
                                                               GetSupportedMethod (
                                                                   () => Queryable.SelectMany<object, object[], object> (null, o => null, null)),
                                                               GetSupportedMethod (
                                                                   () => Enumerable.SelectMany<object, object[], object> (null, o => null, null)),
                                                               GetSupportedMethod (
                                                                   () => Queryable.SelectMany<object, object[]> (null, o => null)),
                                                               GetSupportedMethod (
                                                                   () => Enumerable.SelectMany<object, object[]> (null, o => null)),
                                                               GetSupportedMethod (
                                                                   () => For<object, object[], object> (null, o => null, null))
                                                           };

        static MethodInfo GetSupportedMethod<T>(Expression<Func<T>> methodCall)
        {
            Utils.CheckNotNull("methodCall", methodCall);

            var method = ReflectionUtility.GetMethod(methodCall);
            return MethodInfoBasedNodeTypeRegistry.GetRegisterableMethodDefinition(method, throwOnAmbiguousMatch: true);
        }

        public static AqlQueryable<T> AsAqlQueryable<T>(this IQueryable<T> source)
        {
            var queryable = (source) as ArangoDB.Client.Linq.AqlQueryable<T>;

            if (queryable == null)
                throw new InvalidCastException("Queryable source is not type of AqlQueryable");

            return queryable;
        }

        public static QueryData GetQueryData<T>(this IQueryable<T> source)
        {
            return source.AsAqlQueryable<T>().GetQueryData();
        }

        public static Task<List<T>> ToListAsync<T>(this IQueryable<T> source)
        {
            return source.AsAqlQueryable<T>().ToListAsync();
        }

        public static void ForEach<T>(this IQueryable<T> source, Action<T> action)
        {
            source.AsAqlQueryable<T>().ForEach(x=>action(x));
        }

        public static async Task ForEachAsync<T>(this IQueryable<T> source, Action<T> action)
        {
            await source.AsAqlQueryable<T>().ForEachAsync(x => action(x)).ConfigureAwait(false);
        }

        public static ICursor<T> AsCursor<T>(this IQueryable<T> source)
        {
            return source.AsAqlQueryable<T>().AsCursor();
        }

        /*Relinq Extentions*/

        private static ConcurrentDictionary<string, MethodInfo> _cachedMethodInfos = new ConcurrentDictionary<string, MethodInfo>();

        public static IQueryable<TResult> For<TSource, TCollection, TResult>(this IQueryable<TSource> source, 
            Expression<Func<TSource, IEnumerable<TCollection>>> collectionSelector, Expression<Func<TSource, TCollection, TResult>> resultSelector)
        {
            MethodInfo methodInfo = _cachedMethodInfos.GetOrAdd("For",
                typeof(QueryableExtensions).GetRuntimeMethods().ToList()
                .Where(x => x.Name == "For").First().MakeGenericMethod(typeof(TSource), typeof(TCollection),typeof(TResult)));

            return source.Provider.CreateQuery<TResult>(
                Expression.Call(
                    methodInfo,
                    source.Expression,
                    Expression.Quote(collectionSelector),
                    Expression.Quote(resultSelector)));
        }

        public static IQueryable<IGrouping<TKey, TSource>> Collect<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            MethodInfo methodInfo = _cachedMethodInfos.GetOrAdd("Collect",
                typeof(QueryableExtensions).GetRuntimeMethods().ToList()
                .Where(x => x.Name == "Collect").First().MakeGenericMethod(typeof(TSource), typeof(TKey)));

            return source.Provider.CreateQuery<IGrouping<TKey, TSource>>(
                Expression.Call(
                    methodInfo,
                    source.Expression,
                    Expression.Quote(keySelector)));
        }

        public static IQueryable<TSource> Limit<TSource>(this IQueryable<TSource> source,int offset, int count)
        {
            MethodInfo methodInfo = _cachedMethodInfos.GetOrAdd("Limit",
                typeof(QueryableExtensions).GetRuntimeMethods().ToList()
                .Where(x => x.Name == "Limit" && x.GetParameters().Count() == 3).First().MakeGenericMethod(typeof(TSource)));

            return source.Provider.CreateQuery<TSource>(
                Expression.Call(
                    methodInfo,
                    source.Expression,
                    Expression.Constant(count),
                    Expression.Constant(offset)));
        }

        public static IQueryable<TSource> Limit<TSource>(this IQueryable<TSource> source, int count)
        {
            // 3 parameter for providing 0 for offset
            MethodInfo methodInfo = _cachedMethodInfos.GetOrAdd("Limit",
                typeof(QueryableExtensions).GetRuntimeMethods().ToList()
                .Where(x => x.Name == "Limit" && x.GetParameters().Count()==3).First().MakeGenericMethod(typeof(TSource)));

            return source.Provider.CreateQuery<TSource>(
                Expression.Call(
                    methodInfo,
                    source.Expression,
                    Expression.Constant(count),
                    Expression.Constant(0)));
        }

        public static IQueryable<TResult> Return<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
        {
            MethodInfo methodInfo = _cachedMethodInfos.GetOrAdd("Return",
                typeof(QueryableExtensions).GetRuntimeMethods().ToList()
                .Where(x => x.Name == "Return").First().MakeGenericMethod(typeof(TSource), typeof(TResult)));

            return source.Provider.CreateQuery<TResult>(
                Expression.Call(
                    methodInfo,
                    source.Expression,
                    Expression.Quote(selector)));
        }

        public static IOrderedQueryable<TSource> Sort<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            MethodInfo methodInfo = _cachedMethodInfos.GetOrAdd("Sort",
                typeof(QueryableExtensions).GetRuntimeMethods().ToList()
                .Where(x => x.Name == "Sort").First().MakeGenericMethod(typeof(TSource), typeof(TKey)));

            return (IOrderedQueryable<TSource>)source.Provider.CreateQuery<TSource>(
                Expression.Call(
                    methodInfo,
                    source.Expression,
                    Expression.Quote(keySelector)));
        }

        public static IOrderedQueryable<TSource> SortDescending<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            MethodInfo methodInfo = _cachedMethodInfos.GetOrAdd("SortDescending",
                typeof(QueryableExtensions).GetRuntimeMethods().ToList()
                .Where(x => x.Name == "SortDescending").First().MakeGenericMethod(typeof(TSource), typeof(TKey)));

            return (IOrderedQueryable<TSource>)source.Provider.CreateQuery<TSource>(
                Expression.Call(
                    methodInfo,
                    source.Expression,
                    Expression.Quote(keySelector)));
        }

        public static IQueryable<TResult> Let<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
        {
            MethodInfo methodInfo = _cachedMethodInfos.GetOrAdd("Let",
                typeof(QueryableExtensions).GetRuntimeMethods().ToList()
                .Where(x => x.Name == "Let").First().MakeGenericMethod(typeof(TSource), typeof(TResult)));

            return source.Provider.CreateQuery<TResult>(
                Expression.Call(
                    methodInfo,
                    source.Expression,
                    Expression.Quote(selector)));
        }

        public static IQueryable<TSource> Filter<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            MethodInfo methodInfo = _cachedMethodInfos.GetOrAdd("Filter",
                typeof(QueryableExtensions).GetRuntimeMethods().ToList()
                .Where(x => x.Name == "Filter").First().MakeGenericMethod(typeof(TSource)));
            
            return source.Provider.CreateQuery<TSource>(
                Expression.Call(
                    methodInfo,
                    source.Expression,
                    Expression.Quote(predicate)));
        }

        private static IQueryable<T> CreateQuery<T, TR>(
      IQueryable<T> source, Expression<Func<IQueryable<T>, TR>> expression)
        {
            var newQueryExpression = ReplacingExpressionTreeVisitor.Replace(
                expression.Parameters[0],
                source.Expression,
                expression.Body);
            return source.Provider.CreateQuery<T>(newQueryExpression);
        }
    }
}
