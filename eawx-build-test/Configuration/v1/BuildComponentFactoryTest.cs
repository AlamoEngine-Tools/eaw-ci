using EawXBuild.Configuration.v1;
using EawXBuild.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Configuration.v1 {
    [TestClass]
    public class BuildComponentFactoryTest {
        [TestMethod]
        public void BuildComponentFactory__WhenCallingMakeProject__ShouldReturnProject() {
            var sut = new BuildComponentFactory();

            var project = sut.MakeProject();
            
            Assert.IsInstanceOfType(project, typeof(Project));
        }
        
        [TestMethod]
        public void BuildComponentFactory__WhenCallingMakeJob__ShouldReturnJob() {
            var sut = new BuildComponentFactory();

            var job = sut.MakeJob("job");
            
            Assert.IsInstanceOfType(job, typeof(Job));
            Assert.AreEqual("job", job.Name);
        }

        [TestMethod]
        public void BuildComponentFactory__WhenCallingTaskWith_Copy__ShouldReturnCopyTaskBuilder() {
            var sut = new BuildComponentFactory();

            var taskBuilder = sut.Task("Copy");
            
            Assert.IsInstanceOfType(taskBuilder, typeof(CopyTaskBuilder));
        }
        
    }
}