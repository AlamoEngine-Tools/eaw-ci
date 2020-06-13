namespace EawXBuild.Core
{
    public interface IProject
    {
        string Name { get; set; }
        void AddJob(IJob job);

        System.Threading.Tasks.Task RunJobAsync(string jobName);
    }
}