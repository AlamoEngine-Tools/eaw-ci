using System;
using System.Collections.Generic;
using EawXBuild.Exceptions;

namespace EawXBuild.Core {
    public class Project : IProject {
        private readonly List<IJob> jobs = new List<IJob>();

        public string Name { get; set; }

        public void AddJob(IJob job) {
            if (HasJobWithName(job.Name))
                throw new DuplicateJobNameException(job.Name);

            jobs.Add(job);
        }

        public System.Threading.Tasks.Task RunJobAsync(string jobName) {
            var job = FindJobWithName(jobName);
            if (job == null)
                throw new JobNotFoundException(jobName);

            return System.Threading.Tasks.Task.Run(RunTaskAction(job));
        }

        private static Action RunTaskAction(IJob job) {
            return () => {
                try {
                    job.Run();
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            };
        }

        private IJob FindJobWithName(string jobName) {
            return jobs.Find(job => job.Name.Equals(jobName));
        }

        private bool HasJobWithName(string jobName) {
            return jobs.Exists(j => j.Name.Equals(jobName));
        }
    }
}