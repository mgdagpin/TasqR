using System;
using System.Collections.Generic;
using System.Text;

namespace TasqR
{
    public class LogEventHandlerEventArgs : EventArgs
    {
        public string Message { get; set; }

        public Exception Exception { get; set; }

        internal LogEventHandlerEventArgs(string message) 
        { 
            Message = message; 
        }

        internal LogEventHandlerEventArgs(ITasqHandler handler) 
            : this($"Handler: {handler.GetType().FullName}")
        {
        }

        internal LogEventHandlerEventArgs(ITasq tasq) 
            : this($"Tasq: {tasq.GetType().FullName}")
        {
        }

        internal LogEventHandlerEventArgs(ITasqHandler handler, Exception exception)
            : this(handler)
        {
            Exception = exception;
        }

        internal LogEventHandlerEventArgs(ITasq tasq, Exception exception)
            : this(tasq)
        {
            Exception = exception;
        }
    }

    public delegate void LogEventHandler(object sender, TasqProcess process, LogEventHandlerEventArgs args);
}
