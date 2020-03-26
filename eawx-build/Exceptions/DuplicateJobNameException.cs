namespace EawXBuild.Core.Exceptions
{
    [System.Serializable]
    public class DuplicateJobNameException : System.Exception
    {
        public DuplicateJobNameException(string jobName) : base($"Duplicate Job {jobName}") { }
    }
}