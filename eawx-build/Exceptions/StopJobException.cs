using System;
using System.Runtime.Serialization;

namespace EawXBuild.Exceptions
{
    [Serializable]
    class StopJobException : Exception
    {
        public StopJobException()
        {
        }

        public StopJobException(string message) : base(message)
        {
        }

        public StopJobException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected StopJobException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
