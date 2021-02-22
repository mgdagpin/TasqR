using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace TasqR
{
    public abstract class TasqHandler : ITasqHandler
    {
        internal CancellationToken p_CancellationToken { get; set; }

        internal virtual void AfterRun(object tasq) => throw new NotImplementedException();
        internal virtual void BeforeRun(object tasq) => throw new NotImplementedException();
        internal virtual void Initialize(object tasq) => throw new NotImplementedException();
        internal virtual object Run(object key, object tasq) => throw new NotImplementedException();
        internal virtual IEnumerable SelectionCriteria(object tasq) => throw new NotImplementedException();
    }

    public abstract class TasqHandler<TProcess> : TasqHandler, ITasqHandler<TProcess>
       where TProcess : ITasq
    {
        #region TasqHandler Calls
        internal override void BeforeRun(object tasq)
        {
            BeforeRun((TProcess)tasq);
        }

        internal override void AfterRun(object tasq)
        {
            AfterRun((TProcess)tasq);
        }

        internal override void Initialize(object tasq)
        {
            Initialize((TProcess)tasq);
        }

        internal override object Run(object key, object tasq)
        {
            Run((TProcess)tasq);

            return null;
        }
        #endregion

        public virtual void AfterRun(TProcess tasq) { }

        public virtual void BeforeRun(TProcess tasq) { }

        public virtual void Initialize(TProcess tasq) { }

        public abstract void Run(TProcess process);
    }

    public abstract class TasqHandler<TProcess, TResponse> : TasqHandler, ITasqHandler<TProcess, TResponse>
        where TProcess : ITasq<TResponse>
    {
        #region TasqHandler Calls
        internal override void BeforeRun(object tasq)
        {
            BeforeRun((TProcess)tasq);
        }

        internal override void AfterRun(object tasq)
        {
            AfterRun((TProcess)tasq);
        }

        internal override void Initialize(object tasq)
        {
            Initialize((TProcess)tasq);
        }

        internal override object Run(object key, object tasq)
        {
            return Run((TProcess)tasq);
        }
        #endregion

        public virtual void AfterRun(TProcess tasq) { }

        public virtual void BeforeRun(TProcess tasq) { }

        public virtual void Initialize(TProcess tasq) { }

        public abstract TResponse Run(TProcess process);
    }

    public abstract class TasqHandler<TProcess, TKey, TResponse> : TasqHandler, ITasqHandler<TProcess, TKey, TResponse>
        where TProcess : ITasq<TKey, TResponse>
    {
        #region TasqHandler Calls
        internal override void BeforeRun(object tasq)
        {
            BeforeRun((TProcess)tasq);
        }

        internal override void AfterRun(object tasq)
        {
            AfterRun((TProcess)tasq);
        }

        internal override void Initialize(object tasq)
        {
            Initialize((TProcess)tasq);
        }

        internal override object Run(object key, object tasq)
        {
            TKey k = default;

            if (key != null)
            {
                k = (TKey)key;
            }

            return Run(k, (TProcess)tasq);
        }

        internal override IEnumerable SelectionCriteria(object tasq)
        {
            return SelectionCriteria((TProcess)tasq);
        }
        #endregion


        public virtual void Initialize(TProcess tasq)
        {

        }

        public abstract IEnumerable<TKey> SelectionCriteria(TProcess tasq);

        public virtual void BeforeRun(TProcess tasq) { }

        public abstract TResponse Run(TKey key, TProcess process);


        public virtual void AfterRun(TProcess tasq) { }
    }
}