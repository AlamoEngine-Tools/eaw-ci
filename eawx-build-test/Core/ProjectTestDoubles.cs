using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EawXBuild.Core;

namespace EawXBuildTest.Core
{
    public class ProjectDummy : IProject
    {
        public virtual string Name { get; set; }

        public virtual void AddJob(IJob job)
        {
        }

        public virtual Task RunJobAsync(string jobName)
        {
            return null;
        }

        public List<Task> RunAllJobsAsync()
        {
            throw new NotImplementedException();
        }
    }

    public class ProjectStub : ProjectDummy
    {
        public List<IJob> Jobs { get; } = new List<IJob>();

        public override string Name { get; set; }

        public override void AddJob(IJob job)
        {
            Jobs.Add(job);
        }

        public override Task RunJobAsync(string jobName)
        {
            return Task.CompletedTask;
        }
    }
}