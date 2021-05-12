using System.Collections.Generic;
using EawXBuild.Core;
using EawXBuild.Steam;
using NLua;

namespace EawXBuild.Configuration.Lua.v1
{
    public class EawCiLuaEnvironment
    {
        private readonly IBuildComponentFactory _factory;

        public EawCiLuaEnvironment(IBuildComponentFactory factory, ILuaParser parser)
        {
            _factory = factory;
            LuaTable luaTable = parser.NewTable("visibility");
            luaTable["private"] = WorkshopItemVisibility.Private;
            luaTable["public"] = WorkshopItemVisibility.Public;
        }

        public List<IProject> Projects { get; } = new List<IProject>();

        [LuaGlobal(Name = "project")]
        public LuaProject MakeLuaProject(string name)
        {
            LuaProject luaProject = new LuaProject(name, _factory);
            Projects.Add(luaProject.Project);
            return luaProject;
        }


        [LuaGlobal(Name = "copy")]
        public ILuaTask Copy(string source, string target)
        {
            return new LuaCopyTask(_factory.Task("Copy"), source, target);
        }

        [LuaGlobal(Name = "link")]
        public ILuaTask Link(string source, string target)
        {
            return new LuaCopyTask(_factory.Task("SoftCopy"), source, target);
        }

        [LuaGlobal(Name = "clean")]
        public ILuaTask Clean(string target)
        {
            return new LuaCleanTask(_factory.Task("Clean"), target);
        }

        [LuaGlobal(Name = "run_process")]
        public ILuaTask RunProcess(string path)
        {
            return new LuaRunProcessTask(_factory.Task("RunProgram"), path);
        }

        [LuaGlobal(Name = "create_steam_workshop_item")]
        public ILuaTask CreateSteamWorkshopItem(LuaTable luaTable)
        {
            return new LuaCreateSteamWorkshopItemTask(_factory.Task("CreateSteamWorkshopItem"), luaTable);
        }

        [LuaGlobal(Name = "update_steam_workshop_item")]
        public ILuaTask UpdateSteamWorkshopItem(LuaTable luaTable)
        {
            return new LuaUpdateSteamWorkshopItemTask(_factory.Task("UpdateSteamWorkshopItem"), luaTable);
        }
    }
}