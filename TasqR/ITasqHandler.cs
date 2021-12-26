using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TasqR
{
    public interface ITasqHandler { }

    public interface ITasqHandler<TTasq> : ITasqHandler
        where TTasq : ITasq
    {
        void Initialize(TTasq request);
        void BeforeRun(TTasq request);
        void Run(TTasq request);
        void AfterRun(TTasq request);
    }

    public interface ITasqHandler<TTasq, TResponse> : ITasqHandler
        where TTasq : ITasq<TResponse>
    {
        void Initialize(TTasq request);
        void BeforeRun(TTasq request);
        TResponse Run(TTasq request);
        void AfterRun(TTasq request);
    }

    public interface ITasqHandler<TTasq, TKey, TResponse> : ITasqHandler
        where TTasq : ITasq<TKey, TResponse>
    {
        void Initialize(TTasq request);
        IEnumerable SelectionCriteria(TTasq request);
        void BeforeRun(TTasq request);
        TResponse Run(TKey key, TTasq request);
        void AfterRun(TTasq request);
    }    
}