using EawXBuild.Core;
using Microsoft.VisualBasic.CompilerServices;
using NLua;

namespace EawXBuild.Configuration.Lua.v1 {
    public class LuaUpdateSteamWorkshopItemTask : ILuaTask {
        public LuaUpdateSteamWorkshopItemTask(ITaskBuilder taskBuilder, LuaTable table) {
            taskBuilder
                .With("AppId", IntegerType.FromObject(table["app_id"]))
                .With("ItemId", IntegerType.FromObject(table["item_id"]))
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