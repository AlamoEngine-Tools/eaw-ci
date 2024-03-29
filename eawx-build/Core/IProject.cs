using System.Collections.Generic;
using System.Threading.Tasks;
using EawXBuild.Reporting;

namespace EawXBuild.Core
{
    public interface IProject
    {
        string Name { get; set; }
        void AddJob(IJob job);

        Task RunJobAsync(string jobName, Report report);
        List<Task> RunAllJobsAsync();
    }
}