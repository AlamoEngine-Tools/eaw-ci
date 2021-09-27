using System;

class ProcessNotStartedException : Exception
{
    public ProcessNotStartedException(string message) : base(message)
    { }
}