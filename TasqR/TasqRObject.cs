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

        #region Events
        public event LogEventHandler OnLog;

        public event ProcessEventHandler OnInitializeExecuting;
        public event ProcessEventHandler OnInitializeExecuted;

        public event ProcessEventHandler OnSelectionCriteriaExecuting;
        public event ProcessEventHandler OnSelectionCriteriaExecuted;


        public event ProcessEventHandler OnBeforeRunExecuting;
        public event ProcessEventHandler OnBeforeRunExecuted;

        public event ProcessEventHandler OnRunExecuting;
        public event ProcessEventHandler OnRunExecuted;

        public event ProcessEventHandler OnAfterRunExecuting;
        public event ProcessEventHandler OnAfterRunExecuted;
        #endregion

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
            var tasqType = tasq.GetType();
            OnLog?.Invoke(this, new LogEventHandlerArgs(tasq));

            var resolvedHandler = ForcedHandlerDetail != null 
                ? ForcedHandlerDetail
                : p_TasqHandlerResolver.ResolveHandler(tasqType);

            ForcedHandlerDetail = null;

            OnLog?.Invoke(this, new LogEventHandlerArgs(resolvedHandler.Handler));

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
                    TasqProcessEventHandler.Invoke
                    (
                        startEvent: OnInitializeExecuting,
                        method: () => tasqHandlerInstance.Initialize(tasq),
                        tasq: tasq,
                        endEvent: OnInitializeExecuted
                    );

                    TasqProcessEventHandler.Invoke
                    (
                        startEvent: OnBeforeRunExecuting,
                        method: () => tasqHandlerInstance.BeforeRun(tasq),
                        tasq: tasq,
                        endEvent: OnBeforeRunExecuted
                    );

                    TasqProcessEventHandler.Invoke
                        (
                            startEvent: OnRunExecuting,
                            method: () => tasqHandlerInstance.Run(null, tasq),
                            tasq: tasq,
                            endEvent: OnRunExecuted
                        );

                    TasqProcessEventHandler.Invoke
                    (
                        startEvent: OnAfterRunExecuting,
                        method: () => tasqHandlerInstance.AfterRun(tasq),
                        tasq: tasq,
                        endEvent: OnAfterRunExecuted
                    );
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
            var tasqType = tasq.GetType();
            OnLog?.Invoke(this, new LogEventHandlerArgs(tasq));

            var resolvedHandler = ForcedHandlerDetail != null
                ? ForcedHandlerDetail
                : p_TasqHandlerResolver.ResolveHandler(tasqType);

            ForcedHandlerDetail = null;

            OnLog?.Invoke(this, new LogEventHandlerArgs(resolvedHandler.Handler));

            if (resolvedHandler.Handler is TasqHandlerAsync)
            {
                retVal = RunAsync(tasq).Result;
            }
            else
            {
                TasqHandler tasqHandlerInstance = (TasqHandler)resolvedHandler.Handler;


                TasqProcessEventHandler.Invoke
                (
                    startEvent: OnInitializeExecuting,
                    method: () => tasqHandlerInstance.Initialize(tasq),
                    tasq: tasq,
                    endEvent: OnInitializeExecuted
                );

                TasqProcessEventHandler.Invoke
                (
                    startEvent: OnBeforeRunExecuting,
                    method: () => tasqHandlerInstance.BeforeRun(tasq),
                    tasq: tasq,
                    endEvent: OnBeforeRunExecuted
                );

                retVal = TasqProcessEventHandler.Invoke
                    (
                        startEvent: OnRunExecuting,
                        method: () => (TResponse)tasqHandlerInstance.Run(null, tasq),
                        tasq: tasq,
                        endEvent: OnRunExecuted
                    );

                TasqProcessEventHandler.Invoke
                (
                    startEvent: OnAfterRunExecuting,
                    method: () => tasqHandlerInstance.AfterRun(tasq),
                    tasq: tasq,
                    endEvent: OnAfterRunExecuted
                );
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
            var tasqType = tasq.GetType();
            OnLog?.Invoke(this, new LogEventHandlerArgs(tasq));

            var resolvedHandler = ForcedHandlerDetail != null
                ? ForcedHandlerDetail
                : p_TasqHandlerResolver.ResolveHandler(tasqType);

            ForcedHandlerDetail = null;

            OnLog?.Invoke(this, new LogEventHandlerArgs(resolvedHandler.Handler));

            if (resolvedHandler.Handler is TasqHandlerAsync)
            {
                return RunAsync(tasq).Result;
            }

            TasqHandler tasqHandlerInstance = (TasqHandler)resolvedHandler.Handler;

            List<TResponse> retVal = new List<TResponse>();

            TasqProcessEventHandler.Invoke
            (
                startEvent: OnInitializeExecuting,
                method: () => tasqHandlerInstance.Initialize(tasq),
                tasq: tasq,
                endEvent: OnInitializeExecuted
            );

            TasqProcessEventHandler.Invoke
            (
                startEvent: OnBeforeRunExecuting,
                method: () => tasqHandlerInstance.BeforeRun(tasq),
                tasq: tasq,
                endEvent: OnBeforeRunExecuted
            );

            var selectionCriteria = TasqProcessEventHandler.Invoke
                (
                    startEvent: OnSelectionCriteriaExecuting,
                    method: () => tasqHandlerInstance.SelectionCriteria(tasq),
                    tasq: tasq,
                    endEvent: OnSelectionCriteriaExecuted
                );

            if (selectionCriteria != null)
            {
                foreach (var eachSelection in selectionCriteria)
                {
                    var result = TasqProcessEventHandler.Invoke
                    (
                        startEvent: OnRunExecuting,
                        method: () => (TResponse)tasqHandlerInstance.Run(eachSelection, tasq),
                        tasq: tasq,
                        endEvent: OnRunExecuted
                    );

                    retVal.Add(result);
                }
            }

            TasqProcessEventHandler.Invoke
            (
                startEvent: OnAfterRunExecuting,
                method: () => tasqHandlerInstance.AfterRun(tasq),
                tasq: tasq,
                endEvent: OnAfterRunExecuted
            );


            return retVal;
        }     
        #endregion
    }
}