using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using EawXBuild.Tasks;
using EawXBuildTest.Native;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Tasks {
    [TestClass]
    public class LinkCopyPolicyTest {
        private string _currentDir;
        private string _sourceFileName;
        private string _targetFileName;
        private MockFileSystem _fileSystem;

        [TestInitialize]
        public void SetUp() {
            SetPlatformSpecificPaths();
            SetUpFileSystem();
        }

        [TestMethod]
        public void GivenSourceAndTargetFileInfo__WhenCopyTo__ShouldCallLinkerWithFullFileNames() {
            var sourceFile = _fileSystem.FileInfo.FromFileName(_sourceFileName);
            var targetFile = _fileSystem.FileInfo.FromFileName(_targetFileName);

            var fileLinkerSpy = new FileLinkerSpy();
            var sut = new LinkCopyPolicy(fileLinkerSpy);

            sut.CopyTo(sourceFile, targetFile, true);

            Assert.AreEqual(_sourceFileName, fileLinkerSpy.ReceivedSource);
            Assert.AreEqual(_targetFileName, fileLinkerSpy.ReceivedTarget);
        }

        [TestMethod]
        public void GivenTargetExistsAndOverwriteFalse__WhenCallingCopyTo__ShouldNotCallLinker() {
            var sourceFile = _fileSystem.FileInfo.FromFileName(_sourceFileName);
            var targetFile = _fileSystem.FileInfo.FromFileName(_targetFileName);

            var fileLinkerSpy = new FileLinkerSpy();
            var sut = new LinkCopyPolicy(fileLinkerSpy);

            sut.CopyTo(sourceFile, targetFile, false);

            Assert.IsFalse(fileLinkerSpy.CreateLinkWasCalled);
        }

        private void SetPlatformSpecificPaths() {
            if (TestUtility.IsWindows()) {
                _currentDir = @"C:\folder";
                _sourceFileName = @"C:\folder\sourceFile";
                _targetFileName = @"C:\home\folder\targetFile";
                return;
            }

            _currentDir = "/home/folder";
            _sourceFileName = "/home/folder/sourceFile";
            _targetFileName = "/home/folder/targetFile";
        }

        private void SetUpFileSystem() {
            var files = new Dictionary<string, MockFileData> {
                {_sourceFileName, string.Empty},
                {_targetFileName, string.Empty}
            };
            _fileSystem = new MockFileSystem(files, _currentDir);
        }
    }
}