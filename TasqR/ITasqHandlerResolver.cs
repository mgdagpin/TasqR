using System;

namespace TasqR
{
    public interface ITasqHandlerResolver
    {
        void Register(TypeTasqReference handler);
        void Register<THandler>() where THandler : IJobTasqHandler;

        IJobTasqHandler ResolveHandler<TTasq>() where TTasq : ITasq;
        IJobTasqHandler ResolveHandler(Type type);
    }
}
