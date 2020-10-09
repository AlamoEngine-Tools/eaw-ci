using System.Linq;
using EawXBuild.Configuration.Lua.v1;
using EawXBuildTest.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Configuration.Lua.v1 {
    [TestClass]
    public class LuaProjectTest {

        [TestMethod]
        public void GivenLuaProjectWithName__WhenGettingProject__ProjectShouldHaveName() {
            var factoryStub = new BuildComponentFactoryStub();
            var sut = new LuaProject("TestProject", factoryStub);

            var actual = sut.Project.Name;
            Assert.AreEqual("TestProject", actual);
        }
        
        [TestMethod]
        public void GivenLuaProjectWithJob__WhenGettingProject__ProjectShouldHaveJob() {
            var jobStub = new JobStub();
            var factoryStub = new BuildComponentFactoryStub {
                Project = new ProjectStub(),
                Job = jobStub
            };

            var sut = new LuaProject("TestProject", factoryStub);
            sut.add_job("test-job");

            var actual = sut.Project as ProjectStub;
            var actualJob = actual?.Jobs.First();
            Assert.AreSame(jobStub, actualJob);
            Assert.AreEqual("test-job", actualJob?.Name);
        }

        [TestMethod]
        public void GivenLuaProject__WhenAddingJob__ShouldReturnLuaJob() {
            var jobStub = new JobStub();
            var factoryStub = new BuildComponentFactoryStub {
                Project = new ProjectStub(),
                Job = jobStub
            };

            var sut = new LuaProject("TestProject", factoryStub);
            var actual = sut.add_job("test-job");
            Assert.IsInstanceOfType(actual, typeof(LuaJob));
        }
    }
}