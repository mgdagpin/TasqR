using System;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using TasqR.Common;
using TasqR.Processing;

namespace TasqR
{
    public class MicrosoftDependencyTasqHandlerResolver : TasqHandlerResolver
    {
        private readonly IServiceProvider p_Provider;

        public MicrosoftDependencyTasqHandlerResolver(TasqAssemblyCollection tasqAssemblyCollection, IServiceProvider provider) 
            : base(tasqAssemblyCollection)
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
        static TasqAssemblyCollection p_TasqAssemblyCollection = new TasqAssemblyCollection();

        public static void AddTasqR(this IServiceCollection services,
            ServiceLifetime tasqRServiceLifeTime = ServiceLifetime.Scoped,
            params Assembly[] assemblies)
        {
            services.AddTasqR<ProcessTracker>(tasqRServiceLifeTime, assemblies);
        }

        public static void AddTasqR<T>(this IServiceCollection services, 
            ServiceLifetime tasqRServiceLifeTime = ServiceLifetime.Scoped, 
            params Assembly[] assemblies)
             where T : IProcessTracker
        {
            p_TasqAssemblyCollection.RegisterFromAssembly(assemblies);

            foreach (var handler in p_TasqAssemblyCollection.GetAllHandlers(assemblies))
            {
                services.AddTransient(handler);
            }

            services.AddSingleton(_ => p_TasqAssemblyCollection);

            services.AddScoped<ITasqHandlerResolver, MicrosoftDependencyTasqHandlerResolver>();

            services.Add(new ServiceDescriptor(typeof(ITasqHandlerResolver), typeof(MicrosoftDependencyTasqHandlerResolver), tasqRServiceLifeTime));
            services.Add(new ServiceDescriptor(typeof(ITasqR), typeof(TasqR), tasqRServiceLifeTime));
            services.Add(new ServiceDescriptor(typeof(JobProcessor<T>), factory: _ => new JobProcessor<T>(), tasqRServiceLifeTime));
        }
    }
}