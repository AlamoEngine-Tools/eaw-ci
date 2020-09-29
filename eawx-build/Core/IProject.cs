using System.Collections.Generic;
using System.Threading;

namespace EawXBuild.Core
{
    public interface IProject
    {
        string Name { get; set; }
        void AddJob(IJob job);

        System.Threading.Tasks.Task RunJobAsync(string jobName, CancellationToken token);

        List<System.Threading.Tasks.Task> RunAllJobsAsync(CancellationToken token);
    }
}