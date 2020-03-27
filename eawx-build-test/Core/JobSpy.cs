using EawXBuild.Core;

namespace EawXBuildTest.Core
{
    public class JobSpy : IJob
    {
        public bool WasRun { get; private set; }

        public string Name { get; set; }

        public void Run()
        {
            WasRun = true;
        }
    }
}