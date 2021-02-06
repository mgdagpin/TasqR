using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace TasqR
{
    public static class TasqRRegister
    {
        static ITasqHandlerResolver s_TasqHandlerResolver = new MicrosoftDependencyTasqHandlerResolver();

        public static void AddTasqR(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddScoped<ITasqR>(p =>
            {
                ((MicrosoftDependencyTasqHandlerResolver)s_TasqHandlerResolver)
                    .SetServiceProvider(p);

                return new TasqRObject(s_TasqHandlerResolver);
            });

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
                        .FirstOrDefault(a => !excludedInterfaces.Any(b => b == a))
                })
                .ToList()
                .ForEach(a =>
                {
                    if (a.Interface != null)
                    {
                        var _cmd = a.Interface.GenericTypeArguments
                            .Single(a => a.IsAssignableTo(typeof(ITasq)));

                        services.AddTransient(a.Interface, a.Type);

                        s_TasqHandlerResolver.Register(_cmd, a.Interface);
                    }
                });
            }
        }

        private static bool IsConcrete(this Type type)
        {
            return !type.GetTypeInfo().IsAbstract && !type.GetTypeInfo().IsInterface;
        }
    }

    public class MicrosoftDependencyTasqHandlerResolver : TasqHandlerResolver
    {
        private IServiceProvider p_ServiceProvider;

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            p_ServiceProvider = serviceProvider;
        }

        protected override object GetService(Type type)
        {
            return p_ServiceProvider.GetService(type);
        }
    }
}
