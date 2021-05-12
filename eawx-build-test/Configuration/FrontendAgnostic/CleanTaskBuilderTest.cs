using System;
using System.IO.Abstractions.TestingHelpers;
using EawXBuild.Configuration.FrontendAgnostic;
using EawXBuild.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Configuration.FrontendAgnostic
{
    [TestClass]
    public class CleanTaskBuilderTest
    {
        [TestMethod]
        public void WhenBuildingCleanTaskWithNameAndDirectoryPath__ShouldReturnConfiguredCleanTask()
        {
            const string pathToDirectory = "Path/To/Directory";
            const string expectedId = "taskId";
            const string expectedName = "taskName";

            CleanTaskBuilder sut = new CleanTaskBuilder(new MockFileSystem());

            CleanTask task = (CleanTask) sut
                .With("Id", expectedId)
                .With("Name", expectedName)
                .With("Path", pathToDirectory)
                .Build();

            Assert.AreEqual(expectedId, task.Id);
            Assert.AreEqual(expectedName, task.Name);
            Assert.AreEqual(pathToDirectory, task.Path);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WhenBuildingCleanTaskWithUnknownConfigOption__ShouldThrowInvalidOperationException()
        {
            CleanTaskBuilder sut = new CleanTaskBuilder(new MockFileSystem());

            sut.With("Unknown", "").Build();
        }
    }
}