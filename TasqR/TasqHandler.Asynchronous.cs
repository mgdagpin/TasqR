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

        internal virtual Task InitializeAsync(object request) => throw new NotImplementedException();
        internal virtual Task<IEnumerable> SelectionCriteriaAsync(object request) => throw new NotImplementedException();
        internal virtual Task BeforeRunAsync(object request) => throw new NotImplementedException();
        internal virtual Task XRunAsync(object key, object request) => throw new NotImplementedException();
        internal virtual Task AfterRunAsync(object request) => throw new NotImplementedException();
    }

    public abstract class TasqHandlerAsync<TProcess> : TasqHandlerAsync, ITasqHandlerAsync<TProcess>
       where TProcess : ITasq
    {
        #region TasqHandler Calls
        internal override Task InitializeAsync(object request) => InitializeAsync((TProcess)request, p_CancellationToken);
        internal override Task BeforeRunAsync(object request) => BeforeRunAsync((TProcess)request, p_CancellationToken);
        internal override Task XRunAsync(object key, object request) => RunAsync((TProcess)request, p_CancellationToken);
        internal override Task AfterRunAsync(object request) => AfterRunAsync((TProcess)request, p_CancellationToken);
        #endregion


        public virtual Task InitializeAsync(TProcess request, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public virtual Task BeforeRunAsync(TProcess request, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public abstract Task RunAsync(TProcess request, CancellationToken cancellationToken = default);
        public virtual Task AfterRunAsync(TProcess request, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    public abstract class TasqHandlerAsync<TProcess, TResponse> : TasqHandlerAsync, ITasqHandlerAsync<TProcess, TResponse>
        where TProcess : ITasq<TResponse>
    {
        #region TasqHandler Calls
        internal override Task InitializeAsync(object request) => InitializeAsync((TProcess)request, p_CancellationToken);
        internal override Task BeforeRunAsync(object request) => BeforeRunAsync((TProcess)request, p_CancellationToken);
        internal override Task XRunAsync(object key, object request) => RunAsync((TProcess)request, p_CancellationToken);
        internal override Task AfterRunAsync(object request) => AfterRunAsync((TProcess)request, p_CancellationToken);
        #endregion


        public virtual Task InitializeAsync(TProcess request, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public virtual Task BeforeRunAsync(TProcess request, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public abstract Task<TResponse> RunAsync(TProcess request, CancellationToken cancellationToken = default);
        public virtual Task AfterRunAsync(TProcess request, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    public abstract class TasqHandlerAsync<TProcess, TKey, TResponse> : TasqHandlerAsync, ITasqHandlerAsync<TProcess, TKey, TResponse>
        where TProcess : ITasq<TKey, TResponse>
    {
        #region TasqHandler Calls
        internal override Task InitializeAsync(object request) => InitializeAsync((TProcess)request, p_CancellationToken);
        internal override Task<IEnumerable> SelectionCriteriaAsync(object request) => SelectionCriteriaAsync((TProcess)request, p_CancellationToken);
        internal override Task BeforeRunAsync(object request) => BeforeRunAsync((TProcess)request, p_CancellationToken);
        internal override Task XRunAsync(object key, object request) => RunAsync((TKey)key, (TProcess)request, p_CancellationToken);
        internal override Task AfterRunAsync(object request) => AfterRunAsync((TProcess)request, p_CancellationToken);
        #endregion


        public virtual Task InitializeAsync(TProcess request, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public virtual Task BeforeRunAsync(TProcess request, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public virtual Task AfterRunAsync(TProcess request, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public abstract Task<IEnumerable> SelectionCriteriaAsync(TProcess request, CancellationToken cancellationToken = default);
        public abstract Task<TResponse> RunAsync(TKey key, TProcess request, CancellationToken cancellationToken = default);        
    }
}
