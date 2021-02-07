using System;
using System.Diagnostics;

namespace TasqR
{
    public class ProcessEventArgs : EventArgs
    {
        public long ElapsedTime { get; private set; }
        private Stopwatch p_Stopwatch;

        internal ProcessEventArgs() { }

        protected internal virtual void StartStopwatch()
        {
            p_Stopwatch = new Stopwatch();
            p_Stopwatch.Start();
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


    public static class TasqProcessEventHandler
    {
        /// <summary>
        /// This is a helper to wrap the method with events (before and after execution)
        /// </summary>
        /// <param name="startEvent"></param>
        /// <param name="method"></param>
        /// <param name="tasq"></param>
        /// <param name="endEvent"></param>
        public static void Invoke(ProcessEventHandler startEvent, Action method, ITasq tasq, ProcessEventHandler endEvent)
        {
            var eventArgs = new ProcessEventArgs();

            eventArgs.StartStopwatch();
            startEvent?.Invoke(tasq, eventArgs);

            method.Invoke();

            eventArgs.StopStopwatch();
            endEvent?.Invoke(tasq, eventArgs);
        }

        /// <summary>
        /// This is a helper to wrap the method with events (before and after execution)
        /// </summary>
        /// <param name="startEvent"></param>
        /// <param name="method"></param>
        /// <param name="tasq"></param>
        /// <param name="endEvent"></param>
        public static TReturn Invoke<TReturn>(ProcessEventHandler startEvent, Func<TReturn> method, ITasq tasq, ProcessEventHandler endEvent)
        {
            var eventArgs = new ProcessEventArgs();

            eventArgs.StartStopwatch();
            startEvent?.Invoke(tasq, eventArgs);

            var retVal = method.Invoke();

            eventArgs.StopStopwatch();
            endEvent?.Invoke(tasq, eventArgs);

            return retVal;
        }
    }
}
