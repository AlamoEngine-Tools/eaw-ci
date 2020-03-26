using System.IO.Abstractions.TestingHelpers;
using EawXBuild;
using NUnit.Framework;

namespace EawXBuildTest.Tasks
{
    class CleanTaskTest
    {
        [Test]
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

            Assert.False(fileSystem.FileExists(filePath), $"File {filePath} should not exist, but does.");
        }

        [Test]
        public void GivenPathToDirectory__WhenCallingRun__ShouldDeleteDirectory()
        {
            const string dirPath = "Data";

            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(dirPath);

            var sut = new CleanTask(fileSystem);
            sut.Path = dirPath;

            sut.Run();

            Assert.False(fileSystem.Directory.Exists(dirPath), $"Directory {dirPath} should not exist, but does.");
        }
    }
}