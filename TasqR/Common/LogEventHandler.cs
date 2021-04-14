using System;
using System.Collections.Generic;
using System.Text;

namespace TasqR
{
    public class LogEventHandlerArgs : EventArgs
    {
        public string Message { get; set; }

        public Exception Exception { get; set; }

        internal LogEventHandlerArgs(ITasqHandler handler) 
        {
            Message = $"Handler: {handler.GetType().FullName}";
        }

        internal LogEventHandlerArgs(ITasq tasq) 
        {
            Message = $"Tasq: {tasq.GetType().FullName}";
        }

        internal LogEventHandlerArgs(ITasqHandler handler, Exception exception)
            : this(handler)
        {
            Exception = exception;
        }

        internal LogEventHandlerArgs(ITasq tasq, Exception exception)
            : this(tasq)
        {
            Exception = exception;
        }
    }

    public delegate void LogEventHandler(object sender, LogEventHandlerArgs args);
}
