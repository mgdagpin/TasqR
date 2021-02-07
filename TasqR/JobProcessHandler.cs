using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TasqR
{
    public abstract class JobProcessHandler<TProcess> : IJobTasqHandler<TProcess>
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

        public virtual void Run(TProcess process)
        {
            Task.Run(async () =>
            {
                await RunAsync(process);
            }).GetAwaiter().GetResult();
        }

        public virtual Task RunAsync(TProcess process, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class JobProcessHandler<TProcess, TResponse> : IJobTasqHandler<TProcess, TResponse>
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

    public abstract class JobProcessHandler<TProcess, TKey, TResponse> : IJobTasqHandler<TProcess, TKey, TResponse>
        where TProcess : ITasq<TKey, TResponse>
    {
        public virtual void Initialize(TProcess tasq)
        {

        }

        public abstract IEnumerable<TKey> SelectionCriteria(TProcess tasq);

        public virtual void BeforeRun(TProcess tasq) { }

        public virtual TResponse Run(TKey key, TProcess process)
        {
            return Task.Run(async () =>
            {
                return await RunAsync(key, process);
            }).GetAwaiter().GetResult();
        }

        public virtual Task<TResponse> RunAsync(TKey key, TProcess process, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


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