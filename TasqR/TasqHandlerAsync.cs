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
            BeforeRunAsync((TProcess)tasq, p_CancellationToken).Wait();
        }

        internal override void AfterRun(object tasq)
        {
            AfterRunAsync((TProcess)tasq, p_CancellationToken).Wait();
        }

        internal override void Initialize(object tasq)
        {
            InitializeAsync((TProcess)tasq, p_CancellationToken).Wait();
        }

        internal override object Run(object key, object tasq)
        {
            RunAsync((TProcess)tasq, p_CancellationToken).Wait();

            return null;
        }
        #endregion




        public virtual Task InitializeAsync(TProcess tasq, CancellationToken cancellationToken = default) => Task.FromResult(0);

        public abstract Task RunAsync(TProcess process, CancellationToken cancellationToken = default);

        public virtual Task BeforeRunAsync(TProcess tasq, CancellationToken cancellationToken = default) => Task.FromResult(0);

        public virtual Task AfterRunAsync(TProcess tasq, CancellationToken cancellationToken = default) => Task.FromResult(0);
    }

    public abstract class TasqHandlerAsync<TProcess, TResponse> : TasqHandler<TProcess, TResponse>
        where TProcess : ITasq<TResponse>
    {
        public override void Initialize(TProcess tasq)
        {
            InitializeAsync(tasq, p_CancellationToken).Wait();
        }

        public virtual Task InitializeAsync(TProcess tasq, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }

        public override TResponse Run(TProcess process)
        {
            return RunAsync(process, p_CancellationToken).Result;
        }

        public abstract Task<TResponse> RunAsync(TProcess process, CancellationToken cancellationToken = default);
    }

    public abstract class TasqHandlerAsync<TProcess, TKey, TResponse> : TasqHandler, ITasqHandlerAsync<TProcess, TKey, TResponse>
        where TProcess : ITasq<TKey, TResponse>
    {
        #region TasqHandler Calls
        internal override void BeforeRun(object tasq)
        {
            BeforeRunAsync((TProcess)tasq, p_CancellationToken).Wait();
        }

        internal override void AfterRun(object tasq)
        {
            AfterRunAsync((TProcess)tasq, p_CancellationToken).Wait();
        }

        internal override void Initialize(object tasq)
        {
            InitializeAsync((TProcess)tasq, p_CancellationToken).Wait();
        }

        internal override object Run(object key, object tasq)
        {
            TKey k = default;

            if (key != null)
            {
                k = (TKey)key;
            }

            return RunAsync(k, (TProcess)tasq, p_CancellationToken).Result;
        }

        internal override IEnumerable SelectionCriteria(object tasq)
        {
            return SelectionCriteriaAsync((TProcess)tasq, p_CancellationToken).Result;
        }
        #endregion


        public abstract Task<TResponse> RunAsync(TKey key, TProcess process, CancellationToken cancellationToken = default);

        public abstract Task<IEnumerable<TKey>> SelectionCriteriaAsync(TProcess tasq, CancellationToken cancellationToken = default);

        public virtual Task BeforeRunAsync(TProcess tasq, CancellationToken cancellationToken = default) => Task.FromResult(0);

        public virtual Task AfterRunAsync(TProcess tasq, CancellationToken cancellationToken = default) => Task.FromResult(0);

        public virtual Task InitializeAsync(TProcess tasq, CancellationToken cancellationToken = default) => Task.FromResult(0);
    }
}
