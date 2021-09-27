using System.Linq;
using EawXBuild.Configuration.Lua.v1;
using EawXBuild.Core;
using EawXBuildTest.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Configuration.Lua.v1
{
    [TestClass]
    public class LuaProjectTest
    {
        [TestMethod]
        public void GivenLuaProjectWithName__WhenGettingProject__ProjectShouldHaveName()
        {
            BuildComponentFactoryStub factoryStub = new BuildComponentFactoryStub();
            LuaProject sut = new LuaProject("TestProject", factoryStub);

            string actual = sut.Project.Name;
            Assert.AreEqual("TestProject", actual);
        }

        [TestMethod]
        public void GivenLuaProjectWithJob__WhenGettingProject__ProjectShouldHaveJob()
        {
            JobStub jobStub = new JobStub();
            BuildComponentFactoryStub factoryStub = new BuildComponentFactoryStub
            {
                Project = new ProjectStub(),
                Job = jobStub
            };

            LuaProject sut = new LuaProject("TestProject", factoryStub);
            sut.job("test-job");

            ProjectStub actual = sut.Project as ProjectStub;
            IJob actualJob = actual?.Jobs.First();
            Assert.AreSame(jobStub, actualJob);
            Assert.AreEqual("test-job", actualJob?.Name);
        }

        [TestMethod]
        public void GivenLuaProject__WhenAddingJob__ShouldReturnLuaJob()
        {
            JobStub jobStub = new JobStub();
            BuildComponentFactoryStub factoryStub = new BuildComponentFactoryStub
            {
                Project = new ProjectStub(),
                Job = jobStub
            };

            LuaProject sut = new LuaProject("TestProject", factoryStub);
            LuaJob actual = sut.job("test-job");
            Assert.IsInstanceOfType(actual, typeof(LuaJob));
        }
    }
}