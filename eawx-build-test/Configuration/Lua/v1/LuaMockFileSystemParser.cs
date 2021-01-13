using System.IO.Abstractions.TestingHelpers;
using EawXBuild.Configuration.Lua.v1;
using NLua;

namespace EawXBuildTest.Configuration.Lua.v1 {
    public class LuaMockFileSystemParser : ILuaParser {
        private readonly MockFileSystem _fileSystem;

        public LuaMockFileSystemParser(MockFileSystem fileSystem) {
            _fileSystem = fileSystem;
        }

        public NLua.Lua Lua { get; } = new NLua.Lua();

        public LuaTable NewTable(string fullPath) {
            Lua.NewTable(fullPath);
            return Lua.GetTable(fullPath);
        }

        public void RegisterObject(object obj) {
            LuaRegistrationHelper.TaggedInstanceMethods(Lua, obj);
        }

        public void DoFile(string fileName) {
            var fileData = _fileSystem.GetFile(fileName);
            Lua.DoString(fileData.TextContents);
        }

        public void Dispose() {
            Lua?.Dispose();
        }
    }
}