using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TasqR
{
    public interface ITasqHandlerAsync<TTasq> : ITasqHandler
    {
        Task InitializeAsync(TTasq request, CancellationToken cancellationToken = default);
        Task BeforeRunAsync(TTasq request, CancellationToken cancellationToken = default);
        Task RunAsync(TTasq request, CancellationToken cancellationToken = default);
        Task AfterRunAsync(TTasq request, CancellationToken cancellationToken = default);
    }

    public interface ITasqHandlerAsync<TTasq, TResponse> : ITasqHandler
        where TTasq : ITasq<TResponse>
    {
        Task InitializeAsync(TTasq request, CancellationToken cancellationToken = default);
        Task BeforeRunAsync(TTasq request, CancellationToken cancellationToken = default);
        Task<TResponse> RunAsync(TTasq request, CancellationToken cancellationToken = default);
        Task AfterRunAsync(TTasq request, CancellationToken cancellationToken = default);
    }

    public interface ITasqHandlerAsync<TTasq, TKey, TResponse> : ITasqHandler
        where TTasq : ITasq<TKey, TResponse>
    {
        IAsyncEnumerable<TKey> SelectionCriteriaAsync(TTasq request, CancellationToken cancellationToken = default);

        Task InitializeAsync(TTasq request, CancellationToken cancellationToken = default);
        Task BeforeRunAsync(TTasq request, CancellationToken cancellationToken = default);
        Task<TResponse> RunAsync(TKey key, TTasq request, CancellationToken cancellationToken = default);
        Task AfterRunAsync(TTasq request, CancellationToken cancellationToken = default);
    }
}
