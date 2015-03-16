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

        private static MethodInfo FindCachedMethod(string name,params Type[] arguments)
        {
            string key = name + "-" + string.Join("-", arguments.Select(x => x.Name).ToList());
            return _cachedMethodInfos.GetOrAdd(key,
                typeof(QueryableExtensions).GetRuntimeMethods().ToList()
                .Where(x => x.Name == name).First().MakeGenericMethod(arguments));
        }

        private static MethodInfo FindCachedMethod(string name, int argCount,int genericCount, params Type[] arguments)
        {
            string key = name + "-" + string.Join("-", arguments.Select(x => x.Name).ToList());
            return _cachedMethodInfos.GetOrAdd(key,
                typeof(QueryableExtensions).GetRuntimeMethods().ToList()
                .Where(x => x.Name == name && x.GetParameters().Count() == argCount && x.GetGenericArguments().Count()==genericCount).First().MakeGenericMethod(arguments));
        }

        public static IQueryable<TResult> For<TSource, TCollection, TResult>(this IQueryable<TSource> source, 
            Expression<Func<TSource, IEnumerable<TCollection>>> collectionSelector, Expression<Func<TSource, TCollection, TResult>> resultSelector)
        {
            return source.Provider.CreateQuery<TResult>(
                Expression.Call(
                    FindCachedMethod("For",typeof(TSource), typeof(TCollection),typeof(TResult)),
                    source.Expression,
                    Expression.Quote(collectionSelector),
                    Expression.Quote(resultSelector)));
        }

        public static IQueryable<IGrouping<TKey, TSource>> Collect<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            return source.Provider.CreateQuery<IGrouping<TKey, TSource>>(
                Expression.Call(
                    FindCachedMethod("Collect", typeof(TSource), typeof(TKey)),
                    source.Expression,
                    Expression.Quote(keySelector)));
        }

        public static IQueryable<TSource> Limit<TSource>(this IQueryable<TSource> source,int offset, int count)
        {
            return source.Provider.CreateQuery<TSource>(
                Expression.Call(
                    // 3 parameter for providing 0 for offset
                    FindCachedMethod("Limit", 3,0, typeof(TSource)),
                    source.Expression,
                    Expression.Constant(count),
                    Expression.Constant(offset)));
        }

        public static IQueryable<TSource> Limit<TSource>(this IQueryable<TSource> source, int count)
        {
            return Limit<TSource>(source, 0, count);
        }

        public static IQueryable<TSource> Update<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, object>> withSelector, Expression<Func<TSource, object>> keySelector=null, bool? returnNewResult = null)
        { return Update<TSource, TSource>(source, withSelector, keySelector, returnNewResult); }

        public static IQueryable<TModified> Update<TSource, TModified>(this IQueryable<TSource> source, Expression<Func<TSource, object>> withSelector,
            Expression<Func<TSource, object>> keySelector = null, bool? returnNewResult = null)
        {
            bool _returnModifiedResult = returnNewResult.HasValue;
            bool _returnNewResult = returnNewResult.HasValue ? returnNewResult.Value : true;
            Type type = typeof(TModified);
            return Update<TSource, TModified>(source, withSelector, keySelector, _returnModifiedResult, _returnNewResult, typeof(TModified));
        }

        internal static IQueryable<TModified> Update<TSource, TModified>(this IQueryable<TSource> source, Expression<Func<TSource, object>> withSelector,
             Expression<Func<TSource, object>> keySelector, bool returnModifiedResult, bool returnNewResult, Type type)
        {
            if (withSelector.Body.NodeType != ExpressionType.MemberInit &&
                withSelector.Body.NodeType != ExpressionType.New)
                throw new InvalidOperationException(@"IQueryable.Update() 'withSelector' argument should be initialize within the function:
 for example use a defined type
 db.Query<SomeClass>.Update( x => new SomeClass { SomeCounter = x.SomeCounter + 1 }
 or an anonymous type
 db.Query<SomeClass>.Update( x => new { SomeCounter = x.SomeCounter + 1 }
");


            if (keySelector == null)
                keySelector = x => null;

            return source.Provider.CreateQuery<TModified>(
                Expression.Call(
                    FindCachedMethod("Update", 6, 2, typeof(TSource), typeof(TModified)),
                    source.Expression,
                    Expression.Quote(withSelector),
                    Expression.Quote(keySelector),
                    Expression.Constant(returnModifiedResult),
                    Expression.Constant(returnNewResult),
                    Expression.Constant(type)
                    ));
        }

        public static IQueryable<TResult> Return<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
        {
            return source.Provider.CreateQuery<TResult>(
                Expression.Call(
                    FindCachedMethod("Return", typeof(TSource), typeof(TResult)),
                    source.Expression,
                    Expression.Quote(selector)));
        }

        public static IOrderedQueryable<TSource> Sort<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            return (IOrderedQueryable<TSource>)source.Provider.CreateQuery<TSource>(
                Expression.Call(
                    FindCachedMethod("Sort", typeof(TSource), typeof(TKey)),
                    source.Expression,
                    Expression.Quote(keySelector)));
        }

        public static IOrderedQueryable<TSource> SortDescending<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            return (IOrderedQueryable<TSource>)source.Provider.CreateQuery<TSource>(
                Expression.Call(
                    FindCachedMethod("SortDescending", typeof(TSource), typeof(TKey)),
                    source.Expression,
                    Expression.Quote(keySelector)));
        }

        public static IQueryable<TResult> Let<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
        {
            return source.Provider.CreateQuery<TResult>(
                Expression.Call(
                    FindCachedMethod("Let", typeof(TSource), typeof(TResult)),
                    source.Expression,
                    Expression.Quote(selector)));
        }

        public static IQueryable<TSource> Filter<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            return source.Provider.CreateQuery<TSource>(
                Expression.Call(
                    FindCachedMethod("Filter", typeof(TSource)),
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
