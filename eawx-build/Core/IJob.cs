using EawXBuild.Reporting;
namespace EawXBuild.Core
{
    public interface IJob
    {
        string Name { get; }

        void AddTask(ITask task);

        void Run(Report? report = null);
    }
}