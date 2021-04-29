using System;
using System.IO.Abstractions.TestingHelpers;
using EawXBuild.Configuration.FrontendAgnostic;
using EawXBuild.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Configuration.FrontendAgnostic
{
    [TestClass]
    public class RunProcessTaskBuilderTest
    {
        private const string PathToExe = "Path/To/Exe";
        private const string Args = "--first=5 --second=yes";
        private const string WorkingDirectory = "Path/To/Another/Dir";
        private const bool AllowedToFail = true;

        private const string TaskId = "TaskId";
        private const string TaskName = "TaskName";

        [TestMethod]
        public void WhenBuildingRunProcessTaskWithExePathAndArguments__ShouldReturnConfiguredTask()
        {
            RunProcessTaskBuilder sut = new RunProcessTaskBuilder(new MockFileSystem());

            RunProcessTask task = (RunProcessTask) sut
                .With("Id", TaskId)
                .With("Name", TaskName)
                .With("ExecutablePath", PathToExe)
                .With("Arguments", Args)
                .With("WorkingDirectory", WorkingDirectory)
                .With("AllowedToFail", AllowedToFail)
                .Build();

            Assert.AreEqual(TaskId, task.Id);
            Assert.AreEqual(TaskName, task.Name);
            Assert.AreEqual(PathToExe, task.ExecutablePath);
            Assert.AreEqual(Args, task.Arguments);
            Assert.AreEqual(WorkingDirectory, task.WorkingDirectory);
            Assert.AreEqual(AllowedToFail, task.AllowedToFail);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WhenBuildingWithUnknownConfigOption__ShouldThrowInvalidOperationException()
        {
            RunProcessTaskBuilder sut = new RunProcessTaskBuilder(new MockFileSystem());

            sut.With("UnknownOption", string.Empty);
        }
    }
}