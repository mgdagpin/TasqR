using System;
using System.Diagnostics;

namespace TasqR
{
    public class ProcessEventArgs<TResult> : ProcessEventArgs
    {
        public TResult ReturnValue { get; private set; }

        internal ProcessEventArgs(ITasqR processor) : base(processor)
        {
        }

        internal void SetReturnedValue(TResult result)
        {
            ReturnValue = result;
        }

        protected internal override ProcessEventArgs<TResult> StartStopwatch()
        {
            base.StartStopwatch();

            return this;
        }
    }

    public class ProcessEventArgs : EventArgs
    {
        public long ElapsedTime { get; private set; }
        private Stopwatch p_Stopwatch;

        public ITasqR Processor { get; private set; }

        internal ProcessEventArgs(ITasqR processor)
        {
            Processor = processor;
        }

        protected internal virtual ProcessEventArgs StartStopwatch()
        {
            p_Stopwatch = new Stopwatch();
            p_Stopwatch.Start();

            return this;
        }

        protected internal virtual void StopStopwatch()
        {
            if (p_Stopwatch != null && p_Stopwatch.IsRunning)
            {
                p_Stopwatch.Stop();
                ElapsedTime = p_Stopwatch.ElapsedMilliseconds;
                p_Stopwatch = null;
            }
        }
    }

    public delegate void ProcessEventHandler(object sender, ProcessEventArgs args);
}
