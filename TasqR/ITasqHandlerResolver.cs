using System;

namespace TasqR
{
    public interface ITasqHandlerResolver
    {
        void Register(Type tasq, Type handler);

        IJobTasqHandler ResolveHandler<TTasq>() where TTasq : ITasq;
        IJobTasqHandler ResolveHandler(Type type);
    }
}
