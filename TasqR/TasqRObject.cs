using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TasqR.Common;

namespace TasqR
{
    public partial class TasqRObject : ITasqR
    {
        public Guid ID { get; private set; }

        public IEnumerable<TypeTasqReference> RegisteredReferences => p_TasqHandlerResolver.RegisteredReferences;

        public event LogEventHandler OnLog;

        private readonly ITasqHandlerResolver p_TasqHandlerResolver;
        internal TasqHandlerDetail ForcedHandlerDetail;

        public TasqRObject(ITasqHandlerResolver tasqHandlerResolver)
        {
            p_TasqHandlerResolver = tasqHandlerResolver;
            ID = Guid.NewGuid();
        }

        public ITasqR UsingAsHandler(Type type)
        {
            ForcedHandlerDetail = TasqHandlerDetail.TryGetFromType(type, p_TasqHandlerResolver);

            return this;
        }

        public ITasqR UsingAsHandler<THandler>() where THandler : ITasqHandler
        {
            return UsingAsHandler(typeof(THandler));
        }


        #region Run (No return)
        public void Run
            (
                ITasq tasq
            )
        {
            OnLog?.Invoke(this, TasqProcess.Start, new LogEventHandlerEventArgs(tasq));

            var resolvedHandler = GetHandlerDetail(tasq);

            OnLog?.Invoke(this, TasqProcess.Start, new LogEventHandlerEventArgs(resolvedHandler.Handler));

            if (resolvedHandler.Handler is TasqHandlerAsync)
            {
                RunAsync(tasq).Wait();
            }
            else
            {
                var tasqHandlerInstance = (TasqHandler)resolvedHandler.Handler;

                if (resolvedHandler.Reference.HandlerInterface.IsGenericType
                    && resolvedHandler.Reference.HandlerInterface.GetGenericArguments().Length == 3)
                {
                    var argT1 = resolvedHandler.Reference.HandlerInterface.GetGenericArguments()[1];
                    var argT2 = resolvedHandler.Reference.HandlerInterface.GetGenericArguments()[2];

                    var method = this.GetType().GetMethods().Single(a => a.Name == nameof(Run) && a.GetGenericArguments().Length == 2);

                    method.MakeGenericMethod(argT1, argT2).Invoke(this, parameters: new object[] { tasq });
                }
                else if (resolvedHandler.Reference.HandlerInterface.IsGenericType
                    && resolvedHandler.Reference.HandlerInterface.GetGenericArguments().Length == 2)
                {
                    var argT1 = resolvedHandler.Reference.HandlerInterface.GetGenericArguments()[1];

                    var method = this.GetType().GetMethods().Single(a => a.Name == nameof(Run) && a.GetGenericArguments().Length == 1);

                    method.MakeGenericMethod(argT1).Invoke(this, parameters: new object[] { tasq });
                }
                else
                {
                    tasqHandlerInstance.Initialize(tasq);
                    tasqHandlerInstance.BeforeRun(tasq);
                    tasqHandlerInstance.Run(null, tasq);
                    tasqHandlerInstance.AfterRun(tasq);
                }
            }
        }
        #endregion

        #region Run (with return)
        public TResponse Run<TResponse>
            (
                ITasq<TResponse> tasq
            )
        {
            TResponse retVal;
            
            OnLog?.Invoke(this, TasqProcess.Start, new LogEventHandlerEventArgs(tasq));

            var resolvedHandler = GetHandlerDetail(tasq);
            
            OnLog?.Invoke(this, TasqProcess.Start, new LogEventHandlerEventArgs(resolvedHandler.Handler));

            if (resolvedHandler.Handler is TasqHandlerAsync)
            {
                retVal = RunAsync(tasq).Result;
            }
            else
            {
                TasqHandler tasqHandlerInstance = (TasqHandler)resolvedHandler.Handler;


                tasqHandlerInstance.Initialize(tasq);
                tasqHandlerInstance.BeforeRun(tasq);
                retVal = (TResponse)tasqHandlerInstance.Run(null, tasq);
                tasqHandlerInstance.AfterRun(tasq);
            }

            return retVal;
        }
        #endregion

        #region Run (with key)
        public IEnumerable<TResponse> Run<TKey, TResponse>
            (
                ITasq<TKey, TResponse> tasq
            )
        {
            OnLog?.Invoke(this, TasqProcess.Start, new LogEventHandlerEventArgs(tasq));

            var resolvedHandler = GetHandlerDetail(tasq);

            OnLog?.Invoke(this, TasqProcess.Start, new LogEventHandlerEventArgs(resolvedHandler.Handler));

            if (resolvedHandler.Handler is TasqHandlerAsync)
            {
                return RunAsync(tasq).Result;
            }

            TasqHandler tasqHandlerInstance = (TasqHandler)resolvedHandler.Handler;

            List<TResponse> retVal = new List<TResponse>();

            tasqHandlerInstance.Initialize(tasq);
            tasqHandlerInstance.BeforeRun(tasq);

            var selectionCriteria = tasqHandlerInstance.SelectionCriteria(tasq);

            if (selectionCriteria != null)
            {
                foreach (var eachSelection in selectionCriteria)
                {
                    var result = (TResponse)tasqHandlerInstance.Run(eachSelection, tasq);

                    retVal.Add(result);
                }
            }

            tasqHandlerInstance.AfterRun(tasq);


            return retVal;
        }
        #endregion

        public Type GetHandlerType(ITasq tasq)
        {
            var resolvedHandler = GetHandlerDetail(tasq);

            return resolvedHandler.Reference.HandlerImplementation;
        }

        private TasqHandlerDetail GetHandlerDetail(ITasq tasq)
        {
            var tasqType = tasq.GetType();

            var handlerDetail = ForcedHandlerDetail != null
                ? ForcedHandlerDetail
                : p_TasqHandlerResolver.ResolveHandler(tasqType);

            ForcedHandlerDetail = null;

            return handlerDetail;
        }
    }
}