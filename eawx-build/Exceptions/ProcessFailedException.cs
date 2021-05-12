using System;

namespace EawXBuild.Exceptions
{
    public class ProcessFailedException : Exception
    {
        public ProcessFailedException(string message = null) : base(message)
        {
        }
    }
}