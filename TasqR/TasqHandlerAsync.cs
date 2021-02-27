using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TasqR
{
    public abstract class TasqHandlerAsync : ITasqHandler
    {
        internal CancellationToken p_CancellationToken { get; set; }

        internal virtual Task InitializeAsync(object tasq) => throw new NotImplementedException();
        internal virtual Task<IEnumerable> SelectionCriteriaAsync(object tasq) => throw new NotImplementedException();
        internal virtual Task BeforeRunAsync(object tasq) => throw new NotImplementedException();
        internal virtual Task XRunAsync(object key, object tasq) => throw new NotImplementedException();
        internal virtual Task AfterRunAsync(object tasq) => throw new NotImplementedException();
    }

    public abstract class TasqHandlerAsync<TProcess> : TasqHandlerAsync, ITasqHandlerAsync<TProcess>
       where TProcess : ITasq
    {
        #region TasqHandler Calls
        internal override Task InitializeAsync(object tasq) => InitializeAsync((TProcess)tasq, p_CancellationToken);
        internal override Task BeforeRunAsync(object tasq) => BeforeRunAsync((TProcess)tasq, p_CancellationToken);
        internal override Task XRunAsync(object key, object tasq) => RunAsync((TProcess)tasq, p_CancellationToken);
        internal override Task AfterRunAsync(object tasq) => AfterRunAsync((TProcess)tasq, p_CancellationToken);
        #endregion


        public virtual Task InitializeAsync(TProcess tasq, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public virtual Task BeforeRunAsync(TProcess tasq, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public abstract Task RunAsync(TProcess request, CancellationToken cancellationToken = default);
        public virtual Task AfterRunAsync(TProcess tasq, CancellationToken cancellationToken = default) => Task.FromResult(0);
    }

    public abstract class TasqHandlerAsync<TProcess, TResponse> : TasqHandlerAsync
        where TProcess : ITasq<TResponse>
    {
        #region TasqHandler Calls
        internal override Task InitializeAsync(object tasq) => InitializeAsync((TProcess)tasq, p_CancellationToken);
        internal override Task BeforeRunAsync(object tasq) => BeforeRunAsync((TProcess)tasq, p_CancellationToken);
        internal override Task XRunAsync(object key, object tasq) => RunAsync((TProcess)tasq, p_CancellationToken);
        internal override Task AfterRunAsync(object tasq) => AfterRunAsync((TProcess)tasq, p_CancellationToken);
        #endregion


        public virtual Task InitializeAsync(TProcess request, CancellationToken cancellationToken) => Task.FromResult((TResponse)default);
        public virtual Task BeforeRunAsync(TProcess tasq, CancellationToken cancellationToken = default) => Task.FromResult((TResponse)default);
        public abstract Task<TResponse> RunAsync(TProcess request, CancellationToken cancellationToken = default);
        public virtual Task AfterRunAsync(TProcess tasq, CancellationToken cancellationToken = default) => Task.FromResult((TResponse)default);
    }

    public abstract class TasqHandlerAsync<TProcess, TKey, TResponse> : TasqHandlerAsync, ITasqHandlerAsync<TProcess, TKey, TResponse>
        where TProcess : ITasq<TKey, TResponse>
    {
        #region TasqHandler Calls
        internal override Task InitializeAsync(object tasq) => InitializeAsync((TProcess)tasq, p_CancellationToken);
        internal override Task<IEnumerable> SelectionCriteriaAsync(object tasq) => SelectionCriteriaAsync((TProcess)tasq, p_CancellationToken);
        internal override Task BeforeRunAsync(object tasq) => BeforeRunAsync((TProcess)tasq, p_CancellationToken);
        internal override Task XRunAsync(object key, object tasq) => RunAsync((TKey)key, (TProcess)tasq, p_CancellationToken);
        internal override Task AfterRunAsync(object tasq) => AfterRunAsync((TProcess)tasq, p_CancellationToken);
        #endregion


        public virtual Task InitializeAsync(TProcess tasq, CancellationToken cancellationToken = default) => Task.FromResult((TResponse)default);
        public virtual Task<IEnumerable> SelectionCriteriaAsync(TProcess request, CancellationToken cancellationToken) => Task.FromResult((IEnumerable)default);
        public virtual Task BeforeRunAsync(TProcess tasq, CancellationToken cancellationToken = default) => Task.FromResult((TResponse)default);
        
        public virtual Task AfterRunAsync(TProcess tasq, CancellationToken cancellationToken = default) => Task.FromResult((TResponse)default);

        public abstract Task<TResponse> RunAsync(TKey key, TProcess tasq, CancellationToken cancellationToken = default);        
    }
}
