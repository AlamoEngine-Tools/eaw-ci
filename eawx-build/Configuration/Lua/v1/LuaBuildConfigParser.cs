using System;
using System.Collections.Generic;
using EawXBuild.Configuration.FrontendAgnostic;
using EawXBuild.Core;

namespace EawXBuild.Configuration.Lua.v1 {
    public class LuaBuildConfigParser : IBuildConfigParser {
        private readonly ILuaParser _luaParser;
        private readonly IBuildComponentFactory _factory;
        


        public LuaBuildConfigParser(ILuaParser luaParser, IBuildComponentFactory factory) {
            _luaParser = luaParser;
            _factory = factory;
        }

        public IEnumerable<IProject> Parse(string filePath) {
            var luaEnvironment = new EawCiLuaEnvironment(_factory);
            _luaParser.RegisterObject(luaEnvironment);
            _luaParser.DoFile(filePath);
            return luaEnvironment.Projects;
        }

        public bool TestIsValidConfiguration(string filePath) {
            throw new NotImplementedException();
        }

        public ConfigVersion Version { get; }
        
    }
}