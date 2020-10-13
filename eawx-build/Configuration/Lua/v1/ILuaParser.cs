using System;

namespace EawXBuild.Configuration.Lua.v1 {
    public interface ILuaParser : IDisposable {

        void RegisterObject(object obj);
        
        void DoFile(string fileName);
    }
}