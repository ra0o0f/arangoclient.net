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
                                                                   () => For<object, object> (null, o => null))
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

        /*crud extentions*/

        public static void ForEach<T>(this IAqlModifiable<T> source, Action<T> action)
        {
            ForEach<T>(source, x => action(x), IsNewResult(source));
        }

        public static void ForEach<T>(this IAqlModifiable<T> source, Action<T> action, bool returnNewResult)
        {
            var newSource = ReturnResult<T>(source, returnNewResult);
            newSource.AsAqlQueryable<T>().ForEach(x => action(x));
        }

        public static async Task<List<T>> ToListAsync<T>(this IAqlModifiable<T> source)
        {
            return await ToListAsync<T>(source, IsNewResult(source));
        }

        public static async Task<List<T>> ToListAsync<T>(this IAqlModifiable<T> source, bool returnNewResult)
        {
            var newSource = ReturnResult<T>(source, returnNewResult);
            return await source.AsAqlQueryable<T>().ToListAsync().ConfigureAwait(false);
        }

        public static async Task ForEachAsync<T>(this IAqlModifiable<T> source, Action<T> action)
        {
            await ForEachAsync<T>(source, action, IsNewResult(source)).ConfigureAwait(false);
        }

        public static async Task ForEachAsync<T>(this IAqlModifiable<T> source, Action<T> action, bool returnNewResult)
        {
            var newSource = ReturnResult<T>(source, returnNewResult);
            await newSource.AsAqlQueryable<T>().ForEachAsync(x => action(x)).ConfigureAwait(false);
        }

        public static List<T> ToList<T>(this IAqlModifiable<T> source)
        {
            return ToList(source, IsNewResult(source));
        }

        public static List<T> ToList<T>(this IAqlModifiable<T> source,bool returnNewResult)
        {
            var newSource = ReturnResult<T>(source, returnNewResult);
            return newSource.AsAqlQueryable<T>().ToList();
        }

        private static bool IsNewResult<T>(IAqlModifiable<T> source)
        {
            switch (source.AsAqlQueryable().StateValues["CrudFunction"])
            {
                case "insert":
                case "update":
                case "replace":
                    return true;
                case "remove":
                    return false;
                default:
                    return false;
            }
        }

        public static void Execute<T>(this IAqlModifiable<T> source)
        {
            source.AsAqlQueryable<T>().ToList();
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

        private static MethodInfo FindCachedDefaultMethod(string name, params Type[] arguments)
        {
            return FindCachedDefaultMethod(name, "default", arguments);
        }

        private static MethodInfo FindCachedDefaultMethod(string name, string defaultMethod, params Type[] arguments)
        {
            string key = name + "-" + string.Join("-", arguments.Select(x => x.Name).ToList());
            return _cachedMethodInfos.GetOrAdd(key,
                typeof(QueryableExtensions).GetRuntimeMethods().ToList()
                .Where(x => 
                    x.GetCustomAttribute<DefaultExtentionAttribute>() != null 
                    && x.GetCustomAttribute<DefaultExtentionAttribute>().Name == defaultMethod
                    && x.Name == name)
                .First().MakeGenericMethod(arguments));
        }

        public static IQueryable<TResult> For<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, IEnumerable<TResult>>> selector)
        {
            return source.Provider.CreateQuery<TResult>(
                Expression.Call(
                    FindCachedMethod("For", typeof(TSource), typeof(TResult)),
                    source.Expression,
                    Expression.Quote(selector)
                    ));
        }

        public static IQueryable<IGrouping<TKey, TSource>> Collect<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            return source.Provider.CreateQuery<IGrouping<TKey, TSource>>(
                Expression.Call(
                    FindCachedMethod("Collect", typeof(TSource), typeof(TKey)),
                    source.Expression,
                    Expression.Quote(keySelector)));
        }

        [DefaultExtention]
        public static IQueryable<TSource> Limit<TSource>(this IQueryable<TSource> source,int offset, int count)
        {
            return source.Provider.CreateQuery<TSource>(
                Expression.Call(
                    // 3 parameter for providing 0 for offset
                    FindCachedMethod("Limit", typeof(TSource)),
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

        internal static IAqlModifiable<TSource> UpdateReplace<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, object>> withSelector
            , Expression<Func<TSource, object>> keySelector,string command)
        {

            if (withSelector.Body.NodeType != ExpressionType.MemberInit &&
                withSelector.Body.NodeType != ExpressionType.New)
            {
                throw new InvalidOperationException(string.Format(@"IQueryable.{0}() 'withSelector' object argument should be initialize within the function:
             for example use a defined type
             db.Query<SomeClass>.Update( x => new SomeClass { SomeCounter = x.SomeCounter + 1 }
             or an anonymous type
             db.Query<SomeClass>.Update( x => new { SomeCounter = x.SomeCounter + 1 }
            ",command == "replace" ? "Replace()" : "Update()"));
            }

            if (keySelector == null)
                keySelector = x => null;

            source.AsAqlQueryable().StateValues["CrudFunction"] = command;

            return source.Provider.CreateQuery<TSource>(
                Expression.Call(
                    FindCachedMethod("UpdateReplace",typeof(TSource)),
                    source.Expression,
                    Expression.Quote(withSelector),
                    Expression.Quote(keySelector),
                    Expression.Constant(command)
                    )).AsAqlQueryable().KeepState(source as IQueryableState) as IAqlModifiable<TSource>;
        }

        public static IAqlModifiable<TSource> Insert<TSource>(this IQueryable<TSource> source)
        {
            return Insert(source, null);
        }

        public static IAqlModifiable<TSource> Insert<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, object>> selector)
        {
            return Insert(source, selector,typeof(TSource));
        }

        [DefaultExtention]
        internal static IAqlModifiable<TSource> Insert<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, object>> selector, Type type)
        {
            if (selector!=null && selector.Body.NodeType != ExpressionType.MemberInit &&
                selector.Body.NodeType != ExpressionType.New)
                throw new InvalidOperationException(@"IQueryable.Insert() 'selector' object argument should be initialize within the function:
             for example use a defined type
             db.Query<SomeClass>.Update( x => new SomeClass { SomeCounter = x.SomeCounter + 1 }
             or an anonymous type
             db.Query<SomeClass>.Update( x => new { SomeCounter = x.SomeCounter + 1 }
            ");

            if (selector == null)
                selector = x => null;

            source.AsAqlQueryable().StateValues["CrudFunction"] = "insert";

            return source.Provider.CreateQuery<TSource>(
                Expression.Call(
                    FindCachedDefaultMethod("Insert", typeof(TSource)),
                    source.Expression,
                    Expression.Quote(selector),
                    Expression.Constant(type)
                    )).AsAqlQueryable().KeepState(source as IQueryableState) as IAqlModifiable<TSource>;
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

        [DefaultExtention]
        internal static IAqlModifiable<TSource> Remove<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, object>> keySelector,Type type)
        {
            if (keySelector == null)
                keySelector = x => null;

            source.AsAqlQueryable().StateValues["CrudFunction"] = "remove";

            return  source.Provider.CreateQuery<TSource>(
                Expression.Call(
                    FindCachedDefaultMethod("Remove", typeof(TSource)),
                    source.Expression,
                    Expression.Quote(keySelector),
                    Expression.Constant(type)
                    )).AsAqlQueryable().KeepState(source as IQueryableState) as IAqlModifiable<TSource>;
        }

        public static IAqlModifiable<TResult> In<TResult>(this IAqlModifiable source)
        {
            return source.Provider.CreateQuery<TResult>(
                Expression.Call(
                    FindCachedMethod("In", typeof(TResult)),
                    source.Expression
                    )).AsAqlQueryable().KeepState(source as IQueryableState) as IAqlModifiable<TResult>;
        }

        internal static IAqlModifiable<TResult> ReturnResult<TResult>(this IAqlModifiable<TResult> source,bool returnNewResult)
        {
            return source.Provider.CreateQuery<TResult>(
                Expression.Call(
                    FindCachedMethod("ReturnResult", typeof(TResult)),
                    source.Expression,
                    Expression.Constant(returnNewResult)
                    )).AsAqlQueryable().KeepState(source as IQueryableState) as IAqlModifiable<TResult>;
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

        [DefaultExtention(Name = "Let1")]
        public static IQueryable<TResult> Let<TSource, TLet, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TLet>> letSelector, Expression<Func<IQueryable<TSource>, TLet, IQueryable<TResult>>> querySelector)
        {
            return source.Provider.CreateQuery<TResult>(
                Expression.Call(
                    FindCachedDefaultMethod("Let", "Let1", typeof(TSource), typeof(TLet), typeof(TResult)),
                    source.Expression,
                    Expression.Quote(letSelector),
                    Expression.Quote(querySelector)));
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
