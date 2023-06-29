using System;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using TasqR.Common;
using TasqR.Processing;
using TasqR.Processing.Interfaces;

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
        public static void AddTasqR(this IServiceCollection services,
            ServiceLifetime tasqRServiceLifeTime = ServiceLifetime.Scoped,
            params Assembly[] assemblies)
        {
            services.AddTasqR<ProcessTracker>(tasqRServiceLifeTime, assemblies);
        }

        public static void AddTasqR<T>(this IServiceCollection services, 
            ServiceLifetime tasqRServiceLifeTime = ServiceLifetime.Scoped, 
            params Assembly[] assemblies)
             where T : IProcessTracker, new()
        {
            var instance = new TasqAssemblyCollection(assemblies);

            instance.RegisterFromAssembly();

            foreach (var handler in instance.GetAllHandlers())
            {
                services.AddTransient(handler);
            }

            services.AddSingleton(_ => instance);

            services.AddScoped<ITasqHandlerResolver, MicrosoftDependencyTasqHandlerResolver>();

            services.Add(new ServiceDescriptor(typeof(ITasqHandlerResolver), typeof(MicrosoftDependencyTasqHandlerResolver), tasqRServiceLifeTime));
            services.Add(new ServiceDescriptor(typeof(ITasqR), typeof(TasqR), tasqRServiceLifeTime));
            services.Add(new ServiceDescriptor(typeof(JobProcessor<T>), factory: _ => new JobProcessor<T>(), tasqRServiceLifeTime));
        }
    }
}