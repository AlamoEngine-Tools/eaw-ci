using System;
using System.Collections.Generic;
using EawXBuild.Core;
using NLua;

namespace EawXBuild.Configuration.Lua.v1 {
    public class EawCiLuaEnvironment {
        private readonly IBuildComponentFactory _factory;

        public List<IProject> Projects { get; } = new List<IProject>();

        public EawCiLuaEnvironment(IBuildComponentFactory factory) {
            _factory = factory;
        }

        [LuaGlobal(Name = "project")]
        public LuaProject MakeLuaProject(string name) {
            var luaProject = new LuaProject(name, _factory);
            Projects.Add(luaProject.Project);
            return luaProject;
        }


        [LuaGlobal(Name = "copy")]
        public ILuaTask Copy(string source, string target) {
            return new LuaCopyTask(_factory.Task("Copy"), source, target);
        }

        [LuaGlobal(Name = "link")]
        public ILuaTask Link(string source, string target) {
            return new LuaCopyTask(_factory.Task("SoftCopy"), source, target);
        }

        [LuaGlobal(Name = "clean")]
        public ILuaTask Clean(string target) {
            return new LuaCleanTask(_factory.Task("Clean"), target);
        }

        [LuaGlobal(Name = "run_process")]
        public ILuaTask RunProcess(string path) {
            return new LuaRunProcessTask(_factory.Task("RunProgram"), path);
        }
    }
}