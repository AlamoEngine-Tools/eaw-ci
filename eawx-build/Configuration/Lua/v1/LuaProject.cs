using EawXBuild.Core;

namespace EawXBuild.Configuration.Lua.v1
{
    public class LuaProject
    {
        private readonly IBuildComponentFactory _factory;

        public LuaProject(string name, IBuildComponentFactory factory)
        {
            _factory = factory;
            Project = factory.MakeProject();
            Project.Name = name;
        }

        public IProject Project { get; }

        public LuaJob job(string jobName)
        {
            IJob job = _factory.MakeJob(jobName);
            Project.AddJob(job);
            return new LuaJob(job);
        }
    }
}