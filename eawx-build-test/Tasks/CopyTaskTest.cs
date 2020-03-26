using System.IO;
using System.IO.Abstractions.TestingHelpers;
using EawXBuild;
using EawXBuild.Tasks;
using NUnit.Framework;

namespace EawXBuildTest.Tasks
{
    class CopyTaskTest
    {

        private MockFileSystem fileSystem;
        private CopyTask sut;

        [SetUp]
        public void SetUp()
        {
            fileSystem = new MockFileSystem();
            sut = new CopyTask(fileSystem);
        }

        [Test]
        public void GivenPathToFileAndCopyDestination__WhenCallingRun__ShouldCreateFileAtDestination()
        {
            AddFile("Data/MyFile.txt", string.Empty);

            const string destination = "Copy/MyFile.txt";

            sut.Source = "Data/MyFile.txt";
            sut.Destination = destination;

            sut.Run();

            AssertFileExist(destination);
        }

        [Test]
        public void GivenPathToFileAndCopyDestination__WhenCallingRun__ShouldCopyFileToDestination()
        {
            const string sourceFile = "Data/MyFile.txt";
            const string destFile = "Copy/MyFile.txt";

            AddFile(sourceFile, "File Content");

            sut.Source = sourceFile;
            sut.Destination = destFile;

            sut.Run();

            AssertFileContentsAreEqual(GetFile(sourceFile), GetFile(destFile));
        }

        [Test]
        public void GivenPathToDirectory__WhenCallingRun__ShouldCreateDirectoryAtDestination()
        {
            const string sourceDirectory = "Data/SourceXML";
            const string destDirectory = "Copy/XML";

            fileSystem.AddDirectory(sourceDirectory);

            sut.Source = sourceDirectory;
            sut.Destination = destDirectory;

            sut.Run();

            AssertDirectoryExists(destDirectory);
        }

        [Test]
        public void GivenPathToDirectoryWithFile__WhenCallingRun__ShouldCreateDirectoryWithFileAtDestination()
        {
            const string sourceDirectory = "Data/SourceXML";
            const string sourceFileName = "Data/SourceXML/MyFile.xml";

            const string destDirectory = "Copy/XML";
            const string destFileName = "Copy/XML/MyFile.xml";

            fileSystem.AddDirectory(sourceDirectory);
            AddFile(sourceFileName, "File Content");

            sut.Source = sourceDirectory;
            sut.Destination = destDirectory;

            sut.Run();

            AssertFileExist(destFileName);
            AssertFileContentsAreEqual(GetFile(sourceFileName), GetFile(destFileName));
        }

        [Test]
        public void GivenPathToDirectoryWithSubDirectory__WhenCallingRun__ShouldAlsoCopySubDirectory()
        {
            const string sourceDirectory = "Data/SourceXML";
            const string sourceSubDirectory = "Data/SourceXML/SubDirectory";

            const string destDirectory = "Copy/XML";
            const string destSubDirectory = "Copy/XML/SubDirectory";

            fileSystem.AddDirectory(sourceDirectory);
            fileSystem.AddDirectory(sourceSubDirectory);

            sut.Source = sourceDirectory;
            sut.Destination = destDirectory;

            sut.Run();

            AssertDirectoryExists(destSubDirectory);
        }

        [Test]
        public void GivenPathToDirectoryWithTwoSubDirectoryLevels__WhenCallingRun__ShouldCopyBothLevels()
        {
            fileSystem.AddDirectory("Data/SourceXML");
            fileSystem.AddDirectory("Data/SourceXML/SubDirectory");
            fileSystem.AddDirectory("Data/SourceXML/SubDirectory/SubDirectory2");

            sut.Source = "Data/SourceXML";
            sut.Destination = "Copy/XML";

            sut.Run();

            AssertDirectoryExists("Copy/XML/SubDirectory/SubDirectory2");
        }

        [Test]
        public void GivenPathToDirectoryWithSubDirectoryWithFile__WhenCallingRun__ShouldCopySubDirectoryWithFile()
        {
            fileSystem.AddDirectory("Data/SourceXML");
            fileSystem.AddDirectory("Data/SourceXML/SubDirectory");

            AddFile("Data/SourceXML/SubDirectory/MyFile.xml", "File Content");

            sut.Source = "Data/SourceXML";
            sut.Destination = "Copy/XML";

            sut.Run();

            const string expected = "Data/SourceXML/SubDirectory/MyFile.xml";
            const string actual = "Copy/XML/SubDirectory/MyFile.xml";
            AssertFileExist(actual);
            AssertFileContentsAreEqual(GetFile(expected), GetFile(actual));
        }

        [Test]
        public void GivenDirectoryWithSubDirectoryWithoutRecursive__WhenCallingRun__ShouldNotCopySubDirectory()
        {
            const string sourceDirectory = "Data/SourceXML";
            const string sourceSubDirectory = "Data/SourceXML/SubDirectory";
            
            const string destDirectory = "Copy/XML";
            
            fileSystem.AddDirectory(sourceDirectory);
            fileSystem.AddDirectory(sourceSubDirectory);

            sut.Source = sourceDirectory;
            sut.Destination = destDirectory;
            sut.Recursive = false;

            sut.Run();

            const string destSubDirectory = "Copy/XML/SubDirectory";
            AssertDirectoryDoesNotExist(destSubDirectory);
        }

        [Test]
        public void GivenNonExistantSourcePath__WhenCallingRun__ShouldThrowNoSuchFileSystemObjectException()
        {
            sut.Source = "NonExistantPath";

            TestDelegate expectedFail = () => sut.Run();

            Assert.Throws<NoSuchFileSystemObjectException>(expectedFail);
        }

        private MockFileData GetFile(string path)
        {
            return fileSystem.GetFile(path);
        }

        private void AddFile(string path, string content)
        {
            fileSystem.AddFile(path, new MockFileData(content));
        }
        private void AssertFileExist(string expected)
        {
            Assert.True(fileSystem.FileExists(expected), $"File {expected} should exist, but doesn'nt");
        }
        private static void AssertFileContentsAreEqual(MockFileData expected, MockFileData actual)
        {
            Assert.AreEqual(expected.Contents, actual.Contents);
        }

        private void AssertDirectoryExists(string directory)
        {
            Assert.True(fileSystem.Directory.Exists(directory), $"Directory {directory} should exist, but doesn't.");
        }

        private void AssertDirectoryDoesNotExist(string directory)
        {
            Assert.False(fileSystem.Directory.Exists(directory), $"Directory {directory} should not exist, but does.");
        }

    }
}