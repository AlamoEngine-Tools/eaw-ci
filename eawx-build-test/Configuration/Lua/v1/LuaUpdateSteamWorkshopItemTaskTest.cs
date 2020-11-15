using System.Collections.Generic;
using EawXBuild.Configuration.Lua.v1;
using EawXBuild.Steam;
using EawXBuildTest.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLua;

namespace EawXBuildTest.Configuration.Lua.v1 {
    [TestClass]
    public class LuaUpdateSteamWorkshopItemTaskTest {
        private const long AppIdAsLong = 32740;
        private const uint AppIdAsUInt = (uint) AppIdAsLong;
        private const long ItemIdAsLong = 1234;
        private const ulong ItemIdAsULong = ItemIdAsLong;
        private const string Title = "My Awesome title";
        private const string DescriptionFilePath = "path/to/description";
        private const string FolderPath = "path/to/folder";
        private const string Language = "Spanish";

        private const string LuaPublicVisibility = "public";
        private const string LuaPrivateVisibility = "private";

        [TestMethod]
        public void
            GivenLuaUpdateSteamWorkshopItemTaskWithConfigTable_With_PublicVisibility__OnCreation__ShouldConfigureTask() {
            var taskBuilderMock = CreateTaskBuilderMock(WorkshopItemVisibility.Public);

            using var luaInterpreter = new NLua.Lua();
            PushVisibilityTable(luaInterpreter);
            var table = CreateConfigurationTable(luaInterpreter, LuaPublicVisibility);
            var sut = new LuaUpdateSteamWorkshopItemTask(taskBuilderMock, table);

            taskBuilderMock.Verify();
        }

        [TestMethod]
        public void
            GivenLuaUpdateSteamWorkshopItemTaskWithConfigTable_With_PrivateVisibility__OnCreation__ShouldConfigureTask() {
            var taskBuilderMock = CreateTaskBuilderMock(WorkshopItemVisibility.Private);

            using var luaInterpreter = new NLua.Lua();
            PushVisibilityTable(luaInterpreter);
            var table = CreateConfigurationTable(luaInterpreter, LuaPrivateVisibility);
            var sut = new LuaUpdateSteamWorkshopItemTask(taskBuilderMock, table);

            taskBuilderMock.Verify();
        }

        private static LuaTable CreateConfigurationTable(NLua.Lua luaInterpreter, string luaVisibility) {
            var table = NLuaUtilities.MakeLuaTable(luaInterpreter, "the_table");
            var visibility = (WorkshopItemVisibility) luaInterpreter.GetObjectFromPath("visibility." + luaVisibility);
            table["app_id"] = AppIdAsLong;
            table["item_id"] = ItemIdAsLong;
            table["title"] = Title;
            table["description_file"] = DescriptionFilePath;
            table["item_folder"] = FolderPath;
            table["visibility"] = visibility;
            table["language"] = Language;
            return table;
        }

        private static TaskBuilderMock CreateTaskBuilderMock(WorkshopItemVisibility visibility) {
            return new TaskBuilderMock(new Dictionary<string, object> {
                {"AppId", AppIdAsUInt},
                {"ItemId", ItemIdAsULong},
                {"Title", Title},
                {"DescriptionFilePath", DescriptionFilePath},
                {"ItemFolderPath", FolderPath},
                {"Visibility", visibility},
                {"Language", Language}
            });
        }

        private static void PushVisibilityTable(NLua.Lua luaInterpreter) {
            luaInterpreter.NewTable("visibility");
            var luaTable = luaInterpreter.GetTable("visibility");
            luaTable["private"] = WorkshopItemVisibility.Private;
            luaTable["public"] = WorkshopItemVisibility.Public;
        }
    }
}