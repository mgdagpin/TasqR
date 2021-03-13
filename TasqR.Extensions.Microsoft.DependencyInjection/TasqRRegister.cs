using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TasqR.Common;

namespace TasqR
{
    public static class TasqRRegister
    {
        static ITasqHandlerResolver s_TasqHandlerResolver = new MicrosoftDependencyTasqHandlerResolver();

        public static void AddTasqR(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddScoped<ITasqR>(p =>
            {
                ((MicrosoftDependencyTasqHandlerResolver)s_TasqHandlerResolver).SetServiceProvider(p);

                return new TasqRObject(s_TasqHandlerResolver);
            });

            s_TasqHandlerResolver.RegisterFromAssembly(assemblies);

            foreach (var ttHandler in s_TasqHandlerResolver.RegisteredReferences)
            {
                services.AddTransient(ttHandler.HandlerImplementation);
            }

            var allDerivedHandlers = s_TasqHandlerResolver.GetAllDerivedHandlers(assemblies);

            foreach (var dh in allDerivedHandlers)
            {
                services.AddTransient(dh);
            }
        }
    }

    public class MicrosoftDependencyTasqHandlerResolver : TasqHandlerResolver
    {
        private IServiceProvider p_ServiceProvider;

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            p_ServiceProvider = serviceProvider;
        }

        public override object GetService(TypeTasqReference typeTasqReference)
        {
            return p_ServiceProvider.GetService(typeTasqReference.HandlerImplementation);
        }
    }
}
