using System.IO.Abstractions.TestingHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Tasks {
    public class FileSystemAssertions {
        private readonly MockFileSystem _fileSystem;

        public FileSystemAssertions(MockFileSystem fileSystem) {
            _fileSystem = fileSystem;
        }

        public void AssertDirectoryDoesNotExist(string directory) {
            Assert.IsFalse(_fileSystem.Directory.Exists(directory),
                $"Directory {directory} should not exist, but does.");
        }

        public void AssertDirectoryExists(string directory) {
            Assert.IsTrue(_fileSystem.Directory.Exists(directory), $"Directory {directory} should exist, but doesn't.");
        }

        public void AssertFileExists(string expected) {
            Assert.IsTrue(_fileSystem.FileExists(expected), $"File {expected} should exist, but doesn't");
        }

        public void AssertFileDoesNotExist(string filePath) {
            Assert.IsFalse(_fileSystem.FileExists(filePath), $"File {filePath} should not exist, but does.");
        }

        public void AssertFileContentEquals(string expectedContent, MockFileData actual) {
            Assert.AreEqual(expectedContent, actual.TextContents,
                $"Expected content of file to be '{expectedContent}', but was '{actual.TextContents}'");
        }

        public void AssertFileContentsAreEqual(MockFileData expected, MockFileData actual) {
            CollectionAssert.AreEqual(expected.Contents, actual.Contents);
        }
    }
}