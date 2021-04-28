using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TasqR.Common;

namespace TasqR
{
    public static class TasqRRegister
    {
        static bool IsRegistered = false;
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

            TasqHandlerResolver resolver = new TasqHandlerResolver();

            var h = resolver.RegisterFromAssembly(assemblyList.ToArray());
            foreach (var ttHandler in h)
            {
                services.AddTransient(ttHandler.HandlerImplementation);
            }

            var dh = resolver.GetAllDerivedHandlers(assemblyList.ToArray());
            foreach (var ttHandler in dh)
            {
                services.AddTransient(ttHandler);
            }


            if (!IsRegistered)
            {
                services.AddSingleton<TasqHandlerResolver>(p =>
                {
                    TasqHandlerResolver resolver2 = new TasqHandlerResolver();
                    resolver2.RegisterFromAssembly(assemblyList.ToArray());

                    return resolver2;
                });

                services.AddSingleton<ITasqHandlerResolver, MicrosoftDependencyTasqHandlerResolver>();

                services.AddSingleton<ITasqR, TasqRObject>();

                IsRegistered = true;
            }                  
        }
    }

    public class MicrosoftDependencyTasqHandlerResolver : TasqHandlerResolver
    {
        private readonly IServiceProvider p_Provider;
        private readonly TasqHandlerResolver p_Resolver;

        public MicrosoftDependencyTasqHandlerResolver(IServiceProvider provider, TasqHandlerResolver resolver)
        {
            p_Provider = provider;
            p_Resolver = resolver;
        }

        public override TasqHandlerDetail ResolveHandler(Type type)
        {
            if (!p_Resolver.TasqHanders.ContainsKey(type))
            {
                throw new TasqException($"Type {GetFullName(type)} not registered");
            }

            var tasqHandlerType = p_Resolver.TasqHanders[type];

            var tasqHandlerInstance = (ITasqHandler)GetService(tasqHandlerType);

            if (tasqHandlerInstance == null)
            {
                throw new TasqException($"Type {GetFullName(tasqHandlerType.HandlerImplementation)} not registered");
            }

            return new TasqHandlerDetail
            {
                Handler = tasqHandlerInstance,
                Reference = tasqHandlerType
            };
        }

        public override object GetService(TypeTasqReference typeTasqReference)
        {
            var a = p_Provider.CreateScope().ServiceProvider.GetService(typeTasqReference.HandlerImplementation);

            //var s = p_Provider.GetService(typeTasqReference.HandlerImplementation);

            return a;
        }
    }
}
