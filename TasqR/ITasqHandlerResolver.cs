using System;
using System.Collections.Generic;

namespace TasqR
{
    public interface ITasqHandlerResolver
    {
        IEnumerable<TypeTasqReference> RegisteredReferences { get; }

        void Register(TypeTasqReference handler);
        void Register<THandler>() where THandler : IBaseTasqHandler;

        IBaseTasqHandler ResolveHandler<TTasq>() where TTasq : ITasq;
        IBaseTasqHandler ResolveHandler(Type type);
    }
}
