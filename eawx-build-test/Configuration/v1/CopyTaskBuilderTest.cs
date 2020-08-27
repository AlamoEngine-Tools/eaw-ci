using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using EawXBuild.Configuration.v1;
using EawXBuild.Tasks;
using EawXBuildTest.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Configuration.v1 {
    [TestClass]
    public class CopyTaskBuilderTest {
        private const string TheSourcePath = "the/source/path";
        private const string TheDestPath = "the/dest/path";
        private const bool Recursive = true;
        private const string FilePattern = "*.xml";

        [TestMethod]
        public void
            GivenSourcePathDestPathRecursiveAndFilePattern__WhenCallingBuild__ShouldReturnCopyTaskWithMatchingConfig() {
            var sut = new CopyTaskBuilder(new MockFileSystem(), new CopyPolicyDummy());

            ConfigureTask(sut);

            CopyTask task = (CopyTask) sut.Build();

            Assert.AreEqual(TheSourcePath, task.Source);
            Assert.AreEqual(TheDestPath, task.Destination);
            Assert.AreEqual(FilePattern, task.FilePattern);
            Assert.IsTrue(task.Recursive);
        }

        [TestMethod]
        public void GivenTaskBuiltWithValidConfiguration__WhenRunningTask__ShouldCopyPolicyShouldBeCalled() {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                {TheSourcePath, string.Empty}
            });
            var copyPolicySpy = new CopyPolicySpy();

            var sut = new CopyTaskBuilder(fileSystem, copyPolicySpy);

            ConfigureTask(sut);
            CopyTask task = (CopyTask) sut.Build();

            task.Run();

            Assert.IsTrue(copyPolicySpy.CopyCalled);
        }

        private static void ConfigureTask(CopyTaskBuilder sut) {
            sut.With("CopyFromPath", TheSourcePath)
                .With("CopyToPath", TheDestPath)
                .With("CopySubfolders", Recursive)
                .With("CopyFileByPattern", FilePattern);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WhenCallingWith_WithInvalidConfigOption__ShouldThrowInvalidOperationException() {
            var sut = new CopyTaskBuilder(new MockFileSystem(), new CopyPolicyDummy());

            sut.With("InvalidOption", string.Empty);
        }
    }
}