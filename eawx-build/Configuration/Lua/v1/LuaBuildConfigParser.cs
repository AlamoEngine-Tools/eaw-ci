using System;
using System.Collections.Generic;
using EawXBuild.Configuration.FrontendAgnostic;
using EawXBuild.Core;

namespace EawXBuild.Configuration.Lua.v1
{
    public class LuaBuildConfigParser : IBuildConfigParser
    {
        private const string FileExtension = ".lua";
        private const string DefaultLua = "";
        private const ConfigVersion ConfigVersion = Configuration.ConfigVersion.V1;
        private readonly IBuildComponentFactory _factory;
        private readonly ILuaParser _luaParser;

        public LuaBuildConfigParser(ILuaParser luaParser, IBuildComponentFactory factory)
        {
            _luaParser = luaParser;
            _factory = factory;
        }

        public IEnumerable<IProject> Parse(string filePath)
        {
            EawCiLuaEnvironment luaEnvironment = new EawCiLuaEnvironment(_factory, _luaParser);
            _luaParser.RegisterObject(luaEnvironment);
            _luaParser.DoFile(filePath);
            return luaEnvironment.Projects;
        }

        public bool TestIsValidConfiguration(string filePath)
        {
            throw new NotImplementedException();
        }

        public ConfigVersion Version => ConfigVersion;
        public string ConfiguredFileExtension => FileExtension;
        public string DefaultConfigFile => DefaultLua;
    }
}