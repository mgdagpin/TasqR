using System;
using System.Threading;
using System.Threading.Tasks;

namespace TasqR
{
    public interface ITasqR
    {
        Guid ID { get; }

        event ProcessEventHandler OnInitializeExecuting;
        event ProcessEventHandler OnInitializeExecuted;

        public event ProcessEventHandler OnSelectionCriteriaExecuting;
        public event ProcessEventHandler OnSelectionCriteriaExecuted;

        event ProcessEventHandler OnBeforeRunExecuting;
        event ProcessEventHandler OnBeforeRunExecuted;

        event ProcessEventHandler OnRunExecuting;
        event ProcessEventHandler OnRunExecuted;

        event ProcessEventHandler OnAfterRunExecuting;
        event ProcessEventHandler OnAfterRunExecuted;

        void Run(ITasq tasq);
        TResponse Run<TResponse>(ITasq<TResponse> tasq);
        TResponse Run<TKey, TResponse>(ITasq<TKey, TResponse> tasq);


        Task RunAsync(ITasq tasq, CancellationToken cancellationToken = default);
        Task<TResponse> RunAsync<TResponse>(ITasq<TResponse> tasq, CancellationToken cancellationToken = default);
        Task<TResponse> RunAsync<TKey, TResponse>(ITasq<TKey, TResponse> tasq, CancellationToken cancellationToken = default);

    }
}