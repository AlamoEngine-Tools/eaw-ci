using System.Collections.Generic;
using EawXBuild.Configuration.Lua.v1;
using EawXBuildTest.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLua;

namespace EawXBuildTest.Configuration.Lua.v1 {
    [TestClass]
    public class LuaUpdateSteamWorkshopItemTaskTest {
        
        private const int AppId = 32740;
        private const int ItemId = 1234;
        private const string Title = "My Awesome title";
        private const string DescriptionFilePath = "path/to/description";
        private const string FolderPath = "path/to/folder";
        private const string PublicVisibility = "Public";
        private const string PrivateVisibility = "Private";
        private const string Language = "Spanish";

        private const string LuaPublicVisibility = "public";
        private const string LuaPrivateVisibility = "private";
        
        [TestMethod]
        public void GivenLuaUpdateSteamWorkshopItemTaskWithConfigTable_With_PublicVisibility__OnCreation__ShouldConfigureTask() {
            var taskBuilderMock = CreateTaskBuilderMock(PublicVisibility);

            using var luaInterpreter = new NLua.Lua();
            var table = CreateConfigurationTable(luaInterpreter, LuaPublicVisibility);
            var sut = new LuaUpdateSteamWorkshopItemTask(taskBuilderMock, table);
            
            taskBuilderMock.Verify();
        }
        
        [TestMethod]
        public void GivenLuaUpdateSteamWorkshopItemTaskWithConfigTable_With_PrivateVisibility__OnCreation__ShouldConfigureTask() {
            var taskBuilderMock = CreateTaskBuilderMock(PrivateVisibility);

            using var luaInterpreter = new NLua.Lua();
            var table = CreateConfigurationTable(luaInterpreter, LuaPrivateVisibility);
            var sut = new LuaUpdateSteamWorkshopItemTask(taskBuilderMock, table);
            
            taskBuilderMock.Verify();
        }
        
        private static LuaTable CreateConfigurationTable(NLua.Lua luaInterpreter, string luaVisibility) {
            var table = NLuaUtilities.MakeLuaTable(luaInterpreter, "the_table");
            table["app_id"] = AppId;
            table["item_id"] = ItemId;
            table["title"] = Title;
            table["description_file"] = DescriptionFilePath;
            table["item_folder"] = FolderPath;
            table["visibility"] = luaVisibility;
            table["language"] = Language;
            return table;
        }
        
        private static TaskBuilderMock CreateTaskBuilderMock(string visibility) {
            return new TaskBuilderMock(new Dictionary<string, object> {
                {"AppId", AppId},
                {"ItemId", ItemId},
                {"Title", Title},
                {"DescriptionFilePath", DescriptionFilePath},
                {"ItemFolderPath", FolderPath},
                {"Visibility", visibility},
                {"Language", Language}
            });
        }
        
    }
}