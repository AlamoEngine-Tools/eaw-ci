using EawXBuild.Core;

namespace EawXBuildTest.Core
{
    public class TaskDummy : ITask
    {
        public virtual void Run()
        {
        }
    }

    public class TaskSpy : TaskDummy
    {
        public bool WasRun { get; private set; }

        public override void Run()
        {
            WasRun = true;
        }
    }
}