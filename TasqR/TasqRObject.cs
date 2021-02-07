using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TasqR
{
    public class TasqRObject : ITasqR
    {
        public Guid ID { get; private set; }

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


        private readonly ITasqHandlerResolver p_TasqHandlerResolver;

        public TasqRObject(ITasqHandlerResolver tasqHandlerResolver)
        {
            p_TasqHandlerResolver = tasqHandlerResolver;
            ID = Guid.NewGuid();
        }


        public void Run(ITasq tasq)
        {
            Task.Run(async () =>
            {
                await RunAsync(tasq);
            }).GetAwaiter().GetResult();
        }

        public TResponse Run<TResponse>(ITasq<TResponse> tasq)
        {
            return Task.Run(async () =>
            {
                return await RunAsync(tasq);
            }).GetAwaiter().GetResult();
        }

        public TResponse Run<TKey, TResponse>(ITasq<TKey, TResponse> tasq)
        {
            return Task.Run(async () =>
            {
                return await RunAsync(tasq);
            }).GetAwaiter().GetResult();
        }

        /// See <see href=" https://stackoverflow.com/a/1533349" /> for reference
        private string GetFullName(Type t)
        {
            if (!t.IsGenericType)
                return t.Name;
            var sb = new StringBuilder();

            sb.Append(t.Name.Substring(0, t.Name.LastIndexOf("`")));
            sb.Append
                (
                    t.GetGenericArguments().Aggregate
                        (
                            "<",
                            delegate (string aggregate, Type type)
                            {
                                return aggregate + (aggregate == "<" ? "" : ",") + GetFullName(type);
                            }
                        )
                );

            sb.Append(">");

            return sb.ToString();
        }




        private async Task<object> RunAsyncImplementation(ITasq tasq, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                object retVal = null;

                var tasqType = tasq.GetType();

                var tasqHandlerInstance = p_TasqHandlerResolver.ResolveHandler(tasqType);

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
                    endEvent: OnAfterRunExecuting
                );

                if (tasqHandlerInstance.GetType().HasBaseType(typeof(JobProcessHandler<,,>)))
                {
                    var selectionCriteria = TasqProcessEventHandler.Invoke
                    (
                        startEvent: OnAfterRunExecuting,
                        method: () => tasqHandlerInstance.SelectionCriteria(tasq),
                        tasq: tasq,
                        endEvent: OnAfterRunExecuted
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




        public async Task RunAsync(ITasq tasq, CancellationToken cancellationToken = default)
        {
            await Task.Run(async () =>
            {
                await RunAsyncImplementation(tasq, cancellationToken);
            });
        }

        public async Task<TResponse> RunAsync<TResponse>(ITasq<TResponse> tasq, CancellationToken cancellationToken = default)
        {
            return await Task.Run(async () =>
            {
                var result = await RunAsyncImplementation((ITasq)tasq, cancellationToken);

                return (TResponse)result;
            });
        }

        public async Task<TResponse> RunAsync<TKey, TResponse>(ITasq<TKey, TResponse> tasq, CancellationToken cancellationToken = default)
        {
            return await Task.Run(async () =>
            {
                await RunAsyncImplementation((ITasq)tasq, cancellationToken);

                return (TResponse)default;
            });
        }
    }
}