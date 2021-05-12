using System.Collections.Generic;
using System.Linq;
using EawXBuild.Configuration.Lua.v1;
using EawXBuild.Steam;
using EawXBuildTest.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLua;

namespace EawXBuildTest.Configuration.Lua.v1
{
    [TestClass]
    public class LuaUpdateSteamWorkshopItemTaskTest
    {
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
        private static readonly string[] ExpectedTags = {"EAW", "FOC"};

        [TestMethod]
        public void
            GivenLuaUpdateSteamWorkshopItemTaskWithConfigTable_With_PublicVisibility__OnCreation__ShouldConfigureTask()
        {
            TaskBuilderMock taskBuilderMock = CreateTaskBuilderMock(WorkshopItemVisibility.Public);

            using NLua.Lua luaInterpreter = new NLua.Lua();
            PushVisibilityTable(luaInterpreter);
            LuaTable table = CreateConfigurationTable(luaInterpreter, LuaPublicVisibility);
            LuaUpdateSteamWorkshopItemTask sut = new LuaUpdateSteamWorkshopItemTask(taskBuilderMock, table);

            taskBuilderMock.Verify();
        }

        [TestMethod]
        public void
            GivenLuaUpdateSteamWorkshopItemTaskWithConfigTable_With_PrivateVisibility__OnCreation__ShouldConfigureTask()
        {
            TaskBuilderMock taskBuilderMock = CreateTaskBuilderMock(WorkshopItemVisibility.Private);

            using NLua.Lua luaInterpreter = new NLua.Lua();
            PushVisibilityTable(luaInterpreter);
            LuaTable table = CreateConfigurationTable(luaInterpreter, LuaPrivateVisibility);
            LuaUpdateSteamWorkshopItemTask sut = new LuaUpdateSteamWorkshopItemTask(taskBuilderMock, table);

            taskBuilderMock.Verify();
        }

        /// <summary>
        ///     For this test we're not using the TaskBuilderMock, because it uses CollectionAssert under the hood, which doesn't
        ///     do deep comparisons.
        ///     Instead we're querying the "Tags" key manually
        /// </summary>
        [TestMethod]
        public void
            GivenLuaUpdateSteamWorkshopItemTaskWithConfigTable_With_Tags__OnCreation__ShouldConfigureTaskWithTags()
        {
            TaskBuilderSpy taskBuilderSpy = new TaskBuilderSpy();
            using NLua.Lua luaInterpreter = new NLua.Lua();
            LuaTable table = CreateConfigurationTableWithOnlyTags(luaInterpreter);

            LuaUpdateSteamWorkshopItemTask sut = new LuaUpdateSteamWorkshopItemTask(taskBuilderSpy, table);

            object actual = taskBuilderSpy["Tags"];
            Assert.IsInstanceOfType(actual, typeof(IEnumerable<string>));
            CollectionAssert.AreEquivalent(ExpectedTags, ((IEnumerable<string>) actual).ToArray());
        }

        /// <summary>
        ///     For this test we're not using the TaskBuilderMock, because it uses CollectionAssert under the hood, which doesn't
        ///     do deep comparisons.
        ///     Instead we're querying the "Tags" key manually
        /// </summary>
        [TestMethod]
        public void
            GivenLuaUpdateSteamWorkshopItemTaskWithConfigTable_With_DuplicateTags__OnCreation__ShouldConfigureTaskWithoutDuplicateTags()
        {
            TaskBuilderSpy taskBuilderSpy = new TaskBuilderSpy();
            using NLua.Lua luaInterpreter = new NLua.Lua();

            LuaTable table = CreateConfigurationTableWithOnlyTags(luaInterpreter);
            table["2"] = "EAW";

            LuaUpdateSteamWorkshopItemTask sut = new LuaUpdateSteamWorkshopItemTask(taskBuilderSpy, table);

            object actual = taskBuilderSpy["Tags"];
            Assert.IsInstanceOfType(actual, typeof(IEnumerable<string>));
            CollectionAssert.AreEquivalent(ExpectedTags, ((IEnumerable<string>) actual).ToArray());
        }

        private static LuaTable CreateConfigurationTableWithOnlyTags(NLua.Lua luaInterpreter)
        {
            LuaTable table = NLuaUtilities.MakeLuaTable(luaInterpreter, "the_table");
            LuaTable tags = NLuaUtilities.MakeLuaTable(luaInterpreter, "tag_table");
            tags[0] = "EAW";
            tags[1] = "FOC";
            table["tags"] = tags;
            return table;
        }

        private static LuaTable CreateConfigurationTable(NLua.Lua luaInterpreter, string luaVisibility)
        {
            LuaTable table = NLuaUtilities.MakeLuaTable(luaInterpreter, "the_table");
            WorkshopItemVisibility visibility =
                (WorkshopItemVisibility) luaInterpreter.GetObjectFromPath("visibility." + luaVisibility);
            table["app_id"] = AppIdAsLong;
            table["item_id"] = ItemIdAsLong;
            table["title"] = Title;
            table["description_file"] = DescriptionFilePath;
            table["item_folder"] = FolderPath;
            table["visibility"] = visibility;
            table["language"] = Language;
            return table;
        }

        private static TaskBuilderMock CreateTaskBuilderMock(WorkshopItemVisibility visibility)
        {
            return new TaskBuilderMock(new Dictionary<string, object>
            {
                {"AppId", AppIdAsUInt},
                {"ItemId", ItemIdAsULong},
                {"Title", Title},
                {"DescriptionFilePath", DescriptionFilePath},
                {"ItemFolderPath", FolderPath},
                {"Visibility", visibility},
                {"Language", Language}
            });
        }

        private static void PushVisibilityTable(NLua.Lua luaInterpreter)
        {
            luaInterpreter.NewTable("visibility");
            LuaTable luaTable = luaInterpreter.GetTable("visibility");
            luaTable["private"] = WorkshopItemVisibility.Private;
            luaTable["public"] = WorkshopItemVisibility.Public;
        }
    }
}