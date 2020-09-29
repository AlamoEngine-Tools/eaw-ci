using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EawXBuild.Exceptions;

namespace EawXBuild.Core {
    public class Project : IProject 
    {
        private readonly List<IJob> _jobs = new List<IJob>();

        public string Name { get; set; }

        public void AddJob(IJob job) {
            if (HasJobWithName(job.Name))
                throw new DuplicateJobNameException(job.Name);
            _jobs.Add(job);
        }

        public System.Threading.Tasks.Task RunJobAsync(string jobName, CancellationToken token) {
            var job = FindJobWithName(jobName);
            if (job == null)
                throw new JobNotFoundException(jobName);
            // Do not pass token to this task, as we want the job impl. to handle cancellation.
            return System.Threading.Tasks.Task.Run(() => job.Run(token));
        }

        public List<System.Threading.Tasks.Task> RunAllJobsAsync(CancellationToken token)
        {
            // Do not pass token to this task, as we want the job impl. to handle cancellation.
            return _jobs.Select(job => System.Threading.Tasks.Task.Run(() => job.Run(token))).ToList();
        }

        private IJob FindJobWithName(string jobName) {
            return _jobs.Find(job => job.Name.Equals(jobName));
        }

        private bool HasJobWithName(string jobName) {
            return _jobs.Exists(j => j.Name.Equals(jobName));
        }
    }
}