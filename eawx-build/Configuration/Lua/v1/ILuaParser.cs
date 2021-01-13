using System;
using NLua;

namespace EawXBuild.Configuration.Lua.v1 {
    public interface ILuaParser : IDisposable {
        LuaTable NewTable(string fullPath);

        void RegisterObject(object obj);

        void DoFile(string fileName);
    }
}