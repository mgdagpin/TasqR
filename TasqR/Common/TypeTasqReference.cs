using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TasqR
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
            var handlerInterface = t.GetInterfaces()
                        .SingleOrDefault(a => a.IsGenericType && a.IsAssignableToTasqHandler());

            if (handlerInterface == null)
            {
                throw new TasqException($"{t.FullName} not inheritted from {nameof(ITasqHandler)}");
            }

            return new TypeTasqReference
            {
                TasqProcess = handlerInterface.GenericTypeArguments
                    .Single(a => a.IsAssignableToTasq()),
                HandlerImplementation = t,
                HandlerInterface = handlerInterface
            };
        }

        public static IEnumerable<TypeTasqReference> GetAllTypeTasqReference(Assembly assembly)
        {
            return assembly.DefinedTypes
                    .Where(t => t.IsAssignableToTasqHandler() && IsConcrete(t))
                    .Select(a => Resolve(a))
                    .ToList();
        }

        private static bool IsConcrete(Type type)
        {
            return !type.GetTypeInfo().IsAbstract && !type.GetTypeInfo().IsInterface;
        }
    }
}