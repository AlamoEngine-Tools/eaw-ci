using System.Collections.Generic;
using EawXBuild.Exceptions;

namespace EawXBuild.Core
{
    public class Project
    {
        private readonly List<IJob> jobs = new List<IJob>();

        public void AddJob(IJob job)
        {
            if (HasJobWithName(job.Name))
                throw new DuplicateJobNameException(job.Name);

            jobs.Add(job);
        }

        public System.Threading.Tasks.Task RunJobAsync(string jobName)
        {
            var job = FindJobWithName(jobName);
            if (job == null)
                throw new JobNotFoundException(jobName);

            return System.Threading.Tasks.Task.Run(() => job.Run());
        }

        private IJob FindJobWithName(string jobName)
        {
            return jobs.Find(job => job.Name.Equals(jobName));
        }

        private bool HasJobWithName(string jobName)
        {
            return jobs.Exists(j => j.Name.Equals(jobName));
        }
    }
}