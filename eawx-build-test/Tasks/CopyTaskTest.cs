using System;
using System.IO.Abstractions.TestingHelpers;
using EawXBuild.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Tasks
{
    [TestClass]
    public class CopyTaskTest
    {
        private MockFileSystem _fileSystem;
        private FileSystemAssertions _assertions;
        private EawXBuild.Tasks.CopyTask _sut;

        [TestInitialize]
        public void SetUp()
        {
            _fileSystem = new MockFileSystem();
            _assertions = new FileSystemAssertions(_fileSystem);
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

            _assertions.AssertFileExists(destination);
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

            _assertions.AssertFileContentsAreEqual(GetFile(sourceFile), GetFile(destFile));
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

            _assertions.AssertDirectoryExists(destDirectory);
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

            _assertions.AssertFileExists(destFileName);
            _assertions.AssertFileContentsAreEqual(GetFile(sourceFileName), GetFile(destFileName));
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

            _assertions.AssertDirectoryExists(destSubDirectory);
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

            _assertions.AssertDirectoryExists("Copy/XML/SubDirectory/SubDirectory2");
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
            _assertions.AssertFileExists(actual);
            _assertions.AssertFileContentsAreEqual(GetFile(expected), GetFile(actual));
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
            _assertions.AssertDirectoryDoesNotExist(destSubDirectory);
        }

        [TestMethod]
        public void GivenDestFileExistsButSourceWasModifiedAfter__WhenCallingRun__ShouldOverrideDestFile()
        {
            const string sourceFile = "Data/MyFile.txt";
            const string destFile = "Copy/MyFile.txt";

            var newerWriteTime = new DateTimeOffset(new DateTime(2020, 3, 27), TimeSpan.Zero);
            AddFile(sourceFile, "New Content", newerWriteTime);
            
            var olderWriteTime = new DateTimeOffset(new DateTime(2020, 3, 26), TimeSpan.Zero);
            AddFile(destFile, "Old Content", olderWriteTime);

            _sut.Source = sourceFile;
            _sut.Destination = destFile;

            _sut.Run();

            _assertions.AssertFileContentsAreEqual(GetFile(sourceFile), GetFile(destFile));
        }

        [TestMethod]
        public void GivenDestFileExistAndHasNewerWriteTimeThanSourceFile__WhenCallingRun__ShouldNotOverrideDestFile()
        {
            const string sourceFile = "Data/MyFile.txt";
            const string destFile = "Copy/MyFile.txt";

            const string expectedContent = "New Content";
            
            var olderWriteTime = new DateTimeOffset(new DateTime(2020, 3, 26), TimeSpan.Zero);
            AddFile(sourceFile, "Old Content", olderWriteTime);
            
            var newerWriteTime = new DateTimeOffset(new DateTime(2020, 3, 27), TimeSpan.Zero);
            AddFile(destFile, expectedContent, newerWriteTime);

            _sut.Source = sourceFile;
            _sut.Destination = destFile;

            _sut.Run();
            
            _assertions.AssertFileContentEquals(expectedContent, GetFile(destFile));
        }
        
        [TestMethod]
        public void GivenDestDirectoryWithExistingFileButSourceWasModifiedAfter__WhenCallingRun__ShouldOverrideFileInDestDirectory()
        {
            const string sourceDir = "Data";
            const string sourceFile = "Data/Sub/MyFile.txt";

            const string destDir = "Copy";
            const string destFile = "Copy/Sub/MyFile.txt";
            
            AddFile(sourceFile, "New Content", NewerWriteTime);
            AddFile(destFile, "Old Content", OlderWriteTime);

            _sut.Source = sourceDir;
            _sut.Destination = destDir;

            _sut.Run();

            _assertions.AssertFileContentsAreEqual(GetFile(sourceFile), GetFile(destFile));
        }
        
        [TestMethod]
        public void GivenDestDirectoryWithExistingFileAndNewerWriteTimeThanSourceFile__WhenCallingRun__ShouldNotOverrideDestFile()
        {
            const string sourceDir = "Data";
            const string sourceFile = "Data/Sub/MyFile.txt";

            const string destDir = "Copy";
            const string destFile = "Copy/Sub/MyFile.txt";

            const string expectedContent = "New Content";
            
            AddFile(sourceFile, "Old Content", OlderWriteTime);
            AddFile(destFile, expectedContent, NewerWriteTime);

            _sut.Source = sourceDir;
            _sut.Destination = destDir;

            _sut.Run();
            
            _assertions.AssertFileContentEquals(expectedContent, GetFile(destFile));
        }

        [TestMethod]
        [ExpectedException(typeof(NoSuchFileSystemObjectException))]
        public void GivenNonExistingSourcePath__WhenCallingRun__ShouldThrowNoSuchFileSystemObjectException()
        {
            _sut.Source = "NonExistingPath";
            _sut.Run();
        }

        private MockFileData GetFile(string path)
        {
            return _fileSystem.GetFile(path);
        }
        
        private static DateTimeOffset NewerWriteTime => new DateTimeOffset(new DateTime(2020, 3, 27), TimeSpan.Zero);

        private static DateTimeOffset OlderWriteTime => new DateTimeOffset(new DateTime(2020, 3, 26), TimeSpan.Zero);

        private void AddFile(string path, string content)
        {
            AddFile(path, content, DateTimeOffset.MinValue);
        }
        
        private void AddFile(string sourceFile, string content, DateTimeOffset lastWriteTime)
        {
            var sourceFileData = new MockFileData(content);
            sourceFileData.LastWriteTime = lastWriteTime;
            _fileSystem.AddFile(sourceFile, sourceFileData);
        }
    }
}