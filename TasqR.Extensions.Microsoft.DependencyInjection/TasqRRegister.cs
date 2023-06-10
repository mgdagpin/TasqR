using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using TasqR.Common;

namespace TasqR
{
    public class MicrosoftDependencyTasqHandlerResolver : TasqHandlerResolver
    {
        private readonly IServiceProvider p_Provider;

        public MicrosoftDependencyTasqHandlerResolver(IServiceProvider provider)
        {
            p_Provider = provider;
        }

        public override object GetService(TypeTasqReference typeTasqReference)
        {
            return p_Provider.GetService(typeTasqReference.HandlerImplementation);
        }
    }

    public static class TasqRRegister
    {
        static List<Assembly> assemblyList = new List<Assembly>();

        public static void AddTasqR(this IServiceCollection services, ServiceLifetime tasqRServiceLifeTime = ServiceLifetime.Scoped, params Assembly[] assemblies)
        {
            if (assemblies != null)
            {
                foreach (var a in assemblies)
                {
                    if (!assemblyList.Contains(a))
                    {
                        assemblyList.Add(a);
                    }
                }
            }

            var resolver = new TasqHandlerResolver();

            var handlers = resolver.GetAllHandlers(assemblies);
            foreach (var handler in handlers)
            {
                services.AddTransient(handler);
            }

            if (tasqRServiceLifeTime == ServiceLifetime.Scoped)
            {
                services.AddScoped<ITasqR>(p =>
                {
                    var msDIHandlerResolver = new MicrosoftDependencyTasqHandlerResolver(p);

                    msDIHandlerResolver.RegisterFromAssembly(assemblyList.ToArray());

                    return new TasqR(msDIHandlerResolver);
                });
            }
            else if (tasqRServiceLifeTime == ServiceLifetime.Transient)
            {
                services.AddTransient<ITasqR>(p =>
                {
                    var msDIHandlerResolver = new MicrosoftDependencyTasqHandlerResolver(p);

                    msDIHandlerResolver.RegisterFromAssembly(assemblyList.ToArray());

                    return new TasqR(msDIHandlerResolver);
                });
            }            
        }
    }
}