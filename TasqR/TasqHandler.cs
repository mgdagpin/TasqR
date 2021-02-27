using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace TasqR
{
    public abstract class TasqHandler : ITasqHandler
    {
        internal CancellationToken p_CancellationToken { get; set; }

        internal virtual object AfterRun(object tasq) => throw new NotImplementedException();
        internal virtual object BeforeRun(object tasq) => throw new NotImplementedException();
        internal virtual object Initialize(object tasq) => throw new NotImplementedException();
        internal virtual object Run(object key, object tasq) => throw new NotImplementedException();
        internal virtual IEnumerable SelectionCriteria(object tasq) => throw new NotImplementedException();
    }

    public abstract class TasqHandler<TProcess> : TasqHandler, ITasqHandler<TProcess>
       where TProcess : ITasq
    {
        #region TasqHandler Calls
        internal override object BeforeRun(object tasq)
        {
            BeforeRun((TProcess)tasq);

            return null;
        }

        internal override object AfterRun(object tasq)
        {
            AfterRun((TProcess)tasq);

            return null;
        }

        internal override object Initialize(object tasq)
        {
            Initialize((TProcess)tasq);

            return null;
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

        public abstract void Run(TProcess request);
    }

    public abstract class TasqHandler<TProcess, TResponse> : TasqHandler, ITasqHandler<TProcess, TResponse>
        where TProcess : ITasq<TResponse>
    {
        #region TasqHandler Calls
        internal override object BeforeRun(object tasq)
        {
            return BeforeRun((TProcess)tasq);
        }

        internal override object AfterRun(object tasq)
        {
            return AfterRun((TProcess)tasq);
        }

        internal override object Initialize(object tasq)
        {
            return Initialize((TProcess)tasq);
        }

        internal override object Run(object key, object tasq)
        {
            return Run((TProcess)tasq);
        }
        #endregion

        public virtual TResponse AfterRun(TProcess tasq) => default;

        public virtual TResponse BeforeRun(TProcess tasq) => default;

        public virtual TResponse Initialize(TProcess tasq) => default;

        public abstract TResponse Run(TProcess request);
    }

    public abstract class TasqHandler<TProcess, TKey, TResponse> : TasqHandler, ITasqHandler<TProcess, TKey, TResponse>
        where TProcess : ITasq<TKey, TResponse>
    {
        #region TasqHandler Calls
        internal override object BeforeRun(object tasq)
        {
            return BeforeRun((TProcess)tasq);
        }

        internal override object AfterRun(object tasq)
        {
            return AfterRun((TProcess)tasq);
        }

        internal override object Initialize(object tasq)
        {
            return Initialize((TProcess)tasq);
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


        public virtual TResponse Initialize(TProcess tasq) => default;

        public abstract IEnumerable SelectionCriteria(TProcess tasq);

        public virtual TResponse BeforeRun(TProcess tasq) => default;

        public abstract TResponse Run(TKey key, TProcess request);


        public virtual TResponse AfterRun(TProcess tasq) => default;
    }
}