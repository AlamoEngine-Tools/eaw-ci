using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using EawXBuild.Configuration.FrontendAgnostic;
using EawXBuild.Tasks;
using EawXBuildTest.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Configuration.FrontendAgnostic
{
    [TestClass]
    public class CopyTaskBuilderTest
    {
        private const string TheSourcePath = "the/source/path";
        private const string TheDestPath = "the/dest/path";
        private const bool Recursive = true;
        private const string FilePattern = "*.xml";
        private const bool AlwaysOverwrite = true;

        private const string TaskId = "TaskId";
        private const string TaskName = "TaskName";

        [TestMethod]
        public void
            GivenSourcePathDestPathRecursiveAndFilePattern__WhenCallingBuild__ShouldReturnCopyTaskWithMatchingConfig()
        {
            CopyTaskBuilder sut = new CopyTaskBuilder(new CopyPolicyDummy(), new MockFileSystem());

            ConfigureTask(sut);

            CopyTask task = (CopyTask) sut.Build();

            Assert.AreEqual(TaskId, task.Id);
            Assert.AreEqual(TaskName, task.Name);
            Assert.AreEqual(TheSourcePath, task.Source);
            Assert.AreEqual(TheDestPath, task.Destination);
            Assert.AreEqual(FilePattern, task.FilePattern);
            Assert.AreEqual(AlwaysOverwrite, task.AlwaysOverwrite);
            Assert.IsTrue(task.Recursive);
        }

        [TestMethod]
        public void GivenTaskBuiltWithValidConfiguration__WhenRunningTask__ShouldCopyPolicyShouldBeCalled()
        {
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {TheSourcePath, string.Empty}
            });
            CopyPolicySpy copyPolicySpy = new CopyPolicySpy();

            CopyTaskBuilder sut = new CopyTaskBuilder(copyPolicySpy, fileSystem);

            ConfigureTask(sut);
            CopyTask task = (CopyTask) sut.Build();

            task.Run();

            Assert.IsTrue(copyPolicySpy.CopyCalled);
        }

        private static void ConfigureTask(CopyTaskBuilder sut)
        {
            sut.With("Id", TaskId)
                .With("Name", TaskName)
                .With("CopyFromPath", TheSourcePath)
                .With("CopyToPath", TheDestPath)
                .With("CopySubfolders", Recursive)
                .With("CopyFileByPattern", FilePattern)
                .With("AlwaysOverwrite", AlwaysOverwrite);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WhenCallingWith_WithInvalidConfigOption__ShouldThrowInvalidOperationException()
        {
            CopyTaskBuilder sut = new CopyTaskBuilder(new CopyPolicyDummy(), new MockFileSystem());

            sut.With("InvalidOption", string.Empty);
        }
    }
}