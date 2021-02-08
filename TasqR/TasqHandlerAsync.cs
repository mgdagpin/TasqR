using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TasqR
{
    public abstract class TasqHandlerAsync<TProcess> : TasqHandler, ITasqHandlerAsync<TProcess>
       where TProcess : ITasq
    {
        #region TasqHandler Calls
        internal override void BeforeRun(object tasq)
        {
            Task.Run(async () =>
            {
                await BeforeRunAsync((TProcess)tasq);
            }).GetAwaiter().GetResult();
        }

        internal override void AfterRun(object tasq)
        {
            Task.Run(async () =>
            {
                await AfterRunAsync((TProcess)tasq);
            }).GetAwaiter().GetResult();
        }

        internal override void Initialize(object tasq)
        {
            Task.Run(async () =>
            {
                await InitializeAsync((TProcess)tasq);
            }).GetAwaiter().GetResult();
        }

        internal override object Run(object key, object tasq)
        {
            return Task.Run<object>(async () =>
            {
                await RunAsync((TProcess)tasq);

                return null;
            }).GetAwaiter().GetResult();
        }
        #endregion




        public virtual Task InitializeAsync(TProcess tasq, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public abstract Task RunAsync(TProcess process, CancellationToken cancellationToken = default);

        public virtual Task BeforeRunAsync(TProcess tasq, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public virtual Task AfterRunAsync(TProcess tasq, CancellationToken cancellationToken = default) => Task.CompletedTask;
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

    public abstract class TasqHandlerAsync<TProcess, TKey, TResponse> : TasqHandler, ITasqHandlerAsync<TProcess, TKey, TResponse>
        where TProcess : ITasq<TKey, TResponse>
    {
        #region TasqHandler Calls
        internal override void BeforeRun(object tasq)
        {
            Task.Run(async () =>
            {
                await BeforeRunAsync((TProcess)tasq);
            }).GetAwaiter().GetResult();
        }

        internal override void AfterRun(object tasq)
        {
            Task.Run(async () =>
            {
                await AfterRunAsync((TProcess)tasq);
            }).GetAwaiter().GetResult();
        }

        internal override void Initialize(object tasq)
        {
            Task.Run(async () =>
            {
                await InitializeAsync((TProcess)tasq);
            }).GetAwaiter().GetResult();
        }

        internal override object Run(object key, object tasq)
        {
            return Task.Run<object>(async () =>
            {
                TKey k = default;

                if (key != null)
                {
                    k = (TKey)key;
                }

                return await RunAsync(k, (TProcess)tasq);
            }).GetAwaiter().GetResult();
        }

        internal override IEnumerable SelectionCriteria(object tasq)
        {
            return Task.Run<IEnumerable>(async () =>
            {
                return await SelectionCriteriaAsync((TProcess)tasq);
            }).GetAwaiter().GetResult();
        }
        #endregion


        public abstract Task<TResponse> RunAsync(TKey key, TProcess process, CancellationToken cancellationToken = default);

        public abstract Task<IEnumerable<TKey>> SelectionCriteriaAsync(TProcess tasq, CancellationToken cancellationToken = default);

        public virtual Task BeforeRunAsync(TProcess tasq, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public virtual Task AfterRunAsync(TProcess tasq, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public virtual Task InitializeAsync(TProcess tasq, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
