using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasqR
{
    public interface IBaseTasqHandler { }

    public interface IJobTasqHandler : IBaseTasqHandler, IDisposable
    {
        void Initialize(object tasq);
        bool ReadKey(out object tasqKey);
        void BeforeRun(object tasq);
        object Run(object key, object tasq);
        void AfterRun(object tasq);
    }

    public interface IJobTasqHandler<in TTasq> : IJobTasqHandler
        where TTasq : ITasq
    {
        bool IJobTasqHandler.ReadKey(out object tasqKey)
        {
            tasqKey = null;
            return false;
        }

        object IJobTasqHandler.Run(object key, object tasq)
        {
            Run((TTasq)tasq);

            return null;
        }

        void IJobTasqHandler.Initialize(object tasq)
        {
            Initialize((TTasq)tasq);
        }

        void IJobTasqHandler.BeforeRun(object tasq)
        {
            BeforeRun((TTasq)tasq);
        }

        void IJobTasqHandler.AfterRun(object tasq)
        {
            AfterRun((TTasq)tasq);
        }

        void Initialize(TTasq tasq);
        void BeforeRun(TTasq tasq);
        void Run(TTasq tasq);
        void AfterRun(TTasq tasq);
    }

    public interface IJobTasqHandler<TTasq, TResponse> : IJobTasqHandler
        where TTasq : ITasq<TResponse>
    {
        bool IJobTasqHandler.ReadKey(out object tasqKey)
        {
            tasqKey = null;
            return false;
        }
        void IJobTasqHandler.Initialize(object tasq) => Initialize((TTasq)tasq);
        void IJobTasqHandler.BeforeRun(object tasq) => BeforeRun((TTasq)tasq);
        object IJobTasqHandler.Run(object key, object tasq) => Run((TTasq)tasq);
        void IJobTasqHandler.AfterRun(object tasq) => AfterRun((TTasq)tasq);

        void Initialize(TTasq tasq);
        void BeforeRun(TTasq tasq);
        TResponse Run(TTasq tasq);
        void AfterRun(TTasq tasq);
    }


    public interface IJobTasqHandler<TTasq, TKey, TResponse> : IJobTasqHandler
        where TTasq : ITasq<TKey, TResponse>
    {
        bool IJobTasqHandler.ReadKey(out object tasqKey)
        {
            bool res = ReadKey(out TKey result);
            tasqKey = result;

            return res;
        }
        object IJobTasqHandler.Run(object key, object tasq) => Run((TKey)key, (TTasq)tasq);

        void IJobTasqHandler.Initialize(object tasq) => Initialize((TTasq)tasq);
        void IJobTasqHandler.BeforeRun(object tasq) => BeforeRun((TTasq)tasq);
        void IJobTasqHandler.AfterRun(object tasq) => AfterRun((TTasq)tasq);

        bool ReadKey(out TKey tasqKey);

        void Initialize(TTasq tasq);
        void BeforeRun(TTasq tasq);

        TResponse Run(TKey key, TTasq tasq);
        void AfterRun(TTasq tasq);
    }

}
