using System.Collections.Generic;
using EawXBuild.Core;
using Task = System.Threading.Tasks.Task;

namespace EawXBuildTest.Core
{
    public class ProjectStub : IProject
    {
        private List<IJob> _jobs = new List<IJob>();

        public List<IJob> Jobs => _jobs;

        public string Name { get; set; }

        public void AddJob(IJob job)
        {
            _jobs.Add(job);
        }

        public System.Threading.Tasks.Task RunJobAsync(string jobName)
        {
            return System.Threading.Tasks.Task.CompletedTask;
        }
    }
}