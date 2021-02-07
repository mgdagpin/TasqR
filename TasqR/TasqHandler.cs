﻿using System;
using System.Collections.Generic;

namespace TasqR
{
    public abstract class TasqHandler<TProcess> : ITasqHandler<TProcess>
       where TProcess : ITasq
    {
        public virtual void AfterRun(TProcess tasq) { }

        public virtual void BeforeRun(TProcess tasq) { }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //if (_Connection != null)
                //{
                //    _Connection.Dispose();
                //    _Connection = null;
                //}
            }
        }

        public virtual void Initialize(TProcess tasq) { }

        public abstract void Run(TProcess process);
    }

    public abstract class TasqHandler<TProcess, TResponse> : ITasqHandler<TProcess, TResponse>
        where TProcess : ITasq<TResponse>
    {
        public virtual void AfterRun(TProcess tasq) { }

        public virtual void BeforeRun(TProcess tasq) { }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //if (_Connection != null)
                //{
                //    _Connection.Dispose();
                //    _Connection = null;
                //}
            }
        }

        public virtual void Initialize(TProcess tasq) { }

        public abstract TResponse Run(TProcess process);
    }

    public abstract class TasqHandler<TProcess, TKey, TResponse> : ITasqHandler<TProcess, TKey, TResponse>
        where TProcess : ITasq<TKey, TResponse>
    {
        public virtual void Initialize(TProcess tasq)
        {

        }

        public abstract IEnumerable<TKey> SelectionCriteria(TProcess tasq);

        public virtual void BeforeRun(TProcess tasq) { }

        public abstract TResponse Run(TKey key, TProcess process);


        public virtual void AfterRun(TProcess tasq) { }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //if (_Connection != null)
                //{
                //    _Connection.Dispose();
                //    _Connection = null;
                //}
            }
        }
    }
}