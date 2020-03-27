using EawXBuild.Core;

namespace EawXBuildTest.Core
{
    public class TaskSpy : ITask
    {
        public bool WasRun { get; private set; }

        public void Run()
        {
            WasRun = true;
        }
    }
}