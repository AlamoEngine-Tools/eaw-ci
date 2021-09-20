using System;
using System.IO.Abstractions.TestingHelpers;
using EawXBuild.Exceptions;
using EawXBuild.Tasks;
using EawXBuildTest.Reporting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static EawXBuildTest.ReportingAssertions;

namespace EawXBuildTest.Tasks
{
    [TestClass]
    public class CopyTaskTest
    {
        private FileSystemAssertions _assertions;
        private MockFileSystem _fileSystem;
        private CopyTask _sut;

        private static DateTimeOffset NewerWriteTime => new DateTimeOffset(new DateTime(2020, 3, 27), TimeSpan.Zero);

        private static DateTimeOffset OlderWriteTime => new DateTimeOffset(new DateTime(2020, 3, 26), TimeSpan.Zero);

        [TestInitialize]
        public void SetUp()
        {
            _fileSystem = new MockFileSystem();
            _assertions = new FileSystemAssertions(_fileSystem);
            _sut = new CopyTask(new CopyPolicyFake(), _fileSystem);
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
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
        [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
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
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        public void GivenPathToFileAndCopyDestinationWithReport__WhenCallingRun__ShouldReportCopying()
        {
            const string source = "Data/MyFile.txt";
            const string destination = "Copy/MyFile.txt";

            AddFile(source, string.Empty);

            _sut.Source = source;
            _sut.Destination = destination;

            var report = new ReportSpy();
            _sut.Run(report);

            var message = report.Messages[0];
            var sourceFileInfo = _fileSystem.FileInfo.FromFileName(source);
            var destinationFileInfo = _fileSystem.FileInfo.FromFileName(destination);
            AssertMessageContentEquals($"Copying file {sourceFileInfo.FullName} to {destinationFileInfo.FullName}", message);
        }

        [TestMethod]
        public void GivenPathToFileAndDestination__WhenCallingRun__ShouldCopyUsingCopyPolicy()
        {
            var fileSystem = new MockFileSystemWithFileInfoCopySpy();
            var copyPolicySpy = new CopyPolicySpy();
            fileSystem.FileSystem.AddFile("Data/MyFile.txt", string.Empty);

            const string destination = "Copy/MyFile.txt";

            _sut = new CopyTask(copyPolicySpy, fileSystem) {Source = "Data/MyFile.txt", Destination = destination};

            _sut.Run();

            Assert.IsTrue(copyPolicySpy.CopyCalled);
            Assert.IsFalse(((FileInfoCopySpy) fileSystem.FileInfo.FromFileName("Data/MyFile.txt")).FileWasCopied);
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
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
        [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
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
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        public void GivenPathToDirectoryWithFileAndReport__WhenCallingRun__ShouldReportCopyingOfFile()
        {
            const string sourceDirectory = "Data/SourceXML";
            const string sourceFileName = "Data/SourceXML/MyFile.xml";

            const string destDirectory = "Copy/XML";
            const string destFileName = "Copy/XML/MyFile.xml";

            _fileSystem.AddDirectory(sourceDirectory);
            AddFile(sourceFileName, "File Content");

            _sut.Source = sourceDirectory;
            _sut.Destination = destDirectory;

            var report = new ReportSpy();
            _sut.Run(report);

            var message = report.Messages[0];
            var sourceFileInfo = _fileSystem.FileInfo.FromFileName(sourceFileName);
            var destinationFileInfo = _fileSystem.FileInfo.FromFileName(destFileName);
            AssertMessageContentEquals($"Copying file {sourceFileInfo.FullName} to {destinationFileInfo.FullName}", message);
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
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
        [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
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
        [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
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
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        public void GivenPathToDirectoryWithSubDirectoryAndFileAndReport__WhenCallingRun__ShouldReportCopyingOfFile()
        {
            _fileSystem.AddDirectory("Data/SourceXML");
            _fileSystem.AddDirectory("Data/SourceXML/SubDirectory");

            AddFile("Data/SourceXML/SubDirectory/MyFile.xml", "File Content");

            _sut.Source = "Data/SourceXML";
            _sut.Destination = "Copy/XML";

            var report = new ReportSpy();
            _sut.Run(report);

            var message = report.Messages[0];
            var sourceFileInfo = _fileSystem.FileInfo.FromFileName("Data/SourceXML/SubDirectory/MyFile.xml");
            var destinationFileInfo = _fileSystem.FileInfo.FromFileName("Copy/XML/SubDirectory/MyFile.xml");
            AssertMessageContentEquals($"Copying file {sourceFileInfo.FullName} to {destinationFileInfo.FullName}", message);
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
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
        [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
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
        [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
        public void
            GivenDestDirectoryWithExistingFileButSourceWasModifiedAfter__WhenCallingRun__ShouldOverrideFileInDestDirectory()
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
        [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
        public void
            GivenDestDirectoryWithExistingFileAndNewerWriteTimeThanSourceFile__WhenCallingRun__ShouldNotOverrideDestFile()
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
        public void
            GivenDestDirectoryAndNewerWriteTimeThanSourceFile_WithAlwaysOverwrite__WhenCallingRun__ShouldOverwriteDestFile()
        {
            const string sourceDir = "Data";
            const string sourceFile = "Data/Sub/MyFile.txt";

            const string destDir = "Copy";
            const string destFile = "Copy/Sub/MyFile.txt";

            const string expectedContent = "Old Content";

            AddFile(sourceFile, expectedContent, OlderWriteTime);
            AddFile(destFile, "New Content", NewerWriteTime);

            _sut.Source = sourceDir;
            _sut.Destination = destDir;
            _sut.AlwaysOverwrite = true;

            _sut.Run();

            _assertions.AssertFileContentEquals(expectedContent, GetFile(destFile));
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
        public void GivenFilePattern__WhenCallingRun__ShouldOnlyCopyFilesMatchingPattern()
        {
            const string sourceDir = "Data";
            const string firstFile = "Data/MyFile.txt";
            const string secondFile = "Data/MySecondFile.txt";
            const string thirdFile = "Data/MyThirdFile.cpp";

            const string destDir = "Copy";
            const string firstDestFile = "Copy/MyFile.txt";
            const string secondDestFile = "Copy/MySecondFile.txt";
            const string thirdDestFile = "Copy/MyThirdFile.cpp";

            AddFile(firstFile, "content");
            AddFile(secondFile, "content");
            AddFile(thirdFile, "content");

            _sut.Source = sourceDir;
            _sut.Destination = destDir;
            _sut.FilePattern = "*.txt";

            _sut.Run();


            _assertions.AssertFileExists(firstDestFile);
            _assertions.AssertFileExists(secondDestFile);

            _assertions.AssertFileDoesNotExist(thirdDestFile);
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
        [ExpectedException(typeof(NoRelativePathException))]
        public void GivenAbsolutePathAsSourceDir__WhenCallingRun__ShouldThrowNoRelativePathException()
        {
            const string sourceDir = "/absolute/path";
            _sut.Source = sourceDir;

            _sut.Run();
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
        [ExpectedException(typeof(NoRelativePathException))]
        public void GivenAbsolutePathAsDestDir__WhenCallingRun__ShouldThrowNoRelativePathException()
        {
            const string destDir = "/absolute/path";
            _sut.Destination = destDir;

            _sut.Run();
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
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