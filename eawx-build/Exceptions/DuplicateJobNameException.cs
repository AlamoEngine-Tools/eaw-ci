using System;

namespace EawXBuild.Exceptions
{
    [Serializable]
    public class DuplicateJobNameException : Exception
    {
        public DuplicateJobNameException(string jobName) : base($"Duplicate Job {jobName}")
        {
        }
    }
}