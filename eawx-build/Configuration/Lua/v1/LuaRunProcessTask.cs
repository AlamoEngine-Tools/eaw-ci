using EawXBuild.Core;

namespace EawXBuild.Configuration.Lua.v1
{
    public class LuaRunProcessTask : ILuaTask
    {
        private readonly ITaskBuilder _taskBuilder;

        public LuaRunProcessTask(ITaskBuilder taskBuilder, string path)
        {
            _taskBuilder = taskBuilder;
            _taskBuilder.With("ExecutablePath", path);
        }

        public ITask Task => _taskBuilder.Build();

        public LuaRunProcessTask arguments(string args)
        {
            _taskBuilder.With("Arguments", args);
            return this;
        }

        public LuaRunProcessTask working_directory(string workingDirectory)
        {
            _taskBuilder.With("WorkingDirectory", workingDirectory);
            return this;
        }

        public LuaRunProcessTask allowed_to_fail(bool allowedToFail)
        {
            _taskBuilder.With("AllowedToFail", allowedToFail);
            return this;
        }
    }
}