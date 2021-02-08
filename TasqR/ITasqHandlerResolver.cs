using System;
using System.Collections.Generic;
using TasqR.Common;

namespace TasqR
{
    public interface ITasqHandlerResolver
    {
        IEnumerable<TypeTasqReference> RegisteredReferences { get; }

        void Register(TypeTasqReference handler);
        void Register<THandler>() where THandler : IBaseTasqHandler;

        TasqHandlerDetail ResolveHandler<TTasq>() where TTasq : ITasq;
        TasqHandlerDetail ResolveHandler(Type type);
    }
}
