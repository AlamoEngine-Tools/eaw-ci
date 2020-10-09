using EawXBuild.Configuration.Lua.v1;
using EawXBuildTest.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Configuration.Lua.v1 {
    [TestClass]
    public class LuaJobTest {
        [TestMethod]
        public void GivenLuaJobWithJob__WhenCallingAddTask__ContainedJobShouldHaveTask() {
            var jobStub = new JobStub();
            var taskDummy = new TaskDummy();
            
            var sut = new LuaJob(jobStub);
            var luaTaskStub = new LuaTaskStub { Task = taskDummy};
            sut.add_task(luaTaskStub);
            
            CollectionAssert.Contains(jobStub.Tasks, taskDummy);
        }
    }
}