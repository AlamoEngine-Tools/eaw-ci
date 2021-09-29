using EawXBuild.Reporting;
namespace EawXBuild.Core
{
    public interface ITask
    {
        string Id { get; set; }

        string Name { get; set; }

        void Run(Report? report = null);
    }
}