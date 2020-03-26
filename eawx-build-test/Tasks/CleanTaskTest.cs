using System.IO.Abstractions.TestingHelpers;
using EawXBuild;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Tasks
{
    [TestClass]
    class CleanTaskTest
    {
        [TestMethod]
        public void GivenPathToFile__WhenCallingRun__ShouldDeleteFile()
        {
            const string dirPath = "Data";
            const string filePath = "Data/MyFile.txt";

            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(dirPath);
            fileSystem.AddFile(filePath, new MockFileData(string.Empty));

            var sut = new CleanTask(fileSystem);
            sut.Path = filePath;

            sut.Run();

            Assert.IsFalse(fileSystem.FileExists(filePath), $"File {filePath} should not exist, but does.");
        }

        [TestMethod]
        public void GivenPathToDirectory__WhenCallingRun__ShouldDeleteDirectory()
        {
            const string dirPath = "Data";

            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(dirPath);

            var sut = new CleanTask(fileSystem);
            sut.Path = dirPath;

            sut.Run();

            Assert.IsFalse(fileSystem.Directory.Exists(dirPath), $"Directory {dirPath} should not exist, but does.");
        }
    }
}