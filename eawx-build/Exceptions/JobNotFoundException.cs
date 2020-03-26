namespace EawXBuild.Core.Exceptions
{
    [System.Serializable]
    public class JobNotFoundException : System.Exception
    {
        public JobNotFoundException(string jobName) : base($"No Job with name {jobName}") { }
    }
}