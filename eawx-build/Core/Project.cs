using System.Collections.Generic;
using System.Threading.Tasks;
using EawXBuild.Core.Exceptions;

namespace EawXBuild.Core
{
    public class Project
    {
        private readonly List<IJob> jobs = new List<IJob>();

        public void AddJob(IJob job)
        {
            if (HasJobWithName(job.GetName()))
                throw new DuplicateJobNameException(job.GetName());

            jobs.Add(job);
        }

        public Task RunJobAsync(string jobName)
        {
            var job = FindJobWithName(jobName);
            if (job == null)
                throw new JobNotFoundException(jobName);
            
            return Task.Run(() => job.Run());
        }

        private IJob FindJobWithName(string jobName)
        {
            return jobs.Find(job => job.GetName().Equals(jobName));
        }

        private bool HasJobWithName(string jobName)
        {
            return jobs.Exists(j => j.GetName().Equals(jobName));
        }
    }
}