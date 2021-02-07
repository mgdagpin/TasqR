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

            var tasqHandlerInstance = p_TasqHandlerResolver.ResolveHandler(tasqType);

            Task.Run(async () =>
            {
                await RunAsyncImplementation(tasqHandlerInstance, tasq);
            }).GetAwaiter().GetResult();
        }

        public async Task RunAsync(ITasq tasq, CancellationToken cancellationToken = default)
        {
            var tasqType = tasq.GetType();

            var tasqHandlerInstance = p_TasqHandlerResolver.ResolveHandler(tasqType);

            await Task.Run(async () =>
            {
                await RunAsyncImplementation(tasqHandlerInstance, tasq, cancellationToken);
            });
        }
        #endregion

        #region Run (with return)
        public TResponse Run<TResponse>(ITasq<TResponse> tasq)
        {
            var tasqType = tasq.GetType();

            var tasqHandlerInstance = p_TasqHandlerResolver.ResolveHandler(tasqType);

            return Task.Run(async () =>
            {
                var ret = await RunAsyncImplementation(tasqHandlerInstance, tasq);

                return (TResponse)ret;
            }).GetAwaiter().GetResult();
        }

        public async Task<TResponse> RunAsync<TResponse>(ITasq<TResponse> tasq, CancellationToken cancellationToken = default)
        {
            var tasqType = tasq.GetType();

            var tasqHandlerInstance = p_TasqHandlerResolver.ResolveHandler(tasqType);

            return await Task.Run(async () =>
            {
                var result = await RunAsyncImplementation(tasqHandlerInstance, tasq, cancellationToken);

                return (TResponse)result;
            });
        }
        #endregion

        #region Run (with key)
        public TResponse Run<TKey, TResponse>(ITasq<TKey, TResponse> tasq)
        {
            var tasqType = tasq.GetType();

            var tasqHandlerInstance = p_TasqHandlerResolver.ResolveHandler(tasqType);

            return Task.Run(async () =>
            {
                var ret = await RunAsyncImplementation(tasqHandlerInstance, tasq);

                return (TResponse)ret;
            }).GetAwaiter().GetResult();
        }

        public async Task<TResponse> RunAsync<TKey, TResponse>(ITasq<TKey, TResponse> tasq, CancellationToken cancellationToken = default)
        {
            var tasqType = tasq.GetType();

            var tasqHandlerInstance = p_TasqHandlerResolver.ResolveHandler(tasqType);

            return await Task.Run(async () =>
            {
                var ret = await RunAsyncImplementation(tasqHandlerInstance, tasq, cancellationToken);

                return (TResponse)ret;
            });
        }
        #endregion


        private async Task<object> RunAsyncImplementation(ITasqHandler tasqHandlerInstance, ITasq tasq, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                object retVal = null;

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

                if (tasqHandlerInstance.GetType().HasBaseType(typeof(TasqHandler<,,>)))
                {
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
                                method: () => tasqHandlerInstance.Run(eachSelection, tasq),
                                tasq: tasq,
                                endEvent: OnRunExecuted
                            );
                        }
                    }
                }
                else
                {
                    retVal = TasqProcessEventHandler.Invoke
                    (
                        startEvent: OnRunExecuting,
                        method: () => tasqHandlerInstance.Run(null, tasq),
                        tasq: tasq,
                        endEvent: OnRunExecuted
                    );
                }

                TasqProcessEventHandler.Invoke
                (
                    startEvent: OnAfterRunExecuting,
                    method: () => tasqHandlerInstance.AfterRun(tasq),
                    tasq: tasq,
                    endEvent: OnAfterRunExecuted
                );

                tasqHandlerInstance.Dispose();

                return retVal;
            });
        }
    }
}