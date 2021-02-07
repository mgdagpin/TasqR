using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TasqR
{
    public abstract class TasqHandlerAsync<TProcess> : TasqHandler<TProcess>
       where TProcess : ITasq
    {
        public override void Initialize(TProcess tasq)
        {
            Task.Run(async () =>
            {
                await InitializeAsync(tasq);
            }).GetAwaiter().GetResult();
        }

        public virtual Task InitializeAsync(TProcess tasq, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }

        public override void Run(TProcess process)
        {
            Task.Run(async () =>
            {
                await RunAsync(process);
            }).GetAwaiter().GetResult();
        }

        public abstract Task RunAsync(TProcess process, CancellationToken cancellationToken = default);
    }

    public abstract class TasqHandlerAsync<TProcess, TResponse> : TasqHandler<TProcess, TResponse>
        where TProcess : ITasq<TResponse>
    {
        public override void Initialize(TProcess tasq)
        {
            Task.Run(async () =>
            {
                await InitializeAsync(tasq);
            }).GetAwaiter().GetResult();
        }

        public virtual Task InitializeAsync(TProcess tasq, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }

        public override TResponse Run(TProcess process)
        {
            return Task.Run(async () =>
            {
                return await RunAsync(process);
            }).GetAwaiter().GetResult();
        }

        public abstract Task<TResponse> RunAsync(TProcess process, CancellationToken cancellationToken = default);
    }

    public abstract class TasqHandlerAsync<TProcess, TKey, TResponse> : TasqHandler<TProcess, TKey, TResponse>
        where TProcess : ITasq<TKey, TResponse>
    {
        public override void Initialize(TProcess tasq)
        {
            Task.Run(async () =>
            {
                await InitializeAsync(tasq);
            }).GetAwaiter().GetResult();
        }

        public virtual Task InitializeAsync(TProcess tasq, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }

        public override IEnumerable<TKey> SelectionCriteria(TProcess tasq)
        {
            return Task.Run(async () =>
            {
                return await SelectionCriteriaAsync(tasq);
            }).GetAwaiter().GetResult();
        }

        public virtual Task<IEnumerable<TKey>> SelectionCriteriaAsync(TProcess tasq)
        {
            return Task.FromResult<IEnumerable<TKey>>(null);
        }

        public override TResponse Run(TKey key, TProcess process)
        {
            return Task.Run(async () =>
            {
                return await RunAsync(key, process);
            }).GetAwaiter().GetResult();
        }

        public abstract Task<TResponse> RunAsync(TKey key, TProcess process, CancellationToken cancellationToken = default);
    }
}
