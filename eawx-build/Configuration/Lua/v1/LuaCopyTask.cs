using EawXBuild.Core;

namespace EawXBuild.Configuration.Lua.v1
{
    public class LuaCopyTask : ILuaTask
    {
        private readonly ITaskBuilder _taskBuilder;

        public LuaCopyTask(ITaskBuilder taskBuilder, string source, string target)
        {
            _taskBuilder = taskBuilder;
            _taskBuilder
                .With("CopyFromPath", source)
                .With("CopyToPath", target);
        }

        public ITask Task => _taskBuilder.Build();

        public LuaCopyTask overwrite(bool overwrite)
        {
            _taskBuilder.With("AlwaysOverwrite", overwrite);
            return this;
        }

        public LuaCopyTask pattern(string pattern)
        {
            _taskBuilder.With("CopyFileByPattern", pattern);
            return this;
        }

        public LuaCopyTask recursive(bool recursive)
        {
            _taskBuilder.With("CopySubfolders", recursive);
            return this;
        }
    }
}