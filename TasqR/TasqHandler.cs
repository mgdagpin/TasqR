using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace TasqR
{
    public abstract class TasqHandler : ITasqHandler
    {
        internal CancellationToken p_CancellationToken { get; set; }

        internal virtual object AfterRun(object request) => throw new NotImplementedException();
        internal virtual object BeforeRun(object request) => throw new NotImplementedException();
        internal virtual object Initialize(object request) => throw new NotImplementedException();
        internal virtual object Run(object key, object request) => throw new NotImplementedException();
        internal virtual IEnumerable SelectionCriteria(object request) => throw new NotImplementedException();
    }

    public abstract class TasqHandler<TProcess> : TasqHandler, ITasqHandler<TProcess>
       where TProcess : ITasq
    {
        #region TasqHandler Calls
        internal override object BeforeRun(object request)
        {
            BeforeRun((TProcess)request);

            return null;
        }

        internal override object AfterRun(object request)
        {
            AfterRun((TProcess)request);

            return null;
        }

        internal override object Initialize(object request)
        {
            Initialize((TProcess)request);

            return null;
        }

        internal override object Run(object key, object request)
        {
            Run((TProcess)request);

            return null;
        }
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
        internal override object BeforeRun(object request)
        {
            return BeforeRun((TProcess)request);
        }

        internal override object AfterRun(object request)
        {
            return AfterRun((TProcess)request);
        }

        internal override object Initialize(object request)
        {
            return Initialize((TProcess)request);
        }

        internal override object Run(object key, object request)
        {
            return Run((TProcess)request);
        }
        #endregion

        public virtual TResponse AfterRun(TProcess request) => default;

        public virtual TResponse BeforeRun(TProcess request) => default;

        public virtual TResponse Initialize(TProcess request) => default;

        public abstract TResponse Run(TProcess request);
    }

    public abstract class TasqHandler<TProcess, TKey, TResponse> : TasqHandler, ITasqHandler<TProcess, TKey, TResponse>
        where TProcess : ITasq<TKey, TResponse>
    {
        #region TasqHandler Calls
        internal override object BeforeRun(object request)
        {
            return BeforeRun((TProcess)request);
        }

        internal override object AfterRun(object request)
        {
            return AfterRun((TProcess)request);
        }

        internal override object Initialize(object request)
        {
            return Initialize((TProcess)request);
        }

        internal override object Run(object key, object request)
        {
            TKey k = default;

            if (key != null)
            {
                k = (TKey)key;
            }

            return Run(k, (TProcess)request);
        }

        internal override IEnumerable SelectionCriteria(object request)
        {
            return SelectionCriteria((TProcess)request);
        }
        #endregion


        public virtual TResponse Initialize(TProcess request) => default;
        public virtual TResponse BeforeRun(TProcess request) => default;
        public virtual TResponse AfterRun(TProcess request) => default;


        public abstract IEnumerable SelectionCriteria(TProcess request);
        public abstract TResponse Run(TKey key, TProcess request);
    }
}