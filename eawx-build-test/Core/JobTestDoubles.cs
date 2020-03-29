using EawXBuild.Core;

namespace EawXBuildTest.Core
{
    public class JobStub : IJob
    {
        public string Name { get; set; }

        public virtual void Run()
        {
        }
    }

    public class JobSpy : JobStub
    {
        public bool WasRun { get; private set; }

        public override void Run()
        {
            WasRun = true;
        }
    }
}