using System;
using EawXBuild.Configuration.FrontendAgnostic;
using EawXBuild.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Configuration.FrontendAgnostic
{
    [TestClass]
    public class BuildComponentFactoryTest
    {
        [TestMethod]
        public void BuildComponentFactory__WhenCallingMakeProject__ShouldReturnProject()
        {
            BuildComponentFactory sut = new BuildComponentFactory();

            IProject project = sut.MakeProject();

            Assert.IsInstanceOfType(project, typeof(Project));
        }

        [TestMethod]
        public void BuildComponentFactory__WhenCallingMakeJob__ShouldReturnJob()
        {
            BuildComponentFactory sut = new BuildComponentFactory();

            IJob job = sut.MakeJob("job");

            Assert.IsInstanceOfType(job, typeof(Job));
            Assert.AreEqual("job", job.Name);
        }

        [TestMethod]
        public void BuildComponentFactory__WhenCallingTaskWith_Copy__ShouldReturnCopyTaskBuilder()
        {
            BuildComponentFactory sut = new BuildComponentFactory();

            ITaskBuilder taskBuilder = sut.Task("Copy");

            Assert.IsInstanceOfType(taskBuilder, typeof(CopyTaskBuilder));
        }

        [TestMethod]
        public void BuildComponentFactory__WhenCallingTaskWith_RunProgram__ShouldReturnRunProcessTaskBuilder()
        {
            BuildComponentFactory sut = new BuildComponentFactory();

            ITaskBuilder taskBuilder = sut.Task("RunProgram");

            Assert.IsInstanceOfType(taskBuilder, typeof(RunProcessTaskBuilder));
        }

        [TestMethod]
        public void BuildComponentFactory__WhenCallingTaskWith_Clean__ShouldReturnCleanTaskBuilder()
        {
            BuildComponentFactory sut = new BuildComponentFactory();

            ITaskBuilder taskBuilder = sut.Task("Clean");

            Assert.IsInstanceOfType(taskBuilder, typeof(CleanTaskBuilder));
        }

        [TestMethod]
        public void BuildComponentFactory__WhenCallingTaskWith_SoftCopy__ShouldReturnCopyTaskBuilder()
        {
            BuildComponentFactory sut = new BuildComponentFactory();

            ITaskBuilder taskBuilder = sut.Task("SoftCopy");

            Assert.IsInstanceOfType(taskBuilder, typeof(CopyTaskBuilder));
        }

        [TestMethod]
        public void
            BuildComponentFactory__WhenCallingTaskWith_CreateSteamWorkshopItem__ShouldReturnCreateSteamWorkshopItemTaskBuilder()
        {
            BuildComponentFactory sut = new BuildComponentFactory();

            ITaskBuilder taskBuilder = sut.Task("CreateSteamWorkshopItem");

            Assert.IsInstanceOfType(taskBuilder, typeof(CreateSteamWorkshopItemTaskBuilder));
        }

        [TestMethod]
        public void
            BuildComponentFactory__WhenCallingTaskWith_UpdateSteamWorkshopItem__ShouldReturnUpdateSteamWorkshopItemTaskBuilder()
        {
            BuildComponentFactory sut = new BuildComponentFactory();

            ITaskBuilder taskBuilder = sut.Task("UpdateSteamWorkshopItem");

            Assert.IsInstanceOfType(taskBuilder, typeof(UpdateSteamWorkshopItemTaskBuilder));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BuildComponentFactory__WhenCallingTaskWithUnknownTaskType__ShouldThrowInvalidOperationException()
        {
            BuildComponentFactory sut = new BuildComponentFactory();

            sut.Task("Unknown");
        }
    }
}