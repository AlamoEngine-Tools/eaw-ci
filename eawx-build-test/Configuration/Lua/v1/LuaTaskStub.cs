using EawXBuild.Configuration.Lua.v1;
using EawXBuild.Core;

namespace EawXBuildTest.Configuration.Lua.v1 {
    public class LuaTaskStub : ILuaTask {
        public ITask Task { get; set; }
    }
}