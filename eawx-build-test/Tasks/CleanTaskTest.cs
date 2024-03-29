using System.IO.Abstractions.TestingHelpers;
using EawXBuild.Exceptions;
using EawXBuild.Tasks;
using EawXBuildTest.Reporting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static EawXBuildTest.ReportingAssertions;

namespace EawXBuildTest.Tasks
{
    [TestClass]
    public class CleanTaskTest
    {
        private FileSystemAssertions _assertions;
        private MockFileSystem _fileSystem;
        private CleanTask _sut;

        [TestInitialize]
        public void SetUp()
        {
            _fileSystem = new MockFileSystem();
            _assertions = new FileSystemAssertions(_fileSystem);
            _sut = new CleanTask(_fileSystem);
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
        public void GivenPathToFile__WhenCallingRun__ShouldDeleteFile()
        {
            const string dirPath = "Data";
            const string filePath = "Data/MyFile.txt";

            _fileSystem.AddDirectory(dirPath);
            _fileSystem.AddFile(filePath, new MockFileData(string.Empty));

            _sut.Path = filePath;

            _sut.Run();

            _assertions.AssertFileDoesNotExist(filePath);
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
        public void GivenPathToDirectory__WhenCallingRun__ShouldDeleteDirectory()
        {
            const string dirPath = "Data";

            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(dirPath);

            _sut.Path = dirPath;

            _sut.Run();

            _assertions.AssertDirectoryDoesNotExist(dirPath);
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
        public void GivenPathToDirectoryWithFiles__WhenCallingRun__ShouldDeleteDirectory()
        {
            const string dirPath = "Data";
            const string filePath = "Data/MyFile.txt";

            _fileSystem.AddDirectory(dirPath);
            _fileSystem.AddFile(filePath, new MockFileData(string.Empty));

            _sut.Path = dirPath;

            _sut.Run();

            _assertions.AssertDirectoryDoesNotExist(dirPath);
        }

        [TestMethod]
        [ExpectedException(typeof(NoRelativePathException))]
        public void GivenAbsolutePath__WhenCallingRun__ShouldThrowNoRelativePathException()
        {
            _sut.Path = "/absolute/path";

            _sut.Run();
        }

        [TestMethod]
        public void GivenFileToDelete__WhenCallingRun__ShouldReportDeletion()
        {
            const string filePath = "Data/MyFile.txt";
            _fileSystem.AddFile(filePath, new MockFileData(string.Empty));

            var report = new ReportSpy();

            _sut.Path = filePath;

            _sut.Run(report);

            var actualMessage = report.Messages[0];
            AssertMessageContentEquals($"Deleting file {filePath}", actualMessage);
        }
    }
}