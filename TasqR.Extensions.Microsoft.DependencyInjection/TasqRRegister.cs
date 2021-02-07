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
                ((MicrosoftDependencyTasqHandlerResolver)s_TasqHandlerResolver).SetServiceProvider(p);

                return new TasqRObject(s_TasqHandlerResolver);
            });

            var assembliesToScan = assemblies.Distinct().ToArray();

            foreach (var assembly in assembliesToScan)
            {
                var ttHandlers = TypeTasqReference.GetAllTypeTasqReference(assembly);

                foreach (var ttHandler in ttHandlers)
                {
                    s_TasqHandlerResolver.Register(ttHandler);

                    services.AddTransient(ttHandler.HandlerImplementation);
                }
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

        protected override object GetService(TypeTasqReference typeTasqReference)
        {
            return p_ServiceProvider.GetService(typeTasqReference.HandlerImplementation);
        }
    }
}
