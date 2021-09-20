using EawXBuild.Core;
using EawXBuild.Reporting;

namespace EawXBuildTest.Core
{
    public class TaskDummy : ITask
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public virtual void Run(Report report = null)
        {
        }
    }

    public class TaskSpy : TaskDummy
    {
        public Report Report { get; private set; }
        public bool WasRun { get; private set; }

        public override void Run(Report report = null)
        {
            WasRun = true;
            Report = report;
        }
    }

    public class ReportingTask : TaskDummy
    {
        public override void Run(Report report = null)
        {
            report?.AddMessage(new Message("TaskDummy"));
        }
    }
}