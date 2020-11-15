using System;
using EawXBuild.Core;
using Microsoft.VisualBasic.CompilerServices;
using NLua;

namespace EawXBuild.Configuration.Lua.v1 {
    public class LuaCreateSteamWorkshopItemTask : ILuaTask {
        public LuaCreateSteamWorkshopItemTask(ITaskBuilder taskBuilder, LuaTable table) {
            taskBuilder
                .With("AppId", Convert.ToUInt32(table["app_id"]))
                .With("Title", table["title"])
                .With("DescriptionFilePath", table["description_file"])
                .With("ItemFolderPath", table["item_folder"])
                .With("Visibility", table["visibility"])
                .With("Language", table["language"]);
            Task = taskBuilder.Build();
        }

        public ITask Task { get; private set; }
    }
}