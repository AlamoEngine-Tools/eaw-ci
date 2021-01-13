using System;

namespace EawXBuild.Exceptions {
    [Serializable]
    public class JobNotFoundException : Exception {
        public JobNotFoundException(string jobName) : base($"No Job with name {jobName}") { }
    }
}