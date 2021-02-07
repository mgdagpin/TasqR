using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace TasqR
{


    public static class TasqRRegister
    {
        static Dictionary<Type, Type> s_TypeReferenceDictionary = new Dictionary<Type, Type>();
        static List<TypeTasqReference> s_AllTypeReferences = new List<TypeTasqReference>();

        public static void AddTasqR(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddScoped<ITasqR, TasqRObject>();
            services.AddScoped<ITasqHandlerCollection>(p => new TasqHandlerCollection(s_TypeReferenceDictionary, s_AllTypeReferences, p));

            var assembliesToScan = assemblies.Distinct().ToArray();
            var excludedInterfaces = new[]
            {
                typeof(IDisposable),
                typeof(IBaseTasqHandler),
                typeof(IJobTasqHandler)
            };

            foreach (var assembly in assembliesToScan)
            {
                assembly.DefinedTypes
                .Where(t => t.IsAssignableTo(typeof(IJobTasqHandler)) && t.IsConcrete())
                .Select(a => new TypeTasqReference
                {
                    Type = a,
                    Interface = a.GetInterfaces()
                        .Where(a => !excludedInterfaces.Any(b => b == a))
                        .FirstOrDefault()
                })
                .ToList()
                .ForEach(a =>
                {
                    if (a.Interface != null)
                    {
                        var _cmd = a.Interface.GenericTypeArguments
                            .Single(a => a.IsAssignableTo(typeof(ITasq)));

                        services.AddTransient(a.Interface, a.Type);

                        s_TypeReferenceDictionary[_cmd] = a.Interface;

                        s_AllTypeReferences.Add(a);
                    }
                });
            }
        }

        private static bool IsConcrete(this Type type)
        {
            return !type.GetTypeInfo().IsAbstract && !type.GetTypeInfo().IsInterface;
        }
    }

    public class TasqHandlerCollection : ITasqHandlerCollection
    {
        private readonly IServiceProvider p_SeviceProvider;

        public Dictionary<Type, Type> TasqHanders { get; private set; }
        public IEnumerable<TypeTasqReference> TypeReferences { get; private set; }

        public TasqHandlerCollection
            (
                Dictionary<Type, Type> typeReferenceDictionary,
                IEnumerable<TypeTasqReference> allTypeReferences,
                IServiceProvider seviceProvider
            )
        {
            TasqHanders = typeReferenceDictionary;
            p_SeviceProvider = seviceProvider;
            TypeReferences = allTypeReferences;
        }

        public object GetService(Type type)
        {
            return this.p_SeviceProvider.GetService(type);
        }
    }
}
