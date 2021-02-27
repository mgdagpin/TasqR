using System;
using System.Runtime.Serialization;

namespace TasqR
{
    [Serializable]
    public class TasqException : Exception
    {
        public TasqException()
        {

        }

        public TasqException(string message)
            : base(message)
        {

        }

        public TasqException(Exception exception)
            : base(InnermostException(exception).Message)
        {

        }

        public TasqException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected TasqException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        private static Exception InnermostException(Exception exception)
        {
            if (exception.InnerException != null)
            {
                return InnermostException(exception.InnerException);
            }

            return exception;
        }
    }
}
