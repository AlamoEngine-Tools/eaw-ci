using System;
using EawXBuild.Configuration.v1;
using EawXBuild.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Configuration.v1 {
    [TestClass]
    public class CopyTaskBuilderTest {
        [TestMethod]
        public void
            GivenSourcePathDestPathRecursiveAndFilePattern__WhenCallingBuild__ShouldReturnCopyTaskWithMatchingConfig() {
            var sut = new CopyTaskBuilder();

            const string theSourcePath = "the/source/path";
            const string theDestPath = "the/dest/path";
            const bool recursive = true;
            const string filePattern = "*.xml";

            sut.With("CopyFromPath", theSourcePath)
                .With("CopyToPath", theDestPath)
                .With("CopySubfolders", recursive)
                .With("CopyFileByPattern", filePattern);

            CopyTask task = (CopyTask) sut.Build();

            Assert.AreEqual(theSourcePath, task.Source);
            Assert.AreEqual(theDestPath, task.Destination);
            Assert.AreEqual(filePattern, task.FilePattern);
            Assert.IsTrue(task.Recursive);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WhenCallingWith_WithInvalidConfigOption__ShouldThrowInvalidOperationException() {
            var sut = new CopyTaskBuilder();

            sut.With("InvalidOption", string.Empty);
        }
    }
}