using EawXBuild.Core;

namespace EawXBuildTest.Core
{
    class JobSpy : IJob
    {

        public string Name { get; set; }

        public bool WasRun { get; private set; }

        public void Run()
        {
            WasRun = true;
        }

    }
}