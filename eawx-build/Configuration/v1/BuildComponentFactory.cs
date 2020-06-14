using EawXBuild.Core;

namespace EawXBuild.Configuration.v1 {
    public class BuildComponentFactory : IBuildComponentFactory {
        public IProject MakeProject() {
            return new Project();
        }

        public IJob MakeJob(string name) {
            return new Job(name);
        }

        public ITaskBuilder Task(string taskTypeName) {
            return new CopyTaskBuilder();
        }
    }
}