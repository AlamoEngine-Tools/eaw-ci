using EawXBuild.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Core
{
    public class JobSpy : IJob
    {

        public string Name { get; set; }

        public bool WasRun { get; private set; }

        public void Run()
        {
            WasRun = true;
        }

    }
}
