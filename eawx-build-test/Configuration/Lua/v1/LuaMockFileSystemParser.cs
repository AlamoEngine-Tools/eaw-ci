using System.IO.Abstractions.TestingHelpers;
using EawXBuild.Configuration.Lua.v1;
using NLua;

namespace EawXBuildTest.Configuration.Lua.v1 {
    public class LuaMockFileSystemParser : ILuaParser {
        private readonly MockFileSystem _fileSystem;
        private NLua.Lua _lua = new NLua.Lua();

        public LuaMockFileSystemParser(MockFileSystem fileSystem) {
            _fileSystem = fileSystem;
        }

        public void RegisterObject(object obj) {
            LuaRegistrationHelper.TaggedInstanceMethods(_lua, obj);
        }

        public void DoFile(string fileName) {
            var fileData = _fileSystem.GetFile(fileName);
            _lua.DoString(fileData.TextContents);
        }

        public void Dispose() {
            _lua?.Dispose();
        }
    }
}