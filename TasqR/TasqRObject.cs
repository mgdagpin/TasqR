using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TasqR
{
    public class TasqRObject : ITasqR
    {
        public Guid ID { get; private set; }
        public IEnumerable<TypeTasqReference> RegisteredReferences => p_TasqHandlerResolver.RegisteredReferences;

        #region Events
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

        public TasqRObject(ITasqHandlerResolver tasqHandlerResolver)
        {
            p_TasqHandlerResolver = tasqHandlerResolver;
            ID = Guid.NewGuid();
        }

        #region Run (No return)
        public void Run(ITasq tasq)
        {
            var tasqType = tasq.GetType();

            var resolvedHandler = p_TasqHandlerResolver.ResolveHandler(tasqType);
            TasqHandler tasqHandlerInstance = (TasqHandler)resolvedHandler;

            RunImpl(tasqHandlerInstance, tasq);
        }

        public async Task RunAsync(ITasq tasq, CancellationToken cancellationToken = default)
        {
            var tasqType = tasq.GetType();

            var resolvedHandler = p_TasqHandlerResolver.ResolveHandler(tasqType);
            TasqHandler tasqHandlerInstance = (TasqHandler)resolvedHandler;

            await Task.Run(() =>
            {
                RunImpl(tasqHandlerInstance, tasq);
            });
        }

        private void RunImpl(TasqHandler tasqHandlerInstance, ITasq tasq)
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
        #endregion

        #region Run (with return)
        public TResponse Run<TResponse>(ITasq<TResponse> tasq)
        {
            var tasqType = tasq.GetType();

            var resolvedHandler = p_TasqHandlerResolver.ResolveHandler(tasqType);
            TasqHandler tasqHandlerInstance = (TasqHandler)resolvedHandler;

            return RunImpl<TResponse>(tasqHandlerInstance, tasq);
        }

        public async Task<TResponse> RunAsync<TResponse>(ITasq<TResponse> tasq, CancellationToken cancellationToken = default)
        {
            var tasqType = tasq.GetType();

            var resolvedHandler = p_TasqHandlerResolver.ResolveHandler(tasqType);
            TasqHandler tasqHandlerInstance = (TasqHandler)resolvedHandler;

            return await Task.Run(() =>
            {
                return RunImpl<TResponse>(tasqHandlerInstance, tasq);
            });
        }

        private TResponse RunImpl<TResponse>(TasqHandler tasqHandlerInstance, ITasq tasq)
        {
            TResponse retVal;

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

            return retVal;
        }
        #endregion

        #region Run (with key)
        public TResponse Run<TKey, TResponse>(ITasq<TKey, TResponse> tasq)
        {
            var tasqType = tasq.GetType();

            var resolvedHandler = p_TasqHandlerResolver.ResolveHandler(tasqType);
            TasqHandler tasqHandlerInstance = (TasqHandler)resolvedHandler;

            return RunImpl<TKey, TResponse>(tasqHandlerInstance, tasq);
        }

        public async Task<TResponse> RunAsync<TKey, TResponse>
            (
                ITasq<TKey, TResponse> tasq,
                CancellationToken cancellationToken = default
            )
        {
            var tasqType = tasq.GetType();

            var resolvedHandler = p_TasqHandlerResolver.ResolveHandler(tasqType);
            TasqHandler tasqHandlerInstance = (TasqHandler)resolvedHandler;

            return await Task.Run(() =>
            {
                return RunImpl<TKey, TResponse>(tasqHandlerInstance, tasq);
            });
        }

        private TResponse RunImpl<TKey, TResponse>(TasqHandler tasqHandlerInstance, ITasq tasq)
        {
            TResponse retVal = default;

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
                    retVal = TasqProcessEventHandler.Invoke
                    (
                        startEvent: OnRunExecuting,
                        method: () => (TResponse)tasqHandlerInstance.Run(eachSelection, tasq),
                        tasq: tasq,
                        endEvent: OnRunExecuted
                    );
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