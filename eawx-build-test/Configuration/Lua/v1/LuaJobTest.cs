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
        [TestMethod]
        public void GivenLuaJobWithJob__WhenCallingAddTask__ContainedJobShouldHaveTask()
        {
            JobStub jobStub = new JobStub();
            TaskDummy taskDummy = new TaskDummy();

            LuaJob sut = new LuaJob(jobStub);
            LuaTaskStub luaTaskStub = new LuaTaskStub {Task = taskDummy};
            sut.add_task(luaTaskStub);

            CollectionAssert.Contains(jobStub.Tasks, taskDummy);
        }

        [TestMethod]
        public void GivenLuaJobWithJob__WhenCallingAddTasks__ContainedJobShouldHaveAllTasks()
        {
            using NLua.Lua luaInterpreter = new NLua.Lua();

            TaskDummy firstTaskDummy = new TaskDummy();
            TaskDummy secondTaskDummy = new TaskDummy();

            LuaTable taskTable = MakeLuaTable(luaInterpreter, "taskTable");
            taskTable[1] = new LuaTaskStub {Task = firstTaskDummy};
            taskTable[2] = new LuaTaskStub {Task = secondTaskDummy};

            JobStub jobStub = new JobStub();
            LuaJob sut = new LuaJob(jobStub);
            sut.add_tasks(taskTable);

            ITask[] expectedTasks = {firstTaskDummy, secondTaskDummy};
            CollectionAssert.AreEqual(expectedTasks, jobStub.Tasks);
        }

        [TestMethod]
        [ExpectedException(typeof(LuaScriptException))]
        public void GivenLuaJobWithJob__WhenCallingAddTasksWithNonTaskValue__ShouldThrowLuaScriptException()
        {
            using NLua.Lua luaInterpreter = new NLua.Lua();
            LuaTable invalidTable = MakeLuaTable(luaInterpreter, "invalidTable");
            invalidTable[1] = "Invalid";

            JobStub jobStub = new JobStub();
            LuaJob sut = new LuaJob(jobStub);
            sut.add_tasks(invalidTable);
        }
    }
}