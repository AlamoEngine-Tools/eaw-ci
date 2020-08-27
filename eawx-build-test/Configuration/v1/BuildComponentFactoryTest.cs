using System;
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
        
        [TestMethod]
        public void BuildComponentFactory__WhenCallingTaskWith_RunProgram__ShouldReturnRunProcessTaskBuilder() {
            var sut = new BuildComponentFactory();

            var taskBuilder = sut.Task("RunProgram");
            
            Assert.IsInstanceOfType(taskBuilder, typeof(RunProcessTaskBuilder));
        }
        
        [TestMethod]
        public void BuildComponentFactory__WhenCallingTaskWith_Clean__ShouldReturnCleanTaskBuilder() {
            var sut = new BuildComponentFactory();

            var taskBuilder = sut.Task("Clean");
            
            Assert.IsInstanceOfType(taskBuilder, typeof(CleanTaskBuilder));
        }
        
        [TestMethod]
        public void BuildComponentFactory__WhenCallingTaskWith_SoftCopy__ShouldReturnCopyTaskBuilder() {
            var sut = new BuildComponentFactory();

            var taskBuilder = sut.Task("SoftCopy");
            
            Assert.IsInstanceOfType(taskBuilder, typeof(CopyTaskBuilder));
        }
        
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BuildComponentFactory__WhenCallingTaskWithUnknownTaskType__ShouldThrowInvalidOperationException() {
            var sut = new BuildComponentFactory();

            sut.Task("Unknown");
        }
        
    }
}