using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TasqR
{
    public interface ITasqHandler { }
    public interface ITasqHandler<in TTasq> : ITasqHandler
        where TTasq : ITasq
    {
        void Initialize(TTasq tasq);
        void BeforeRun(TTasq tasq);
        void Run(TTasq tasq);
        void AfterRun(TTasq tasq);
    }

    public interface ITasqHandlerAsync<in TTasq> : ITasqHandler
    {
        Task InitializeAsync(TTasq tasq, CancellationToken cancellationToken = default);
        Task BeforeRunAsync(TTasq tasq, CancellationToken cancellationToken = default);
        Task RunAsync(TTasq tasq, CancellationToken cancellationToken = default);
        Task AfterRunAsync(TTasq tasq, CancellationToken cancellationToken = default);
    }

    public interface ITasqHandler<TTasq, out TResponse> : ITasqHandler
        where TTasq : ITasq<TResponse>
    {
        void Initialize(TTasq tasq);
        void BeforeRun(TTasq tasq);
        TResponse Run(TTasq tasq);
        void AfterRun(TTasq tasq);
    }

    public interface ITasqHandler<in TTasq, TKey, TResponse> : ITasqHandler
        where TTasq : ITasq<TKey, TResponse>
    {
        IEnumerable<TKey> SelectionCriteria(TTasq tasq);
        void Initialize(TTasq tasq);
        void BeforeRun(TTasq tasq);
        TResponse Run(TKey key, TTasq tasqt);
        void AfterRun(TTasq tasq);
    }

    public interface ITasqHandlerAsync<in TTasq, TKey, TResponse> : ITasqHandler
        where TTasq : ITasq<TKey, TResponse>
    {
        Task<IEnumerable<TKey>> SelectionCriteriaAsync(TTasq tasq, CancellationToken cancellationToken = default);
        Task InitializeAsync(TTasq tasq, CancellationToken cancellationToken = default);
        Task BeforeRunAsync(TTasq tasq, CancellationToken cancellationToken = default);
        Task<TResponse> RunAsync(TKey key, TTasq tasq, CancellationToken cancellationToken = default);
        Task AfterRunAsync(TTasq tasq, CancellationToken cancellationToken = default);
    }
}