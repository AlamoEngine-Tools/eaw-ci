using EawXBuild.Core;

namespace EawXBuild.Configuration.Lua.v1 {
    public class LuaJob {
        private readonly IJob _job;

        public LuaJob(IJob job) {
            _job = job;
        }

        public void add_task(ILuaTask task) {
            _job.AddTask(task.Task);
        }
    }
}