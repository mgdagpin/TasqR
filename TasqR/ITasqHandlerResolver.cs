using System;
using System.Collections.Generic;

namespace TasqR
{
    public interface ITasqHandlerResolver
    {
        IEnumerable<TypeTasqReference> RegisteredReferences { get; }

        void Register(TypeTasqReference handler);
        void Register<THandler>() where THandler : ITasqHandler;

        ITasqHandler ResolveHandler<TTasq>() where TTasq : ITasq;
        ITasqHandler ResolveHandler(Type type);
    }
}
