using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TasqR.Common
{
    public class TypeTasqReference
    {
        public Type TasqProcess { get; set; }
        public Type HandlerImplementation { get; set; }
        public Type HandlerInterface { get; set; }

        public static TypeTasqReference Resolve<TTaskHandler>() where TTaskHandler : ITasqHandler
        {
            return Resolve(typeof(TTaskHandler));
        }

        public static TypeTasqReference Resolve(Type t)
        {
            var hI = t.GetInterfaces()
                .Where(a => typeof(ITasqHandler).IsAssignableFrom(a))
                .OrderByDescending(a => a.GenericTypeArguments.Length)
                .ToList();

            if (!t.IsAssignableToTasqHandler())
            {
                throw new TasqException($"{t.FullName} not inheritted from {nameof(ITasqHandler)}");
            }

            return new TypeTasqReference
            {
                TasqProcess = TryFindGenericBaseType(t).GenericTypeArguments
                    .Single(a => a.IsAssignableToTasq()),
                HandlerImplementation = t,
                HandlerInterface = hI.FirstOrDefault()
            };
        }

        private static Type TryFindGenericBaseType(Type t)
        {
            if (t.BaseType.IsGenericType)
            {
                return t.BaseType;
            }

            return TryFindGenericBaseType(t.BaseType);
        }

        public static IEnumerable<TypeTasqReference> GetAllTypeTasqReference(Assembly assembly)
        {
            return assembly.DefinedTypes
                    .Where(t => IsValidHandler(t))
                    .Select(a => Resolve(a))
                    .ToList();
        }

        public static bool IsValidHandler(Type type)
        {
            return type.IsAssignableToTasqHandler() && IsConcrete(type);
        }

        private static bool IsConcrete(Type type)
        {
            return !type.GetTypeInfo().IsAbstract && !type.GetTypeInfo().IsInterface;
        }
    }
}