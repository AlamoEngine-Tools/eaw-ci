using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EawXBuild.Exceptions;
using EawXBuild.Reporting;

namespace EawXBuild.Core
{
    public class Project : IProject
    {
        private readonly List<IJob> _jobs = new List<IJob>();

        public string Name { get; set; } = "";

        public void AddJob(IJob job)
        {
            if (HasJobWithName(job.Name))
                throw new DuplicateJobNameException(job.Name);

            _jobs.Add(job);
        }

        public Task RunJobAsync(string jobName, Report report)
        {
            var job = FindJobWithName(jobName);
            if (job == null)
                throw new JobNotFoundException(jobName);

            return Task.Run(() =>
            {
                Report(report, $"Starting job \"{jobName}\"");
                job.Run(report);
                Report(report, $"Finished job \"{jobName}\"");
            });
        }

        public List<Task> RunAllJobsAsync()
        {
            return _jobs.Select(job => Task.Run(() => job.Run())).ToList();
        }

        private IJob? FindJobWithName(string jobName)
        {
            return _jobs.Find(job => job.Name.Equals(jobName));
        }

        private bool HasJobWithName(string jobName)
        {
            return _jobs.Exists(j => j.Name.Equals(jobName));
        }

        private void Report(Report report, string messageContent)
        {
            report?.AddMessage(new Message(messageContent));
        }
    }
}