using System.IO.Abstractions.TestingHelpers;
using EawXBuild.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Tasks
{
    [TestClass]
    internal class CleanTaskTest
    {

        private MockFileSystem _fileSystem;
        private FileSystemAssertions _assertions;
        private CleanTask sut;

        [TestInitialize]
        public void SetUp()
        {
            _fileSystem = new MockFileSystem();
            _assertions = new FileSystemAssertions(_fileSystem);
            sut = new CleanTask(_fileSystem);
        }
        
        [TestMethod]
        public void GivenPathToFile__WhenCallingRun__ShouldDeleteFile()
        {
            const string dirPath = "Data";
            const string filePath = "Data/MyFile.txt";
            
            _fileSystem.AddDirectory(dirPath);
            _fileSystem.AddFile(filePath, new MockFileData(string.Empty));
            
            sut.Path = filePath;

            sut.Run();

            _assertions.AssertFileDoesNotExist(filePath);
        }

        [TestMethod]
        public void GivenPathToDirectory__WhenCallingRun__ShouldDeleteDirectory()
        {
            const string dirPath = "Data";

            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(dirPath);
            
            sut.Path = dirPath;

            sut.Run();

            _assertions.AssertDirectoryDoesNotExist(dirPath);
        }
    }
}