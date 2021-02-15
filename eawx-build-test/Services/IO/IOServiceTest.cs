using System.IO.Abstractions.TestingHelpers;
using EawXBuild.Services.IO;
using EawXBuildTest.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Services.IO {
    [TestClass]
    public class IOServiceTest {
        private MockFileSystem _fileSystem;
        private FileSystemAssertions _assertions;


        [TestInitialize]
        public void SetUp() {
            TestUtility.GetConfiguredMockFileSystem(out _fileSystem, out _assertions);
        }

        [PlatformSpecificTestMethod("Windows")]
        [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
        [DataRow("C:/data/test/path", ".dat", true)]
        [DataRow("C:/data/test/path", ".xml", false)]
        [DataRow("C:/data/abba/path", ".xml", false)]
        [DataRow("C:/data/path", ".xml", true)]
        [DataRow("C:/da/path", ".xml", false)]
        [DataRow("C:/data/path", ".dat", false)]
        public void GivenAbsolutePathToFile__WithRootedPath__IsValidPath__IsExpected__WIN(string absoluteDirectoryPath,
            string fileExtension, bool expected) {
            const string fileName = "test";
            var svc = new IOService(_fileSystem);
            Assert.AreEqual(expected, svc.IsValidPath(
                _fileSystem.Path.Combine(absoluteDirectoryPath, fileName + fileExtension),
                string.Empty, fileExtension));
        }

        [PlatformSpecificTestMethod("Linux", "OSX")]
        [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
        [DataRow("/mnt/c/data/test/path", ".dat", true)]
        [DataRow("/mnt/c/data/test/path", ".xml", false)]
        [DataRow("/mnt/c/data/abba/path", ".xml", false)]
        [DataRow("/mnt/c/data/path", ".xml", true)]
        [DataRow("/mnt/c/da/path", ".xml", false)]
        [DataRow("/mnt/c/data/path", ".dat", false)]
        public void GivenAbsolutePathToFile__WithRootedPath__IsValidPath__IsExpected__UNX(string absoluteDirectoryPath,
            string fileExtension, bool expected) {
            const string fileName = "test";
            var svc = new IOService(_fileSystem);
            Assert.AreEqual(expected, svc.IsValidPath(
                _fileSystem.Path.Combine(absoluteDirectoryPath, fileName + fileExtension),
                string.Empty, fileExtension));
        }

        [PlatformSpecificTestMethod("Windows")]
        [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
        [DataRow("data/test/path", ".dat")]
        [DataRow("/test/path", ".xml")]
        [DataRow("abba/path", ".xml")]
        [DataRow("data/path", ".xml")]
        [DataRow("da/path", ".xml")]
        [DataRow("data/path", ".dat")]
        public void GivenRelativePathToFile__WithoutRootedPath__IsValidPath__IsFalse_WIN(string absoluteDirectoryPath,
            string fileExtension) {
            const string fileName = "test";
            var svc = new IOService(_fileSystem);
            Assert.IsFalse(svc.IsValidPath(
                _fileSystem.Path.Combine(absoluteDirectoryPath, fileName + fileExtension),
                string.Empty, fileExtension));
        }

        [PlatformSpecificTestMethod("Linux", "OSX")]
        [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
        [DataRow("data/test/path", ".dat")]
        [DataRow("/test/path", ".xml")]
        [DataRow("abba/path", ".xml")]
        [DataRow("data/path", ".xml")]
        [DataRow("da/path", ".xml")]
        [DataRow("data/path", ".dat")]
        public void GivenRelativePathToFile__WithoutRootedPath__IsValidPath__IsFalse_UNX(string absoluteDirectoryPath,
            string fileExtension) {
            var svc = new IOService(_fileSystem);

            const string fileName = "test";
            Assert.IsFalse(svc.IsValidPath(
                _fileSystem.Path.Combine(absoluteDirectoryPath, fileName + fileExtension),
                string.Empty, fileExtension));
        }

        [PlatformSpecificTestMethod("Linux", "OSX")]
        [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
        [DataRow("test/path", "/mnt/c/data", ".dat", true)]
        [DataRow("../../path", "/mnt/c/data/test/path", ".xml", true)]
        [DataRow("../../path", "/mnt/c/data/test/path", ".dat", false)]
        [DataRow("../path", "/mnt/c/data/test/path", ".xml", false)]
        public void GivenRelativePathToFile__IsValidPath__IsExpected_UNX(string relativeDirectoryPath, string basePath,
            string fileExtension, bool expected) {
            const string fileName = "test";
            var svc = new IOService(_fileSystem);
            Assert.AreEqual(expected, svc.IsValidPath(
                _fileSystem.Path.Combine(relativeDirectoryPath, fileName + fileExtension),
                basePath, fileExtension));
        }

        [PlatformSpecificTestMethod("Windows")]
        [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
        [DataRow("test/path", "C:/data", ".dat", true)]
        [DataRow("../../path", "C:/data/test/path", ".xml", true)]
        [DataRow("../../path", "C:/data/test/path", ".dat", false)]
        [DataRow("../path", "C:/data/test/path", ".xml", false)]
        public void GivenRelativePathToFile__IsValidPath__IsExpected_WIN(string relativeDirectoryPath, string basePath,
            string fileExtension, bool expected) {
            const string fileName = "test";
            var svc = new IOService(_fileSystem);
            Assert.AreEqual(expected, svc.IsValidPath(
                _fileSystem.Path.Combine(relativeDirectoryPath, fileName + fileExtension),
                basePath, fileExtension));
        }
    }
}