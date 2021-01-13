using NLua;

namespace EawXBuild.Configuration.Lua.v1 {
    public class NLuaParser : ILuaParser {
        private readonly NLua.Lua _lua = new NLua.Lua();

        public LuaTable NewTable(string fullPath) {
            _lua.NewTable(fullPath);
            return _lua.GetTable(fullPath);
        }

        public void RegisterObject(object obj) {
            LuaRegistrationHelper.TaggedInstanceMethods(_lua, obj);
        }

        public void DoFile(string fileName) {
            _lua.DoFile(fileName);
        }

        public void Dispose() {
            _lua?.Dispose();
        }
    }
}