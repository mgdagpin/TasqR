using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TasqR
{
    public static class TasqRRegister
    {
        static Dictionary<Type, Type> passMeHere = new Dictionary<Type, Type>();

        public static void AddTasqR(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddScoped<ITasqR, TasqRObject>();
            services.AddSingleton<ITasqHandlerCollection>(p => new TasqHandlerCollection(passMeHere, p));

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
                .Where(t => (typeof(IJobTasqHandler)).IsAssignableFrom(t) && t.IsConcrete())
                .Select(a => new
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
                            .Single(a => (typeof(ITasq)).IsAssignableFrom(a));

                        services.AddTransient(a.Interface, a.Type);

                        passMeHere[_cmd] = a.Interface;
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
        private readonly IServiceProvider seviceProvider;

        public Dictionary<Type, Type> TasqHanders { get; private set; }

        public TasqHandlerCollection(Dictionary<Type, Type> passMeHere, IServiceProvider seviceProvider)
        {
            TasqHanders = passMeHere;
            this.seviceProvider = seviceProvider;
        }

        public object GetService(Type type)
        {
            return this.seviceProvider.GetService(type);
        }
    }
}
