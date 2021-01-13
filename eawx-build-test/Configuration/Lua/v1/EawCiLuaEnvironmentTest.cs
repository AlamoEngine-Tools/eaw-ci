using System.IO.Abstractions.TestingHelpers;
using EawXBuild.Configuration.Lua.v1;
using EawXBuild.Steam;
using EawXBuildTest.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLua;
using static EawXBuildTest.Configuration.Lua.v1.NLuaUtilities;

namespace EawXBuildTest.Configuration.Lua.v1 {
    [TestClass]
    public class EawCiLuaEnvironmentTest {

        private LuaMockFileSystemParser _luaParser;
        
        [TestInitialize]
        public void SetUp() {
            _luaParser = new LuaMockFileSystemParser(new MockFileSystem());
        }

        [TestCleanup]
        public void TearDown() {
            _luaParser.Dispose();
        }
        
        [TestMethod]
        public void WhenCallingCopy__ShouldCallBuildComponentFactoryWithCopyTaskName() {
            var factorySpy = new BuildComponentFactorySpy();
            var sut = new EawCiLuaEnvironment(factorySpy, _luaParser);
            sut.Copy(string.Empty, string.Empty);

            Assert.AreSame("Copy", factorySpy.ActualTaskTypeName);
        }

        [TestMethod]
        public void WhenCallingCopy__ShouldReturnLuaCopyTask() {
            var factoryStub = new BuildComponentFactoryStub();
            var sut = new EawCiLuaEnvironment(factoryStub, _luaParser);
            var actual = sut.Copy(string.Empty, string.Empty);

            Assert.IsInstanceOfType(actual, typeof(LuaCopyTask));
        }
        
        [TestMethod]
        public void WhenCallingLink__ShouldCallBuildComponentFactoryWithSoftCopyTaskName() {
            var factorySpy = new BuildComponentFactorySpy();
            var sut = new EawCiLuaEnvironment(factorySpy, _luaParser);
            sut.Link(string.Empty, string.Empty);

            Assert.AreSame("SoftCopy", factorySpy.ActualTaskTypeName);
        }
        
        [TestMethod]
        public void WhenCallingLink__ShouldReturnLuaCopyTask() {
            var factoryStub = new BuildComponentFactoryStub();
            var sut = new EawCiLuaEnvironment(factoryStub, _luaParser);
            var actual = sut.Link(string.Empty, string.Empty);

            Assert.IsInstanceOfType(actual, typeof(LuaCopyTask));
        }

        [TestMethod]
        public void WhenCallingClean__ShouldCallBuildComponentFactoryWithCleanTaskName() {
            var factorySpy = new BuildComponentFactorySpy();
            var sut = new EawCiLuaEnvironment(factorySpy, _luaParser);
            sut.Clean(string.Empty);

            Assert.AreSame("Clean", factorySpy.ActualTaskTypeName);
        }

        [TestMethod]
        public void WhenCallingClean__ShouldReturnLuaCleanTask() {
            var factorySpy = new BuildComponentFactorySpy();
            var sut = new EawCiLuaEnvironment(factorySpy, _luaParser);
            var actual = sut.Clean(string.Empty);
            
            Assert.IsInstanceOfType(actual, typeof(LuaCleanTask));
        }
        
        [TestMethod]
        public void WhenCallingRunProcess__ShouldCallBuildComponentFactoryWithRunProgramTaskName() {
            var factorySpy = new BuildComponentFactorySpy();
            var sut = new EawCiLuaEnvironment(factorySpy, _luaParser);
            sut.RunProcess(string.Empty);

            Assert.AreSame("RunProgram", factorySpy.ActualTaskTypeName);
        }
        
        [TestMethod]
        public void WhenCallingRunProcess__ShouldReturnLuaRunProcessTask() {
            var factorySpy = new BuildComponentFactorySpy();
            var sut = new EawCiLuaEnvironment(factorySpy, _luaParser);
            var actual = sut.RunProcess(string.Empty);
            
            Assert.IsInstanceOfType(actual, typeof(LuaRunProcessTask));
        }
        
        [TestMethod]
        public void WhenCallingCreateSteamWorkshopItem__ShouldCallBuildComponentFactoryWithCreateWorkshopItemTaskName() {
            var factorySpy = new BuildComponentFactorySpy();
            var sut = new EawCiLuaEnvironment(factorySpy, _luaParser);
            
            var table = MakeLuaTable(_luaParser.Lua, "the_table");
            sut.CreateSteamWorkshopItem(table);

            Assert.AreSame("CreateSteamWorkshopItem", factorySpy.ActualTaskTypeName);
        }
        
        [TestMethod]
        public void WhenCallingCreateSteamWorkshopItem__ShouldReturnLuaCreateSteamWorkshopItemTask() {
            var factorySpy = new BuildComponentFactorySpy();
            var sut = new EawCiLuaEnvironment(factorySpy, _luaParser);
            
            var table = MakeLuaTable(_luaParser.Lua, "the_table");
            var actual = sut.CreateSteamWorkshopItem(table);
            
            Assert.IsInstanceOfType(actual, typeof(LuaCreateSteamWorkshopItemTask));
        }
        
        [TestMethod]
        public void WhenCallingUpdateSteamWorkshopItem__ShouldCallBuildComponentFactoryWithUpdateWorkshopItemTaskName() {
            var factorySpy = new BuildComponentFactorySpy();
            var sut = new EawCiLuaEnvironment(factorySpy, _luaParser);
            
            var table = MakeLuaTable(_luaParser.Lua, "the_table");
            sut.UpdateSteamWorkshopItem(table);

            Assert.AreSame("UpdateSteamWorkshopItem", factorySpy.ActualTaskTypeName);
        }
        
        [TestMethod]
        public void WhenCallingUpdateSteamWorkshopItem__ShouldReturnLuaUpdateSteamWorkshopItemTask() {
            var factorySpy = new BuildComponentFactorySpy();
            var sut = new EawCiLuaEnvironment(factorySpy, _luaParser);

            var table = MakeLuaTable(_luaParser.Lua, "the_table");
            var actual = sut.UpdateSteamWorkshopItem(table);
            
            Assert.IsInstanceOfType(actual, typeof(LuaUpdateSteamWorkshopItemTask));
        }

        [TestMethod]
        public void GivenNewEawCiLuaEnvironment__OnCreation__ShouldPushVisibilityTableToLuaEnvironment() {
            var sut = new EawCiLuaEnvironment(new BuildComponentFactoryStub(), _luaParser);

            var visibilityTable = _luaParser.Lua.GetTable("visibility");
            Assert.AreEqual(visibilityTable["private"], WorkshopItemVisibility.Private);
            Assert.AreEqual(visibilityTable["public"], WorkshopItemVisibility.Public);
        }

    }
}