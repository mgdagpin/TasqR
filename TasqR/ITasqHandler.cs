using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TasqR
{
    public interface ITasqHandler { }
    public interface ITasqHandler<in TTasq> : ITasqHandler
        where TTasq : ITasq
    {
        void Initialize(TTasq request);
        void BeforeRun(TTasq request);
        void Run(TTasq request);
        void AfterRun(TTasq request);
    }

    public interface ITasqHandlerAsync<in TTasq> : ITasqHandler
    {
        Task InitializeAsync(TTasq request, CancellationToken cancellationToken = default);
        Task BeforeRunAsync(TTasq request, CancellationToken cancellationToken = default);
        Task RunAsync(TTasq request, CancellationToken cancellationToken = default);
        Task AfterRunAsync(TTasq request, CancellationToken cancellationToken = default);
    }

    public interface ITasqHandler<TTasq, out TResponse> : ITasqHandler
        where TTasq : ITasq<TResponse>
    {
        void Initialize(TTasq request);
        void BeforeRun(TTasq request);
        TResponse Run(TTasq request);
        void AfterRun(TTasq request);
    }

    public interface ITasqHandler<in TTasq, TKey, TResponse> : ITasqHandler
        where TTasq : ITasq<TKey, TResponse>
    {
        void Initialize(TTasq request);
        IEnumerable SelectionCriteria(TTasq request);
        void BeforeRun(TTasq request);
        TResponse Run(TKey key, TTasq request);
        void AfterRun(TTasq request);
    }

    public interface ITasqHandlerAsync<in TTasq, TKey, TResponse> : ITasqHandler
        where TTasq : ITasq<TKey, TResponse>
    {
        Task<IEnumerable> SelectionCriteriaAsync(TTasq request, CancellationToken cancellationToken = default);
        Task InitializeAsync(TTasq request, CancellationToken cancellationToken = default);
        Task BeforeRunAsync(TTasq request, CancellationToken cancellationToken = default);
        Task<TResponse> RunAsync(TKey key, TTasq request, CancellationToken cancellationToken = default);
        Task AfterRunAsync(TTasq request, CancellationToken cancellationToken = default);
    }
}