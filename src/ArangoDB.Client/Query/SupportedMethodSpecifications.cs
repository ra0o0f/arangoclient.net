using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Query
{
    public static class SupportedMethodSpecifications
    {
        private static readonly ReadOnlyCollection<MethodInfo> s_enumerableAndQueryableMethods =
        new ReadOnlyCollection<MethodInfo>(
            typeof(Enumerable).GetRuntimeMethods()
            .Concat(typeof(QueryableExtensions).GetRuntimeMethods())
            .Concat(typeof(Queryable).GetRuntimeMethods()).ToList());

        public static ReadOnlyCollection<MethodInfo> EnumerableAndQueryableMethods
        {
            get { return s_enumerableAndQueryableMethods; }
        }

        public static IEnumerable<MethodInfo> WhereNameMatches(this IEnumerable<MethodInfo> input, string name)
        {
            LinqUtility.CheckNotNull("input", input);
            LinqUtility.CheckNotNullOrEmpty("name", name);

            if (input.Count(mi => mi.Name == name) == 0)
                throw new InvalidOperationException($"No suitable queryable method found for {name}");

            return input.Where(mi => mi.Name == name);
        }

        public static IEnumerable<MethodInfo> WithoutIndexSelector(this IEnumerable<MethodInfo> input, int parameterPosition)
        {
            LinqUtility.CheckNotNull("input", input);

            return input.Where(mi => !HasIndexSelectorParameter(mi, parameterPosition));
        }

        private static bool HasIndexSelectorParameter(MethodInfo methodInfo, int parameterPosition)
        {
            var parameters = methodInfo.GetParameters();
            if (parameters.Length <= parameterPosition)
                return false;

            var selectorType = parameters[parameterPosition].ParameterType.GetTypeInfo().UnwrapEnumerable();

            return selectorType.GenericTypeArguments[1] == typeof(int);
        }

        private static TypeInfo UnwrapEnumerable(this TypeInfo typeInfo)
        {
            var comparedTypeInfo = typeInfo;

            while (comparedTypeInfo.ContainsGenericParameters)
                comparedTypeInfo = comparedTypeInfo.BaseType.GetTypeInfo();

            // Enumerable taks a Func<...> but Querable takes an Expression<Func<...>>
            if (typeof(Expression).GetTypeInfo().IsAssignableFrom(comparedTypeInfo))
                return typeInfo.GenericTypeArguments[0].GetTypeInfo();

            return typeInfo;
        }

        public static IEnumerable<MethodInfo> WithoutResultSelector(this IEnumerable<MethodInfo> input)
        {
            LinqUtility.CheckNotNull("input", input);

            return input.Where(mi => mi.GetParameters().All(p => p.Name != "resultSelector"));
        }

        public static IEnumerable<MethodInfo> WithoutEqualityComparer(this IEnumerable<MethodInfo> input)
        {
            LinqUtility.CheckNotNull("input", input);

            return input.Where(mi => !HasGenericDelegateOfType(mi, typeof(IEqualityComparer<>)));
        }

        private static bool HasGenericDelegateOfType(MethodInfo methodInfo, Type genericDelegateType)
        {
            return methodInfo.GetParameters()
                .Select(p => p.ParameterType.GetTypeInfo())
                .Any(p => p.IsGenericType && genericDelegateType.GetTypeInfo().IsAssignableFrom(p.GetGenericTypeDefinition().GetTypeInfo()));
        }
    }
}
