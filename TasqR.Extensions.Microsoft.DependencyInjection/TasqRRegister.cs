using System;
using System.Collections.Generic;
using System.Linq;
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

        public static void AddTasqR(this IServiceCollection services, params Assembly[] assemblies)
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

            var h = resolver.RegisterFromAssembly(assemblies);
            foreach (var ttHandler in h)
            {
                services.AddTransient(ttHandler.HandlerImplementation);
            }

            var dh = resolver.GetAllDerivedHandlers(assemblies);
            foreach (var ttHandler in dh)
            {
                services.AddTransient(ttHandler);
            }

            services.AddScoped<ITasqR>(p =>
            {
                var hr = new MicrosoftDependencyTasqHandlerResolver(p);

                hr.RegisterFromAssembly(assemblyList.ToArray());

                var t = new TasqRObject(hr);

                return t;
            });
        }
    }   
}
