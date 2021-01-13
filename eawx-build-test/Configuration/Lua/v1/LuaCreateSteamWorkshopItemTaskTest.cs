using System.Collections.Generic;
using System.Linq;
using EawXBuild.Configuration.Lua.v1;
using EawXBuild.Steam;
using EawXBuildTest.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLua;

namespace EawXBuildTest.Configuration.Lua.v1 {
    [TestClass]
    public class LuaCreateSteamWorkshopItemTaskTest {
        private static readonly string[] ExpectedTags = {"EAW", "FOC"};
        private const long AppIdAsLong = 32740;
        private const uint AppIdAsUInt = (uint) AppIdAsLong;
        private const string Title = "My Awesome title";
        private const string DescriptionFilePath = "path/to/description";
        private const string FolderPath = "path/to/folder";
        private const string Language = "Spanish";

        private const string LuaPublicVisibility = "public";
        private const string LuaPrivateVisibility = "private";

        [TestMethod]
        public void GivenLuaCreateSteamWorkshopItemTaskWithConfigTable_With_PublicVisibility__OnCreation__ShouldConfigureTask() {
            var taskBuilderMock = CreateTaskBuilderMock(WorkshopItemVisibility.Public);

            using var luaInterpreter = new NLua.Lua();
            PushVisibilityTable(luaInterpreter);
            var table = CreateFullConfigurationTableWithoutTags(luaInterpreter, LuaPublicVisibility);
            var sut = new LuaCreateSteamWorkshopItemTask(taskBuilderMock, table);
            
            taskBuilderMock.Verify();
        }
        
        [TestMethod]
        public void GivenLuaCreateSteamWorkshopItemTaskWithConfigTable_With_PrivateVisibility__OnCreation__ShouldConfigureTask() {
            var taskBuilderMock = CreateTaskBuilderMock(WorkshopItemVisibility.Private);

            using var luaInterpreter = new NLua.Lua();
            PushVisibilityTable(luaInterpreter);
            var table = CreateFullConfigurationTableWithoutTags(luaInterpreter, LuaPrivateVisibility);
            var sut = new LuaCreateSteamWorkshopItemTask(taskBuilderMock, table);
            
            taskBuilderMock.Verify();
        }
        
        /// <summary>
        /// For this test we're not using the TaskBuilderMock, because it uses CollectionAssert under the hood, which doesn't do deep comparisons.
        /// Instead we're querying the "Tags" key manually
        /// </summary>
        [TestMethod]
        public void GivenLuaCreateSteamWorkshopItemTaskWithConfigTable_With_Tags__OnCreation__ShouldConfigureTaskWithTags() {
            var taskBuilderSpy = new TaskBuilderSpy();
            using var luaInterpreter = new NLua.Lua();
            var table = CreateConfigurationTableWithOnlyTags(luaInterpreter);
            
            var sut = new LuaCreateSteamWorkshopItemTask(taskBuilderSpy, table);

            var actual = taskBuilderSpy["Tags"];
            Assert.IsInstanceOfType(actual, typeof(IEnumerable<string>));
            CollectionAssert.AreEquivalent(ExpectedTags, ((IEnumerable<string>) actual).ToArray());
        }
        
        /// <summary>
        /// For this test we're not using the TaskBuilderMock, because it uses CollectionAssert under the hood, which doesn't do deep comparisons.
        /// Instead we're querying the "Tags" key manually
        /// </summary>
        [TestMethod]
        public void GivenLuaCreateSteamWorkshopItemTaskWithConfigTable_With_DuplicateTags__OnCreation__ShouldConfigureTaskWithoutDuplicateTags() {
            var taskBuilderSpy = new TaskBuilderSpy();
            using var luaInterpreter = new NLua.Lua();

            var table = CreateConfigurationTableWithOnlyTags(luaInterpreter);
            table["2"] = "EAW";

            var sut = new LuaCreateSteamWorkshopItemTask(taskBuilderSpy, table);

            var actual = taskBuilderSpy["Tags"];
            Assert.IsInstanceOfType(actual, typeof(IEnumerable<string>));
            CollectionAssert.AreEquivalent(ExpectedTags, ((IEnumerable<string>) actual).ToArray());
        }

        private static TaskBuilderMock CreateTaskBuilderMock(WorkshopItemVisibility visibility) {
            return new TaskBuilderMock(new Dictionary<string, object> {
                {"AppId", AppIdAsUInt},
                {"Title", Title},
                {"DescriptionFilePath", DescriptionFilePath},
                {"ItemFolderPath", FolderPath},
                {"Visibility", visibility},
                {"Language", Language}
            });
        }

        private static LuaTable CreateConfigurationTableWithOnlyTags(NLua.Lua luaInterpreter) {
            var table = NLuaUtilities.MakeLuaTable(luaInterpreter, "the_table");
            var tags = NLuaUtilities.MakeLuaTable(luaInterpreter, "tag_table");
            tags[0] = "EAW";
            tags[1] = "FOC";
            table["tags"] = tags;
            return table;
        }

        private static LuaTable CreateFullConfigurationTableWithoutTags(NLua.Lua luaInterpreter, string luaVisibility) {
            var table = NLuaUtilities.MakeLuaTable(luaInterpreter, "the_table");
            var visibility = (WorkshopItemVisibility) luaInterpreter.GetObjectFromPath("visibility." + luaVisibility);

            table["app_id"] = AppIdAsLong;
            table["title"] = Title;
            table["description_file"] = DescriptionFilePath;
            table["item_folder"] = FolderPath;
            table["visibility"] = visibility;
            table["language"] = Language;

            return table;
        }
        
        private static void PushVisibilityTable(NLua.Lua luaInterpreter) {
            luaInterpreter.NewTable("visibility");
            var luaTable = luaInterpreter.GetTable("visibility");
            luaTable["private"] = WorkshopItemVisibility.Private;
            luaTable["public"] = WorkshopItemVisibility.Public;
        }
    }
}