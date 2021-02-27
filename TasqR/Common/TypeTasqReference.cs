﻿using System;
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
                .Where(a => a.IsAssignableToTasqHandler())
                .OrderByDescending(a => a.GenericTypeArguments.Length);

            if (!t.IsAssignableToTasqHandler())
            {
                throw new TasqException($"{t.FullName} not inheritted from {nameof(ITasqHandler)}");
            }

            return new TypeTasqReference
            {
                TasqProcess = t.BaseType.GenericTypeArguments
                    .Single(a => a.IsAssignableToTasq()),
                HandlerImplementation = t,
                HandlerInterface = hI.FirstOrDefault()
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