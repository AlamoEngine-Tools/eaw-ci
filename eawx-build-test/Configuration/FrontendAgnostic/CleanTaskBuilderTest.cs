using System;
using System.IO.Abstractions.TestingHelpers;
using EawXBuild.Configuration.FrontendAgnostic;
using EawXBuild.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Configuration.FrontendAgnostic {
    [TestClass]
    public class CleanTaskBuilderTest {
        [TestMethod]
        public void WhenBuildingCleanTaskWithDirectoryPath__ShouldReturnConfiguredCleanTask() {
            const string pathToDirectory = "Path/To/Directory";
            var sut = new CleanTaskBuilder(new MockFileSystem());

            CleanTask task = (CleanTask) sut.With("Path", pathToDirectory).Build();

            Assert.AreEqual(pathToDirectory, task.Path);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WhenBuildingCleanTaskWithUnknownConfigOption__ShouldThrowInvalidOperationException() {
            var sut = new CleanTaskBuilder(new MockFileSystem());

            sut.With("Unknown", "").Build();
        }
    }
}