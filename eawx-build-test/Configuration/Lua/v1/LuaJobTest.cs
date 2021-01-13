using EawXBuild.Configuration.Lua.v1;
using EawXBuild.Core;
using EawXBuildTest.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLua.Exceptions;
using static EawXBuildTest.Configuration.Lua.v1.NLuaUtilities;

namespace EawXBuildTest.Configuration.Lua.v1 {
    [TestClass]
    public class LuaJobTest {
        [TestMethod]
        public void GivenLuaJobWithJob__WhenCallingAddTask__ContainedJobShouldHaveTask() {
            var jobStub = new JobStub();
            var taskDummy = new TaskDummy();

            var sut = new LuaJob(jobStub);
            var luaTaskStub = new LuaTaskStub {Task = taskDummy};
            sut.add_task(luaTaskStub);

            CollectionAssert.Contains(jobStub.Tasks, taskDummy);
        }

        [TestMethod]
        public void GivenLuaJobWithJob__WhenCallingAddTasks__ContainedJobShouldHaveAllTasks() {
            using var luaInterpreter = new NLua.Lua();

            var firstTaskDummy = new TaskDummy();
            var secondTaskDummy = new TaskDummy();

            var taskTable = MakeLuaTable(luaInterpreter, "taskTable");
            taskTable[1] = new LuaTaskStub {Task = firstTaskDummy};
            taskTable[2] = new LuaTaskStub {Task = secondTaskDummy};

            var jobStub = new JobStub();
            var sut = new LuaJob(jobStub);
            sut.add_tasks(taskTable);

            ITask[] expectedTasks = {firstTaskDummy, secondTaskDummy};
            CollectionAssert.AreEqual(expectedTasks, jobStub.Tasks);
        }

        [TestMethod]
        [ExpectedException(typeof(LuaScriptException))]
        public void GivenLuaJobWithJob__WhenCallingAddTasksWithNonTaskValue__ShouldThrowLuaScriptException() {
            using var luaInterpreter = new NLua.Lua();
            var invalidTable = MakeLuaTable(luaInterpreter, "invalidTable");
            invalidTable[1] = "Invalid";
            
            var jobStub = new JobStub();
            var sut = new LuaJob(jobStub);
            sut.add_tasks(invalidTable);
        }

    }
}