using EawXBuild.Configuration.Lua.v1;
using EawXBuildTest.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLua;
using static EawXBuildTest.Configuration.Lua.v1.NLuaUtilities;

namespace EawXBuildTest.Configuration.Lua.v1 {
    [TestClass]
    public class EawCiLuaEnvironmentTest {
        [TestMethod]
        public void WhenCallingCopy__ShouldCallBuildComponentFactoryWithCopyTaskName() {
            var factorySpy = new BuildComponentFactorySpy();
            var sut = new EawCiLuaEnvironment(factorySpy);
            sut.Copy(string.Empty, string.Empty);

            Assert.AreSame("Copy", factorySpy.ActualTaskTypeName);
        }

        [TestMethod]
        public void WhenCallingCopy__ShouldReturnLuaCopyTask() {
            var factoryStub = new BuildComponentFactoryStub();
            var sut = new EawCiLuaEnvironment(factoryStub);
            var actual = sut.Copy(string.Empty, string.Empty);

            Assert.IsInstanceOfType(actual, typeof(LuaCopyTask));
        }
        
        [TestMethod]
        public void WhenCallingLink__ShouldCallBuildComponentFactoryWithSoftCopyTaskName() {
            var factorySpy = new BuildComponentFactorySpy();
            var sut = new EawCiLuaEnvironment(factorySpy);
            sut.Link(string.Empty, string.Empty);

            Assert.AreSame("SoftCopy", factorySpy.ActualTaskTypeName);
        }
        
        [TestMethod]
        public void WhenCallingLink__ShouldReturnLuaCopyTask() {
            var factoryStub = new BuildComponentFactoryStub();
            var sut = new EawCiLuaEnvironment(factoryStub);
            var actual = sut.Link(string.Empty, string.Empty);

            Assert.IsInstanceOfType(actual, typeof(LuaCopyTask));
        }

        [TestMethod]
        public void WhenCallingClean__ShouldCallBuildComponentFactoryWithCleanTaskName() {
            var factorySpy = new BuildComponentFactorySpy();
            var sut = new EawCiLuaEnvironment(factorySpy);
            sut.Clean(string.Empty);

            Assert.AreSame("Clean", factorySpy.ActualTaskTypeName);
        }

        [TestMethod]
        public void WhenCallingClean__ShouldReturnLuaCleanTask() {
            var factorySpy = new BuildComponentFactorySpy();
            var sut = new EawCiLuaEnvironment(factorySpy);
            var actual = sut.Clean(string.Empty);
            
            Assert.IsInstanceOfType(actual, typeof(LuaCleanTask));
        }
        
        [TestMethod]
        public void WhenCallingRunProcess__ShouldCallBuildComponentFactoryWithRunProgramTaskName() {
            var factorySpy = new BuildComponentFactorySpy();
            var sut = new EawCiLuaEnvironment(factorySpy);
            sut.RunProcess(string.Empty);

            Assert.AreSame("RunProgram", factorySpy.ActualTaskTypeName);
        }
        
        [TestMethod]
        public void WhenCallingRunProcess__ShouldReturnLuaRunProcessTask() {
            var factorySpy = new BuildComponentFactorySpy();
            var sut = new EawCiLuaEnvironment(factorySpy);
            var actual = sut.RunProcess(string.Empty);
            
            Assert.IsInstanceOfType(actual, typeof(LuaRunProcessTask));
        }
        
        [TestMethod]
        public void WhenCallingCreateSteamWorkshopItem__ShouldCallBuildComponentFactoryWithCreateWorkshopItemTaskName() {
            var factorySpy = new BuildComponentFactorySpy();
            var sut = new EawCiLuaEnvironment(factorySpy);

            using var luaInterpreter = new NLua.Lua();
            var table = MakeLuaTable(luaInterpreter, "the_table");
            sut.CreateSteamWorkshopItem(table);

            Assert.AreSame("CreateSteamWorkshopItem", factorySpy.ActualTaskTypeName);
        }
        
        [TestMethod]
        public void WhenCallingCreateSteamWorkshopItem__ShouldReturnLuaCreateSteamWorkshopItemTask() {
            var factorySpy = new BuildComponentFactorySpy();
            var sut = new EawCiLuaEnvironment(factorySpy);
            
            using var luaInterpreter = new NLua.Lua();
            var table = MakeLuaTable(luaInterpreter, "the_table");
            var actual = sut.CreateSteamWorkshopItem(table);
            
            Assert.IsInstanceOfType(actual, typeof(LuaCreateSteamWorkshopItemTask));
        }

    }
}