using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TasqR.Common;

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
        public void Run
            (
                ITasq tasq
            )
        {
            var tasqType = tasq.GetType();

            var resolvedHandler = p_TasqHandlerResolver.ResolveHandler(tasqType);
            var tasqHandlerInstance = (TasqHandler)resolvedHandler.Handler;

            if (resolvedHandler.Reference.HandlerInterface.IsGenericType
                && resolvedHandler.Reference.HandlerInterface.GetGenericArguments().Length == 3)
            {
                RunImplWithKey<object>(tasqHandlerInstance, tasq).Wait();
            }
            else if (resolvedHandler.Reference.HandlerInterface.IsGenericType
                && resolvedHandler.Reference.HandlerInterface.GetGenericArguments().Length == 2)
            {
                RunImplWithReturn<object>(tasqHandlerInstance, tasq).Wait();
            }
            else
            {
                RunImpl(tasqHandlerInstance, tasq).Wait();
            }
        }

        public Task RunAsync
            (
                ITasq tasq,
                CancellationToken cancellationToken = default
            )
        {
            var tasqType = tasq.GetType();

            var resolvedHandler = p_TasqHandlerResolver.ResolveHandler(tasqType);
            var tasqHandlerInstance = (TasqHandler)resolvedHandler.Handler;

            if (resolvedHandler.Reference.HandlerInterface.IsGenericType
                && resolvedHandler.Reference.HandlerInterface.GetGenericArguments().Length == 3)
            {
                var arg1 = resolvedHandler.Reference.HandlerInterface.GetGenericArguments()[1];
                var arg2 = resolvedHandler.Reference.HandlerInterface.GetGenericArguments()[2];

                throw new TasqException($"Cast your Tasq with {nameof(ITasq)}<{arg1.Name},{arg2.Name}>");
            }
            else if (resolvedHandler.Reference.HandlerInterface.IsGenericType
                && resolvedHandler.Reference.HandlerInterface.GetGenericArguments().Length == 2)
            {
                var arg1 = resolvedHandler.Reference.HandlerInterface.GetGenericArguments()[1];

                throw new TasqException($"Cast your Tasq with {nameof(ITasq)}<{arg1.Name}>");
            }
            else
            {
                return RunImpl(tasqHandlerInstance, tasq, cancellationToken);
            }
        }

        private Task RunImpl
            (
                TasqHandler tasqHandlerInstance,
                ITasq tasq,
                CancellationToken cancellationToken = default
            )
        {
            tasqHandlerInstance.p_CancellationToken = cancellationToken;

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

            return Task.FromResult(false);
        }
        #endregion

        #region Run (with return)
        public TResponse Run<TResponse>
            (
                ITasq<TResponse> tasq
            )
        {
            var tasqType = tasq.GetType();

            var resolvedHandler = p_TasqHandlerResolver.ResolveHandler(tasqType);
            TasqHandler tasqHandlerInstance = (TasqHandler)resolvedHandler.Handler;

            return RunImplWithReturn<TResponse>(tasqHandlerInstance, tasq).Result;
        }

        public Task<TResponse> RunAsync<TResponse>
            (
                ITasq<TResponse> tasq,
                CancellationToken cancellationToken = default
            )
        {
            var tasqType = tasq.GetType();

            var resolvedHandler = p_TasqHandlerResolver.ResolveHandler(tasqType);
            TasqHandler tasqHandlerInstance = (TasqHandler)resolvedHandler.Handler;

            return RunImplWithReturn<TResponse>(tasqHandlerInstance, tasq, cancellationToken);
        }

        private Task<TResponse> RunImplWithReturn<TResponse>
            (
                TasqHandler tasqHandlerInstance,
                ITasq tasq,
                CancellationToken cancellationToken = default
            )
        {
            TResponse retVal;
            tasqHandlerInstance.p_CancellationToken = cancellationToken;

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

            return Task.FromResult(retVal);
        }
        #endregion

        #region Run (with key)
        public TResponse Run<TKey, TResponse>
            (
                ITasq<TKey, TResponse> tasq
            )
        {
            var tasqType = tasq.GetType();

            var resolvedHandler = p_TasqHandlerResolver.ResolveHandler(tasqType);
            TasqHandler tasqHandlerInstance = (TasqHandler)resolvedHandler.Handler;

            return RunImplWithKey<TResponse>(tasqHandlerInstance, tasq).Result;
        }

        public Task<TResponse> RunAsync<TKey, TResponse>
            (
                ITasq<TKey, TResponse> tasq,
                CancellationToken cancellationToken = default
            )
        {
            var tasqType = tasq.GetType();

            var resolvedHandler = p_TasqHandlerResolver.ResolveHandler(tasqType);
            TasqHandler tasqHandlerInstance = (TasqHandler)resolvedHandler.Handler;

            return RunImplWithKey<TResponse>(tasqHandlerInstance, tasq, cancellationToken);
        }

        private Task<TResponse> RunImplWithKey<TResponse>
            (
                TasqHandler tasqHandlerInstance,
                ITasq tasq,
                CancellationToken cancellationToken = default
            )
        {
            TResponse retVal = default;
            tasqHandlerInstance.p_CancellationToken = cancellationToken;

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


            return Task.FromResult(retVal);
        }
        #endregion
    }
}