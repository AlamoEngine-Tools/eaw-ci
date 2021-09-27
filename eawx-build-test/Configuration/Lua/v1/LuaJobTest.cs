using EawXBuild.Configuration.Lua.v1;
using EawXBuild.Core;
using EawXBuildTest.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLua;
using NLua.Exceptions;
using static EawXBuildTest.Configuration.Lua.v1.NLuaUtilities;

namespace EawXBuildTest.Configuration.Lua.v1
{
    [TestClass]
    public class LuaJobTest
    {

        private LuaJob sut;

        private JobStub jobStub;

        private NLua.Lua luaInterpreter;

        [TestInitialize]
        public void SetUp()
        {
            luaInterpreter = new NLua.Lua();
            jobStub = new JobStub();
            sut = new LuaJob(jobStub);
        }

        [TestCleanup]
        public void TearDown()
        {
            luaInterpreter.Dispose();
        }

        [TestMethod]
        public void GivenLuaJobWithJob__WhenCallingTasksWithSingleTask__ContainedJobShouldHaveTask()
        {
            var taskDummy = new TaskDummy();
            var luaTaskStub = new LuaTaskStub { Task = taskDummy };
            const string taskName = "TheTaskName";
            var taskTable = CreateLuaTaskTableWithActionAndName(luaTaskStub, taskName);

            var allTasksTable = MakeLuaTable(luaInterpreter, "allTasks");
            allTasksTable[1] = taskTable;

            sut.tasks(allTasksTable);

            Assert.AreEqual("TheTaskName", taskDummy.Name);
            CollectionAssert.Contains(jobStub.Tasks, taskDummy);
        }

        [TestMethod]
        public void GivenLuaJobWithJob__WhenCallingTasksWithMultipleTaskEntries__ContainedJobShouldHaveAllTasks()
        {
            var firstTaskDummy = new TaskDummy();
            var firstLuaTaskStub = new LuaTaskStub { Task = firstTaskDummy };
            var firstTaskTable = CreateLuaTaskTableWithActionAndName(firstLuaTaskStub, "FirstTask");

            var secondTaskDummy = new TaskDummy();
            var secondLuaTaskStub = new LuaTaskStub { Task = secondTaskDummy };
            var secondTaskTable = CreateLuaTaskTableWithActionAndName(secondLuaTaskStub, "SecondTask");

            var allTasksTable = MakeLuaTable(luaInterpreter, "allTasks");
            allTasksTable[1] = firstTaskTable;
            allTasksTable[2] = secondTaskTable;

            sut.tasks(allTasksTable);

            Assert.AreEqual("FirstTask", firstTaskDummy.Name);
            Assert.AreEqual("SecondTask", secondTaskDummy.Name);
            CollectionAssert.AreEquivalent(new[] { firstTaskDummy, secondTaskDummy }, jobStub.Tasks);
        }

        [TestMethod]
        [ExpectedException(typeof(LuaScriptException))]
        public void GivenLuaJobWithJob__WhenAddingTaskWithNonTaskTypeAction__ShouldThrowLuaScriptException()
        {
            const string action = "Something else";
            const string taskName = "TheTaskName";

            var taskTable = CreateLuaTaskTableWithActionAndName(action, taskName);

            var allTasksTable = MakeLuaTable(luaInterpreter, "allTasks");
            allTasksTable[1] = taskTable;

            sut.tasks(allTasksTable);
        }

        [TestMethod]
        [ExpectedException(typeof(LuaScriptException))]
        public void GivenLuaJobWithJob__WhenAddingTaskWithoutName__ShouldThrowLuaScriptException()
        {
            var action = new LuaTaskStub { Task = new TaskDummy() };
            const string taskName = null;

            var taskTable = CreateLuaTaskTableWithActionAndName(action, taskName);

            var allTasksTable = MakeLuaTable(luaInterpreter, "allTasks");
            allTasksTable[1] = taskTable;

            sut.tasks(allTasksTable);
        }

        [TestMethod]
        [ExpectedException(typeof(LuaScriptException))]
        public void GivenLuaJobWithJob__WhenCallingTasksWithNonArrayTable__ShouldThrowLuaScriptException()
        {

            var action = new LuaTaskStub { Task = new TaskDummy() };
            const string taskName = null;

            var taskTable = CreateLuaTaskTableWithActionAndName(action, taskName);

            var allTasksTable = MakeLuaTable(luaInterpreter, "allTasks");
            allTasksTable["NotAnInteger"] = taskTable;

            sut.tasks(allTasksTable);
        }

        private LuaTable CreateLuaTaskTableWithActionAndName(object action, string taskName)
        {
            var taskTable = MakeLuaTable(luaInterpreter, "taskTable");
            taskTable["action"] = action;
            taskTable["name"] = taskName;
            return taskTable;
        }

    }
}