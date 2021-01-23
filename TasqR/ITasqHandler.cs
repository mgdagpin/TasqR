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
        void Initialize();
        bool ReadKey(out object tasqKey);
        void BeforeRun();
        object Run(object key, object tasq);
        void AfterRun();
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

        void Run(TTasq tasq);
    }

    public interface IJobTasqHandler<TTasq, TResponse> : IJobTasqHandler
        where TTasq : ITasq<TResponse>
    {
        bool IJobTasqHandler.ReadKey(out object tasqKey)
        {
            tasqKey = null;
            return false;
        }
        object IJobTasqHandler.Run(object key, object tasq) => Run((TTasq)tasq);

        TResponse Run(TTasq tasq);
    }


    public interface IJobTasqHandler<Ttasq, TKey, TResponse> : IJobTasqHandler
        where Ttasq : ITasq<TKey, TResponse>
    {
        bool IJobTasqHandler.ReadKey(out object tasqKey)
        {
            bool res = ReadKey(out TKey result);
            tasqKey = result;

            return res;
        }
        object IJobTasqHandler.Run(object key, object tasq) => Run((TKey)key, (Ttasq)tasq);

        bool ReadKey(out TKey tasqKey);
        TResponse Run(TKey key, Ttasq tasq);
    }

}
