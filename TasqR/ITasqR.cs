using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TasqR.Common;

namespace TasqR
{
    public interface ITasqR
    {
        Guid ID { get; }

        event LogEventHandler OnLog;

        IEnumerable<TypeTasqReference> RegisteredReferences { get; }

        ITasqR UsingAsHandler(Type type);
        ITasqR UsingAsHandler<THandler>() where THandler : ITasqHandler;

        void Run(ITasq tasq);

        TResponse Run<TResponse>(ITasq<TResponse> tasq);

        IEnumerable<TResponse> Run<TKey, TResponse>(ITasq<TKey, TResponse> tasq);


        Task RunAsync(ITasq tasq, CancellationToken cancellationToken = default);

        Task<TResponse> RunAsync<TResponse>(ITasq<TResponse> tasq, CancellationToken cancellationToken = default);

        Task<IEnumerable<TResponse>> RunAsync<TKey, TResponse>(ITasq<TKey, TResponse> tasq, CancellationToken cancellationToken = default);

        Type GetHandlerType(ITasq tasq);
    }
}