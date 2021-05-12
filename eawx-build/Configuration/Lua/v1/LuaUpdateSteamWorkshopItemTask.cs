using System;
using System.Collections.Generic;
using System.Linq;
using EawXBuild.Core;
using NLua;

namespace EawXBuild.Configuration.Lua.v1
{
    public class LuaUpdateSteamWorkshopItemTask : ILuaTask
    {
        public LuaUpdateSteamWorkshopItemTask(ITaskBuilder taskBuilder, LuaTable table)
        {
            taskBuilder
                .With("AppId", Convert.ToUInt32(table["app_id"]))
                .With("ItemId", Convert.ToUInt64(table["item_id"]))
                .With("Title", table["title"])
                .With("DescriptionFilePath", table["description_file"])
                .With("ItemFolderPath", table["item_folder"])
                .With("Visibility", table["visibility"])
                .With("Language", table["language"]);

            LuaTable tags = (LuaTable) table["tags"];
            HashSet<string> stringTags = tags?.Values.Cast<string>().ToHashSet();
            if (stringTags != null)
                taskBuilder.With("Tags", stringTags);

            Task = taskBuilder.Build();
        }

        public ITask Task { get; }
    }
}