using EawXBuild.Core;
using NLua;
using NLua.Exceptions;

namespace EawXBuild.Configuration.Lua.v1 {
    public class LuaJob {
        private readonly IJob _job;

        public LuaJob(IJob job) {
            _job = job;
        }

        public void add_task(ILuaTask task) {
            _job.AddTask(task.Task);
        }

        public void add_tasks(LuaTable taskTable) {
            foreach (var key in taskTable.Keys) {
                if (!(taskTable[key] is ILuaTask luaTask))
                    throw new LuaScriptException("Table values must be tasks!", "");
                _job.AddTask(luaTask.Task);
            }
        }
    }
}