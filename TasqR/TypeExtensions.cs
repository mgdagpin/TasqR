using System;
using System.Linq;

namespace TasqR
{
    public static class TypeExtensions
    {
        public static bool HasBaseType(this Type type, Type baseType)
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

        public static bool IsAssignableToTasq(this Type type)
        {
            var toType = typeof(ITasq);

            return type.IsAssignableTo2(toType);
        }

        public static bool IsAssignableToTasqHandler(this Type type)
        {
            var toType = typeof(IJobTasqHandler);

            return type.IsAssignableTo2(toType);
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
