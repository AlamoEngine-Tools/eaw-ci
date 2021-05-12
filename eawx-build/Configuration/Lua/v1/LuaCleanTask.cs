using EawXBuild.Core;

namespace EawXBuild.Configuration.Lua.v1
{
    public class LuaCleanTask : ILuaTask
    {
        private readonly ITaskBuilder _taskBuilder;

        public LuaCleanTask(ITaskBuilder taskBuilder, string path)
        {
            _taskBuilder = taskBuilder;
            _taskBuilder.With("Path", path);
        }

        public ITask Task => _taskBuilder.Build();
    }
}