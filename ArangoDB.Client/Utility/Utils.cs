using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Utility
{
    public class Utils
    {
        public static MemberInfo GetMemberInfo<T>(Expression<Func<T, object>> attribute)
        {
            var memberExpression = attribute.Body as MemberExpression;
            var unaryExpression = attribute.Body as UnaryExpression;
            if (unaryExpression != null)
                memberExpression = unaryExpression.Operand as MemberExpression;

            if (memberExpression == null || memberExpression.Member == null)
            {
                throw new InvalidOperationException("attribute should be value type");
            }

            return memberExpression.Member;
        }

        public static T CheckNotNull<T>(string argumentName, T actualValue)
        {
            if (actualValue == null)
                throw new ArgumentNullException(argumentName);

            return actualValue;
        }

        public static string EdgeDirectionToString(EdgeDirection direction)
        {
            switch (direction)
            {
                case EdgeDirection.Any:
                    return "any";
                case EdgeDirection.Inbound:
                    return "inbound";
                case EdgeDirection.Outbound:
                    return "outbound";
                default:
                    throw new InvalidOperationException($"EdgeDirection {direction} binding not found, this is a client bug");
            }
        }

        public static string UniquenessTypeToString(UniquenessType type)
        {
            switch (type)
            {
                case UniquenessType.Global:
                    return "global";
                case UniquenessType.None:
                    return "none";
                case UniquenessType.Path:
                    return "path";
                default:
                    throw new InvalidOperationException($"UniquenessType {type} binding not found, this is a client bug");
            }
        }

        public static string TraversalStrategyToString(TraversalStrategy strategy)
        {
            switch(strategy)
            {
                case TraversalStrategy.BreadthFirst:
                    return "breadthfirst";
                case TraversalStrategy.DepthFirst:
                    return "depthfirst";
                default:
                    throw new InvalidOperationException($"TraversalStrategy {strategy} binding not found, this is a client bug");
            }
        }

        public static string TraversalOrderToString(TraversalOrder order)
        {
            switch (order)
            {
                case TraversalOrder.Preorder:
                    return "preorder";
                case TraversalOrder.Postorder:
                    return "postorder";
                case TraversalOrder.PreorderExpander:
                    return "preorder-expander";
                default:
                    throw new InvalidOperationException($"TraversalOrder {order} binding not found, this is a client bug");
            }
        }

        public static string TraversalItemOrderToString(TraversalItemOrder itemOrder)
        {
            switch (itemOrder)
            {
                case TraversalItemOrder.Forward:
                    return "forward";
                case TraversalItemOrder.Backward:
                    return "backward";
                default:
                    throw new InvalidOperationException($"TraversalItemOrder {itemOrder} binding not found, this is a client bug");
            }
        }

        public static string GetAssemblyVersion()
        {
#if !PORTABLE
            //var version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
            //return version.Major + "." + version.Minor;
            return "";
#else
            // from http://stackoverflow.com/a/16525426/1271333
            var assembly = typeof(ArangoDatabase).GetTypeInfo().Assembly;
            // In some PCL profiles the above line is: var assembly = typeof(MyType).Assembly;
            var assemblyName = new AssemblyName(assembly.FullName);
            return assemblyName.Version.Major + "." + assemblyName.Version.Minor;
#endif
        }
    }
}
