using System;
using System.Collections.Generic;
using System.Reflection;
using TasqR.Common;

namespace TasqR
{
    public interface ITasqHandlerResolver
    {
        IEnumerable<TypeTasqReference> RegisteredReferences { get; }

        void Register(TypeTasqReference handler);
        void Register<THandler>() where THandler : ITasqHandler;

        TasqHandlerDetail ResolveHandler<TTasq>() where TTasq : ITasq;
        TasqHandlerDetail ResolveHandler(Type type);
        void RegisterFromAssembly(params Assembly[] assemblies);
        IEnumerable<Type> GetAllDerivedHandlers(params Assembly[] assemblies);

        object GetService(TypeTasqReference typeTasqReference);
    }
}
