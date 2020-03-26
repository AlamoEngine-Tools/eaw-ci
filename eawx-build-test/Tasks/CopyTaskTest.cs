using System.IO.Abstractions.TestingHelpers;
using EawXBuild;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Tasks
{
    [TestClass]
    public class CopyTaskTest
    {

        private MockFileSystem _fileSystem;
        private EawXBuild.Tasks.CopyTask _sut;

        [TestInitialize]
        public void SetUp()
        {
            _fileSystem = new MockFileSystem();
            _sut = new EawXBuild.Tasks.CopyTask(_fileSystem);
        }

        [TestMethod]
        public void GivenPathToFileAndCopyDestination__WhenCallingRun__ShouldCreateFileAtDestination()
        {
            AddFile("Data/MyFile.txt", string.Empty);

            const string destination = "Copy/MyFile.txt";

            _sut.Source = "Data/MyFile.txt";
            _sut.Destination = destination;

            _sut.Run();

            AssertFileExist(destination);
        }

        [TestMethod]
        public void GivenPathToFileAndCopyDestination__WhenCallingRun__ShouldCopyFileToDestination()
        {
            const string sourceFile = "Data/MyFile.txt";
            const string destFile = "Copy/MyFile.txt";

            AddFile(sourceFile, "File Content");

            _sut.Source = sourceFile;
            _sut.Destination = destFile;

            _sut.Run();

            AssertFileContentsAreEqual(GetFile(sourceFile), GetFile(destFile));
        }

        [TestMethod]
        public void GivenPathToDirectory__WhenCallingRun__ShouldCreateDirectoryAtDestination()
        {
            const string sourceDirectory = "Data/SourceXML";
            const string destDirectory = "Copy/XML";

            _fileSystem.AddDirectory(sourceDirectory);

            _sut.Source = sourceDirectory;
            _sut.Destination = destDirectory;

            _sut.Run();

            AssertDirectoryExists(destDirectory);
        }

        [TestMethod]
        public void GivenPathToDirectoryWithFile__WhenCallingRun__ShouldCreateDirectoryWithFileAtDestination()
        {
            const string sourceDirectory = "Data/SourceXML";
            const string sourceFileName = "Data/SourceXML/MyFile.xml";

            const string destDirectory = "Copy/XML";
            const string destFileName = "Copy/XML/MyFile.xml";

            _fileSystem.AddDirectory(sourceDirectory);
            AddFile(sourceFileName, "File Content");

            _sut.Source = sourceDirectory;
            _sut.Destination = destDirectory;

            _sut.Run();

            AssertFileExist(destFileName);
            AssertFileContentsAreEqual(GetFile(sourceFileName), GetFile(destFileName));
        }

        [TestMethod]
        public void GivenPathToDirectoryWithSubDirectory__WhenCallingRun__ShouldAlsoCopySubDirectory()
        {
            const string sourceDirectory = "Data/SourceXML";
            const string sourceSubDirectory = "Data/SourceXML/SubDirectory";

            const string destDirectory = "Copy/XML";
            const string destSubDirectory = "Copy/XML/SubDirectory";

            _fileSystem.AddDirectory(sourceDirectory);
            _fileSystem.AddDirectory(sourceSubDirectory);

            _sut.Source = sourceDirectory;
            _sut.Destination = destDirectory;

            _sut.Run();

            AssertDirectoryExists(destSubDirectory);
        }

        [TestMethod]
        public void GivenPathToDirectoryWithTwoSubDirectoryLevels__WhenCallingRun__ShouldCopyBothLevels()
        {
            _fileSystem.AddDirectory("Data/SourceXML");
            _fileSystem.AddDirectory("Data/SourceXML/SubDirectory");
            _fileSystem.AddDirectory("Data/SourceXML/SubDirectory/SubDirectory2");

            _sut.Source = "Data/SourceXML";
            _sut.Destination = "Copy/XML";

            _sut.Run();

            AssertDirectoryExists("Copy/XML/SubDirectory/SubDirectory2");
        }

        [TestMethod]
        public void GivenPathToDirectoryWithSubDirectoryWithFile__WhenCallingRun__ShouldCopySubDirectoryWithFile()
        {
            _fileSystem.AddDirectory("Data/SourceXML");
            _fileSystem.AddDirectory("Data/SourceXML/SubDirectory");

            AddFile("Data/SourceXML/SubDirectory/MyFile.xml", "File Content");

            _sut.Source = "Data/SourceXML";
            _sut.Destination = "Copy/XML";

            _sut.Run();

            const string expected = "Data/SourceXML/SubDirectory/MyFile.xml";
            const string actual = "Copy/XML/SubDirectory/MyFile.xml";
            AssertFileExist(actual);
            AssertFileContentsAreEqual(GetFile(expected), GetFile(actual));
        }

        [TestMethod]
        public void GivenDirectoryWithSubDirectoryWithoutRecursive__WhenCallingRun__ShouldNotCopySubDirectory()
        {
            const string sourceDirectory = "Data/SourceXML";
            const string sourceSubDirectory = "Data/SourceXML/SubDirectory";

            const string destDirectory = "Copy/XML";

            _fileSystem.AddDirectory(sourceDirectory);
            _fileSystem.AddDirectory(sourceSubDirectory);

            _sut.Source = sourceDirectory;
            _sut.Destination = destDirectory;
            _sut.Recursive = false;

            _sut.Run();

            const string destSubDirectory = "Copy/XML/SubDirectory";
            AssertDirectoryDoesNotExist(destSubDirectory);
        }



        [TestMethod]
        [ExpectedException(typeof(NoSuchFileSystemObjectException))]
        public void GivenNonExistantSourcePath__WhenCallingRun__ShouldThrowNoSuchFileSystemObjectException()
        {
            _sut.Source = "NonExistantPath";
            _sut.Run();
        }

        private MockFileData GetFile(string path)
        {
            return _fileSystem.GetFile(path);
        }

        private void AddFile(string path, string content)
        {
            _fileSystem.AddFile(path, new MockFileData(content));
        }
        private void AssertFileExist(string expected)
        {
            Assert.IsTrue(_fileSystem.FileExists(expected), $"File {expected} should exist, but doesn'nt");
        }
        private static void AssertFileContentsAreEqual(MockFileData expected, MockFileData actual)
        {
            Assert.AreEqual(expected.Contents, actual.Contents);
        }

        private void AssertDirectoryExists(string directory)
        {
            Assert.IsTrue(_fileSystem.Directory.Exists(directory), $"Directory {directory} should exist, but doesn't.");
        }

        private void AssertDirectoryDoesNotExist(string directory)
        {
            Assert.IsFalse(_fileSystem.Directory.Exists(directory), $"Directory {directory} should not exist, but does.");
        }

    }
}
