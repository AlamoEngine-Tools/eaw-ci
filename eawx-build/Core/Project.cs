using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EawXBuild.Exceptions;
using Microsoft.Extensions.Logging;

namespace EawXBuild.Core {
    public class Project : IProject {
        private readonly List<IJob> jobs = new List<IJob>();

        public string Name { get; set; }

        public void AddJob(IJob job) {
            if (HasJobWithName(job.Name))
                throw new DuplicateJobNameException(job.Name);

            jobs.Add(job);
        }

        public Task RunJobAsync(string jobName) {
            var job = FindJobWithName(jobName);
            if (job == null)
                throw new JobNotFoundException(jobName);

            Console.WriteLine($"Running job {job.Name}");
            return Task.Run(() => job.Run());
        }

        public List<Task> RunAllJobsAsync()
        {
            return jobs.Select(job => Task.Run(job.Run)).ToList();
        }

        private IJob FindJobWithName(string jobName) {
            return jobs.Find(job => job.Name.Equals(jobName));
        }

        private bool HasJobWithName(string jobName) {
            return jobs.Exists(j => j.Name.Equals(jobName));
        }
    }
}