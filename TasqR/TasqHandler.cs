using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace TasqR
{
    public abstract class TasqHandler : ITasqHandler
    {
        internal CancellationToken p_CancellationToken { get; set; }

        internal virtual void Initialize(object request) => throw new NotImplementedException();
        internal virtual IEnumerable SelectionCriteria(object request) => throw new NotImplementedException();
        internal virtual void BeforeRun(object request) => throw new NotImplementedException();
        internal virtual object Run(object key, object request) => throw new NotImplementedException();
        internal virtual void AfterRun(object request) => throw new NotImplementedException();
    }

    public abstract class TasqHandler<TProcess> : TasqHandler, ITasqHandler<TProcess>
       where TProcess : ITasq
    {
        #region TasqHandler Calls
        internal override void Initialize(object request) => Initialize((TProcess)request);
        internal override void BeforeRun(object request) => BeforeRun((TProcess)request);
        internal override object Run(object key, object request)
        {
            Run((TProcess)request);

            return null;
        }
        internal override void AfterRun(object request) => AfterRun((TProcess)request);


        #endregion

        public virtual void AfterRun(TProcess request) { }

        public virtual void BeforeRun(TProcess request) { }

        public virtual void Initialize(TProcess request) { }

        public abstract void Run(TProcess request);
    }

    public abstract class TasqHandler<TProcess, TResponse> : TasqHandler, ITasqHandler<TProcess, TResponse>
        where TProcess : ITasq<TResponse>
    {
        #region TasqHandler Calls
        internal override void Initialize(object request) => Initialize((TProcess)request);
        internal override void BeforeRun(object request) => BeforeRun((TProcess)request);
        internal override object Run(object key, object request) => Run((TProcess)request);
        internal override void AfterRun(object request) => AfterRun((TProcess)request);
        #endregion

        public virtual void Initialize(TProcess request) { }
        public virtual void AfterRun(TProcess request) { }
        public abstract TResponse Run(TProcess request);
        public virtual void BeforeRun(TProcess request) { }
    }

    public abstract class TasqHandler<TProcess, TKey, TResponse> : TasqHandler, ITasqHandler<TProcess, TKey, TResponse>
        where TProcess : ITasq<TKey, TResponse>
    {
        #region TasqHandler Calls
        internal override void Initialize(object request) => Initialize((TProcess)request);
        internal override IEnumerable SelectionCriteria(object request) => SelectionCriteria((TProcess)request);
        internal override void BeforeRun(object request) => BeforeRun((TProcess)request);
        internal override object Run(object key, object request)
        {
            TKey k = default;

            if (key != null) k = (TKey)key;

            return Run(k, (TProcess)request);
        }
        internal override void AfterRun(object request) => AfterRun((TProcess)request);
        #endregion


        public virtual void Initialize(TProcess request) { }
        public abstract IEnumerable SelectionCriteria(TProcess request);
        public virtual void BeforeRun(TProcess request) { }
        public abstract TResponse Run(TKey key, TProcess request);
        public virtual void AfterRun(TProcess request) { }
    }
}