using System;
using EawXBuild.Core;
using NLua;

namespace EawXBuild.Configuration.Lua.v1 {
    public class LuaUpdateSteamWorkshopItemTask : ILuaTask {
        public LuaUpdateSteamWorkshopItemTask(ITaskBuilder taskBuilder, LuaTable table) {
            taskBuilder
                .With("AppId", Convert.ToUInt32(table["app_id"]))
                .With("ItemId", Convert.ToUInt64(table["item_id"]))
                .With("Title", table["title"])
                .With("DescriptionFilePath", table["description_file"])
                .With("ItemFolderPath", table["item_folder"])
                .With("Visibility", table["visibility"])
                .With("Language", table["language"]);

            Task = taskBuilder.Build();
        }

        public ITask Task { get; }
    }
}