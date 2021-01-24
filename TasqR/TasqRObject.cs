using System;
using System.Linq;
using System.Text;

namespace TasqR
{
    public class TasqRObject : ITasqR
    {
        public event ProcessEventHandler OnInitializeExecuting;
        public event ProcessEventHandler OnInitializeExecuted;
        public event ProcessEventHandler OnBeforeRunExecuting;
        public event ProcessEventHandler OnBeforeRunExecuted;
        public event ProcessEventHandler OnRunExecuting;
        public event ProcessEventHandler OnRunExecuted;
        public event ProcessEventHandler OnAfterRunExecuting;
        public event ProcessEventHandler OnAfterRunExecuted;

        private readonly ITasqHandlerCollection handlerCollection;

        public TasqRObject(ITasqHandlerCollection handlerCollection)
        {
            this.handlerCollection = handlerCollection;
        }



        public void Run(ITasq tasq)
        {
            var _tt = tasq.GetType();
            var _t = handlerCollection.TasqHanders[_tt];

            var _a = (IJobTasqHandler)handlerCollection.GetService(_t);

            if (_a == null)
            {
                throw new Exception($"Type {GetFullName(_t)} not registered");
            }

            var initializeProcessEventArgs = new ProcessEventArgs(this).StartStopwatch();
            OnInitializeExecuting?.Invoke(tasq, initializeProcessEventArgs);
            _a.Initialize(tasq);
            initializeProcessEventArgs.StopStopwatch();
            OnInitializeExecuted?.Invoke(tasq, initializeProcessEventArgs);

            var onBeforeRunProcessEventArgs = new ProcessEventArgs(this).StartStopwatch();
            OnBeforeRunExecuting?.Invoke(tasq, onBeforeRunProcessEventArgs);
            _a.BeforeRun(tasq);
            onBeforeRunProcessEventArgs.StopStopwatch();
            OnBeforeRunExecuted?.Invoke(tasq, onBeforeRunProcessEventArgs);

            if (_t.GetGenericArguments().Length == 3)
            {
                while (_a.ReadKey(out object key))
                {
                    var onRunProcessEventArgs = new ProcessEventArgs(this).StartStopwatch();
                    OnRunExecuting?.Invoke(tasq, onRunProcessEventArgs);
                    _a.Run(key, tasq);
                    onRunProcessEventArgs.StopStopwatch();
                    OnRunExecuted?.Invoke(tasq, onRunProcessEventArgs);

                }
            }
            else
            {
                var onRunProcessEventArgs = new ProcessEventArgs(this).StartStopwatch();
                OnRunExecuting?.Invoke(tasq, onRunProcessEventArgs);
                _a.Run(null, tasq);
                onRunProcessEventArgs.StopStopwatch();
                OnRunExecuted?.Invoke(tasq, onRunProcessEventArgs);
            }

            var onAfterRunProcessEventArgs = new ProcessEventArgs(this).StartStopwatch();
            OnAfterRunExecuting?.Invoke(tasq, onAfterRunProcessEventArgs);
            _a.AfterRun(tasq);
            onAfterRunProcessEventArgs.StopStopwatch();
            OnAfterRunExecuted?.Invoke(tasq, onAfterRunProcessEventArgs);

            _a.Dispose();
        }

        public TResponse Run<TResponse>(ITasq<TResponse> tasq)
        {
            var _t = typeof(IJobTasqHandler<,>).MakeGenericType(tasq.GetType(), typeof(TResponse));

            var _a = (IJobTasqHandler)handlerCollection.GetService(_t);

            if (_a == null)
            {
                throw new Exception($"Type {GetFullName(_t)} not registered");
            }

            var initializeProcessEventArgs = new ProcessEventArgs(this).StartStopwatch();
            OnInitializeExecuting?.Invoke(tasq, initializeProcessEventArgs);
            _a.Initialize(tasq);
            initializeProcessEventArgs.StopStopwatch();
            OnInitializeExecuted?.Invoke(tasq, initializeProcessEventArgs);

            var onBeforeRunProcessEventArgs = new ProcessEventArgs(this).StartStopwatch();
            OnBeforeRunExecuting?.Invoke(tasq, onBeforeRunProcessEventArgs);
            _a.BeforeRun(tasq);
            onBeforeRunProcessEventArgs.StopStopwatch();
            OnBeforeRunExecuted?.Invoke(tasq, onBeforeRunProcessEventArgs);

            var onRunProcessEventArgs = new ProcessEventArgs<TResponse>(this).StartStopwatch();
            OnRunExecuting?.Invoke(tasq, onRunProcessEventArgs);
            var retVal = (TResponse)_a.Run(null, tasq);
            onRunProcessEventArgs.SetReturnedValue(retVal);
            onRunProcessEventArgs.StopStopwatch();
            OnRunExecuted?.Invoke(tasq, onRunProcessEventArgs);

            var onAfterRunProcessEventArgs = new ProcessEventArgs(this).StartStopwatch();
            OnAfterRunExecuting?.Invoke(tasq, onAfterRunProcessEventArgs);
            _a.AfterRun(tasq);
            onAfterRunProcessEventArgs.StopStopwatch();
            OnAfterRunExecuted?.Invoke(tasq, onAfterRunProcessEventArgs);

            _a.Dispose();

            return retVal;
        }

        public TResponse Run<TKey, TResponse>(ITasq<TKey, TResponse> tasq)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        /// See <see href=" https://stackoverflow.com/a/1533349" /> for reference
        private string GetFullName(Type t)
        {
            if (!t.IsGenericType)
                return t.Name;
            StringBuilder sb = new StringBuilder();

            sb.Append(t.Name.Substring(0, t.Name.LastIndexOf("`")));
            sb.Append(t.GetGenericArguments().Aggregate("<",

                delegate (string aggregate, Type type)
                {
                    return aggregate + (aggregate == "<" ? "" : ",") + GetFullName(type);
                }
                ));
            sb.Append(">");

            return sb.ToString();
        }
    }
}
