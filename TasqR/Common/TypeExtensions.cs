using System;
using System.Linq;
using System.Text;

namespace TasqR.Common
{
    internal static class TypeExtensions
    {
        internal static bool HasBaseType(this Type type, Type baseType)
        {
            if (type == baseType)
            {
                return true;
            }

            if (type.BaseType == null || type.BaseType == typeof(object))
            {
                return false;
            }

            if (type.BaseType.IsGenericType
                && type.BaseType.GetGenericTypeDefinition() == baseType)
            {
                return true;
            }

            if (type.BaseType == baseType)
            {
                return true;
            }

            return type.BaseType.HasBaseType(baseType);
        }

        internal static bool IsAssignableToTasq(this Type type)
        {
            var toType = typeof(ITasq);

            return type.IsAssignableTo2(toType);
        }

        internal static bool IsDirectDerivedFromTasqHandler(this Type type)
        {
            if (type.BaseType == null)
            {
                return false;
            }

            if (type.BaseType == typeof(TasqHandler) || type.BaseType.BaseType == typeof(TasqHandler))
            {
                return true;
            }

            if (type.BaseType == typeof(TasqHandlerAsync) || type.BaseType.BaseType == typeof(TasqHandlerAsync))
            {
                return true;
            }

            return false;
        }

        internal static bool IsAssignableToTasqHandler(this Type type)
        {
            if (type.BaseType == null)
            {
                return false;
            }

            if (type.IsSubclassOf(typeof(TasqHandler))
                || type.IsSubclassOf(typeof(TasqHandler<>))
                || type.IsSubclassOf(typeof(TasqHandler<,>))
                || type.IsSubclassOf(typeof(TasqHandler<,,>)))
            {
                return true;
            }

            if (type.IsSubclassOf(typeof(TasqHandlerAsync))
                || type.IsSubclassOf(typeof(TasqHandlerAsync<>))
                || type.IsSubclassOf(typeof(TasqHandlerAsync<,>))
                || type.IsSubclassOf(typeof(TasqHandlerAsync<,,>)))
            {
                return true;
            }

            return false;
            //var toType = typeof(ITasqHandler);

            //return type.IsAssignableTo2(toType);
        }

        /// See <see href=" https://stackoverflow.com/a/1533349" /> for reference
        internal static string GetReadableFullName(this Type t)
        {
            if (!t.IsGenericType)
                return t.Name;

            var sb = new StringBuilder();

            sb.Append(t.Name.Substring(0, t.Name.LastIndexOf("`")));
            sb.Append
                (
                    t.GetGenericArguments().Aggregate
                        (
                            "<",
                            delegate (string aggregate, Type type)
                            {
                                return aggregate + (aggregate == "<" ? "" : ",") + GetReadableFullName(type);
                            }
                        )
                );

            sb.Append(">");

            return sb.ToString();
        }

        static bool IsAssignableTo2(this Type type, Type toType)
        {
            if (type.GetInterfaces().Any(a => a == toType))
            {
                return true;
            }

            if (type.BaseType == toType)
            {
                return true;
            }

            if (type.HasBaseType(toType))
            {
                return true;
            }

            return false;
        }
    }
}
