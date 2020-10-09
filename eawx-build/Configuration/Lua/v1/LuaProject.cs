using EawXBuild.Core;

namespace EawXBuild.Configuration.Lua.v1 {
    public class LuaProject {
        private readonly IBuildComponentFactory _factory;

        public LuaProject(string name, IBuildComponentFactory factory) {
            _factory = factory;
            Project = factory.MakeProject();
            Project.Name = name;
        }

        public LuaJob add_job(string jobName) {
            var job = _factory.MakeJob(jobName);
            Project.AddJob(job);
            return new LuaJob(job);
        }

        public IProject Project { get; private set; }
    }
}