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
            services.AddScoped<ITasqR>(p =>
            {
                ((MicrosoftDependencyTasqHandlerResolver)s_TasqHandlerResolver).SetServiceProvider(p);

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

        protected override object GetService(TypeTasqReference typeTasqReference)
        {
            return p_ServiceProvider.GetService(typeTasqReference.HandlerImplementation);
        }
    }
}
