using System;
using System.Linq;
using System.Text;

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

        private readonly ITasqHandlerCollection p_HandlerCollection;

        public TasqRObject(ITasqHandlerCollection handlerCollection)
        {
            p_HandlerCollection = handlerCollection;
            ID = Guid.NewGuid();
        }


        public void Run(ITasq tasq)
        {
            var tasqType = tasq.GetType();
            var tasqHandlerType = p_HandlerCollection.TasqHanders[tasqType];

            var tasqHandlerInstance = (IJobTasqHandler)p_HandlerCollection.GetService(tasqHandlerType);

            if (tasqHandlerInstance == null)
            {
                throw new Exception($"Type {GetFullName(tasqHandlerType)} not registered");
            }

            var initializeProcessEventArgs = new ProcessEventArgs(this).StartStopwatch();
            OnInitializeExecuting?.Invoke(tasq, initializeProcessEventArgs);
            tasqHandlerInstance.Initialize(tasq);
            initializeProcessEventArgs.StopStopwatch();
            OnInitializeExecuted?.Invoke(tasq, initializeProcessEventArgs);

            var onBeforeRunProcessEventArgs = new ProcessEventArgs(this).StartStopwatch();
            OnBeforeRunExecuting?.Invoke(tasq, onBeforeRunProcessEventArgs);
            tasqHandlerInstance.BeforeRun(tasq);
            onBeforeRunProcessEventArgs.StopStopwatch();
            OnBeforeRunExecuted?.Invoke(tasq, onBeforeRunProcessEventArgs);

            if (tasqHandlerType.GetGenericArguments().Length == 3)
            {
                var onSelectionCriteriaProcessEventArgs = new ProcessEventArgs(this).StartStopwatch();
                OnSelectionCriteriaExecuting?.Invoke(tasq, onSelectionCriteriaProcessEventArgs);
                var selectionCriteria = tasqHandlerInstance.SelectionCriteria(tasq);
                onSelectionCriteriaProcessEventArgs.StopStopwatch();
                OnSelectionCriteriaExecuted?.Invoke(tasq, onSelectionCriteriaProcessEventArgs);

                if (selectionCriteria != null)
                {
                    foreach (var eachSelection in selectionCriteria)
                    {
                        var onRunProcessEventArgs = new ProcessEventArgs(this).StartStopwatch();
                        OnRunExecuting?.Invoke(tasq, onRunProcessEventArgs);
                        tasqHandlerInstance.Run(eachSelection, tasq);
                        onRunProcessEventArgs.StopStopwatch();
                        OnRunExecuted?.Invoke(tasq, onRunProcessEventArgs);
                    }
                }
            }
            else
            {
                var onRunProcessEventArgs = new ProcessEventArgs(this).StartStopwatch();
                OnRunExecuting?.Invoke(tasq, onRunProcessEventArgs);
                tasqHandlerInstance.Run(null, tasq);
                onRunProcessEventArgs.StopStopwatch();
                OnRunExecuted?.Invoke(tasq, onRunProcessEventArgs);
            }

            var onAfterRunProcessEventArgs = new ProcessEventArgs(this).StartStopwatch();
            OnAfterRunExecuting?.Invoke(tasq, onAfterRunProcessEventArgs);
            tasqHandlerInstance.AfterRun(tasq);
            onAfterRunProcessEventArgs.StopStopwatch();
            OnAfterRunExecuted?.Invoke(tasq, onAfterRunProcessEventArgs);

            tasqHandlerInstance.Dispose();
        }

        public TResponse Run<TResponse>(ITasq<TResponse> tasq)
        {
            var tasqHandlerType = typeof(IJobTasqHandler<,>).MakeGenericType(tasq.GetType(), typeof(TResponse));
            var tasqHandlerInstance = (IJobTasqHandler)p_HandlerCollection.GetService(tasqHandlerType);

            if (tasqHandlerInstance == null)
            {
                throw new Exception($"Type {GetFullName(tasqHandlerType)} not registered");
            }

            var initializeProcessEventArgs = new ProcessEventArgs(this).StartStopwatch();
            OnInitializeExecuting?.Invoke(tasq, initializeProcessEventArgs);
            tasqHandlerInstance.Initialize(tasq);
            initializeProcessEventArgs.StopStopwatch();
            OnInitializeExecuted?.Invoke(tasq, initializeProcessEventArgs);

            var onBeforeRunProcessEventArgs = new ProcessEventArgs(this).StartStopwatch();
            OnBeforeRunExecuting?.Invoke(tasq, onBeforeRunProcessEventArgs);
            tasqHandlerInstance.BeforeRun(tasq);
            onBeforeRunProcessEventArgs.StopStopwatch();
            OnBeforeRunExecuted?.Invoke(tasq, onBeforeRunProcessEventArgs);

            var onRunProcessEventArgs = new ProcessEventArgs<TResponse>(this).StartStopwatch();
            OnRunExecuting?.Invoke(tasq, onRunProcessEventArgs);
            var retVal = (TResponse)tasqHandlerInstance.Run(null, tasq);
            onRunProcessEventArgs.SetReturnedValue(retVal);
            onRunProcessEventArgs.StopStopwatch();
            OnRunExecuted?.Invoke(tasq, onRunProcessEventArgs);

            var onAfterRunProcessEventArgs = new ProcessEventArgs(this).StartStopwatch();
            OnAfterRunExecuting?.Invoke(tasq, onAfterRunProcessEventArgs);
            tasqHandlerInstance.AfterRun(tasq);
            onAfterRunProcessEventArgs.StopStopwatch();
            OnAfterRunExecuted?.Invoke(tasq, onAfterRunProcessEventArgs);

            tasqHandlerInstance.Dispose();

            return retVal;
        }

        public TResponse Run<TKey, TResponse>(ITasq<TKey, TResponse> tasq)
        {
            throw new NotImplementedException();
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
    }
}
