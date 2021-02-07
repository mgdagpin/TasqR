using System;
using System.Collections.Generic;
using System.Linq;

namespace TasqR
{
    public interface IBaseTasqHandler { }
    public interface ITasqHandler : IBaseTasqHandler, IDisposable
    {
        void Initialize(object tasq);

        IEnumerable<object> SelectionCriteria(object tasq);


        void BeforeRun(object tasq);
        object Run(object key, object tasq);
        void AfterRun(object tasq);
    }
    public interface ITasqHandler<TTasq> : ITasqHandler
        where TTasq : ITasq
    {
        IEnumerable<object> ITasqHandler.SelectionCriteria(object tasq)
        {
            return null;
        }

        object ITasqHandler.Run(object key, object tasq)
        {
            Run((TTasq)tasq);

            return null;
        }

        void ITasqHandler.Initialize(object tasq)
        {
            Initialize((TTasq)tasq);
        }

        void ITasqHandler.BeforeRun(object tasq)
        {
            BeforeRun((TTasq)tasq);
        }

        void ITasqHandler.AfterRun(object tasq)
        {
            AfterRun((TTasq)tasq);
        }

        void Initialize(TTasq tasq);
        void BeforeRun(TTasq tasq);
        void Run(TTasq tasq);
        void AfterRun(TTasq tasq);
    }
    public interface ITasqHandler<TTasq, TResponse> : ITasqHandler
        where TTasq : ITasq<TResponse>
    {
        IEnumerable<object> ITasqHandler.SelectionCriteria(object tasq)
        {
            return null;
        }

        void ITasqHandler.Initialize(object tasq) => Initialize((TTasq)tasq);
        void ITasqHandler.BeforeRun(object tasq) => BeforeRun((TTasq)tasq);
        object ITasqHandler.Run(object key, object tasq) => Run((TTasq)tasq);

        void ITasqHandler.AfterRun(object tasq) => AfterRun((TTasq)tasq);

        void Initialize(TTasq tasq);
        void BeforeRun(TTasq tasq);
        TResponse Run(TTasq tasq);
        void AfterRun(TTasq tasq);
    }
    public interface ITasqHandler<TTasq, TKey, TResponse> : ITasqHandler
        where TTasq : ITasq<TKey, TResponse>
    {
        IEnumerable<object> ITasqHandler.SelectionCriteria(object tasq)
        {
            return SelectionCriteria((TTasq)tasq)
                .Select(a => (object)a);
        }

        object ITasqHandler.Run(object key, object tasq) => Run(key == null ? default : (TKey)key, (TTasq)tasq);

        void ITasqHandler.Initialize(object tasq) => Initialize((TTasq)tasq);
        void ITasqHandler.BeforeRun(object tasq) => BeforeRun((TTasq)tasq);
        void ITasqHandler.AfterRun(object tasq) => AfterRun((TTasq)tasq);

        IEnumerable<TKey> SelectionCriteria(TTasq tasq);

        void Initialize(TTasq tasq);
        void BeforeRun(TTasq tasq);

        TResponse Run(TKey key, TTasq tasq);
        void AfterRun(TTasq tasq);
    }
}