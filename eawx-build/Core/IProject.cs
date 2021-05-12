using System.Collections.Generic;
using System.Threading.Tasks;

namespace EawXBuild.Core
{
    public interface IProject
    {
        string Name { get; set; }
        void AddJob(IJob job);

        Task RunJobAsync(string jobName);
        List<Task> RunAllJobsAsync();
    }
}