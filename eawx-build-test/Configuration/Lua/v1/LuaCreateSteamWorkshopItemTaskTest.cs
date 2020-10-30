using System.Collections.Generic;
using EawXBuild.Configuration.Lua.v1;
using EawXBuildTest.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLua;

namespace EawXBuildTest.Configuration.Lua.v1 {
    [TestClass]
    public class LuaCreateSteamWorkshopItemTaskTest {
        private const int AppId = 32740;
        private const string Title = "My Awesome title";
        private const string Description = "The description";
        private const string FolderPath = "path/to/folder";
        private const string PublicVisibility = "Public";
        private const string PrivateVisibility = "Private";
        private const string Language = "Spanish";

        private const string LuaPublicVisibility = "public";
        private const string LuaPrivateVisibility = "private";

        [TestMethod]
        public void GivenLuaCreateSteamWorkshopItemTaskWithConfigTable_With_PublicVisibility__OnCreation__ShouldConfigureTask() {
            var taskBuilderMock = CreateTaskBuilderMock(PublicVisibility);

            using var luaInterpreter = new NLua.Lua();
            var table = CreateConfigurationTable(luaInterpreter, LuaPublicVisibility);
            var sut = new LuaCreateSteamWorkshopItemTask(taskBuilderMock, table);
            
            taskBuilderMock.Verify();
        }
        
        [TestMethod]
        public void GivenLuaCreateSteamWorkshopItemTaskWithConfigTable_With_PrivateVisibility__OnCreation__ShouldConfigureTask() {
            var taskBuilderMock = CreateTaskBuilderMock(PrivateVisibility);

            using var luaInterpreter = new NLua.Lua();
            var table = CreateConfigurationTable(luaInterpreter, LuaPrivateVisibility);
            var sut = new LuaCreateSteamWorkshopItemTask(taskBuilderMock, table);
            
            taskBuilderMock.Verify();
        }

        private static TaskBuilderMock CreateTaskBuilderMock(string visibility) {
            return new TaskBuilderMock(new Dictionary<string, object> {
                {"AppId", AppId},
                {"Title", Title},
                {"Description", Description},
                {"ItemFolderPath", FolderPath},
                {"Visibility", visibility},
                {"Language", Language}
            });
        }

        private static LuaTable CreateConfigurationTable(NLua.Lua luaInterpreter, string luaVisibility) {
            var table = NLuaUtilities.MakeLuaTable(luaInterpreter, "the_table");
            table["app_id"] = AppId;
            table["title"] = Title;
            table["description"] = Description;
            table["item_folder"] = FolderPath;
            table["visibility"] = luaVisibility;
            table["language"] = Language;
            return table;
        }
    }
}