using EawXBuild.Core;

namespace EawXBuildTest.Core
{
    public class BuildComponentFactoryStub : IBuildComponentFactory
    {
        public IProject Project { get; set; }

        public JobStub Job { get; set; }

        public IProject MakeProject()
        {
            return Project;
        }

        public IJob MakeJob(string name)
        {
            Job = new JobStub {Name = name};
            return Job;
        }
    }
}
