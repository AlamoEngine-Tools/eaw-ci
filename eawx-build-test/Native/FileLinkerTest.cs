using System.IO.Abstractions;
using EawXBuild.Native;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Native {
    [TestClass]
    public class FileLinkerTest {
        private readonly IFileSystem _fileSystem = new FileSystem();
        private IDirectoryInfo _directoryInfo;
        private IFileInfo _sourceFileInfo;
        private IFileInfo _targetFileInfo;

        private const string FolderPath = "folder";
        private const string UnixFilePath = FolderPath + "/testFile.txt";
        private const string UnixLinkedFilePath = FolderPath + "/linkedFile.txt";
        private const string WinFilePath = FolderPath + @"\testFile.txt";
        private const string WinLinkedFilePath = FolderPath + @"\linkedFile.txt";

        [TestInitialize]
        public void SetUp() {
            _directoryInfo = _fileSystem.DirectoryInfo.FromDirectoryName(FolderPath);
            _directoryInfo.Create();
            _sourceFileInfo = _fileSystem.FileInfo.FromFileName(GetPlatformSourcePath());
            _targetFileInfo = _fileSystem.FileInfo.FromFileName(GetPlatformTargetPath());
            using var stream = _sourceFileInfo.Create();
        }

        [TestCleanup]
        public void TearDown() {
            _sourceFileInfo.Delete();
            _targetFileInfo.Delete();
            _directoryInfo.Delete();
            Assert.IsFalse(_fileSystem.File.Exists(GetPlatformSourcePath()));
            Assert.IsFalse(_fileSystem.File.Exists(GetPlatformTargetPath()));
        }

        [PlatformSpecificTestMethod("OSX")]
        public void GivenRunningMacOS__WhenLinkingFileWithUnixStylePath__FileShouldExist() {
            var sut = new MacOSFileLinker();

            sut.CreateLink(UnixFilePath, UnixLinkedFilePath);

            Assert.IsTrue(_fileSystem.File.Exists(UnixLinkedFilePath));
        }

        [PlatformSpecificTestMethod("OSX")]
        public void GivenRunningMacOS__WhenLinkingFileWithWinStylePath__FileShouldExist() {
            var sut = new MacOSFileLinker();

            sut.CreateLink(WinFilePath, WinLinkedFilePath);

            Assert.IsTrue(_fileSystem.File.Exists(UnixLinkedFilePath));
        }

        [PlatformSpecificTestMethod("Windows")]
        public void GivenRunningWindows__WhenLinkingFileWithUnixStylePath__FileShouldExist() {
            var sut = new WinFileLinker();

            sut.CreateLink(UnixFilePath, UnixLinkedFilePath);

            Assert.IsTrue(_fileSystem.File.Exists(WinLinkedFilePath));
        }

        [PlatformSpecificTestMethod("Windows")]
        public void GivenRunningWindows__WhenLinkingFileWithWinStylePath__FileShouldExist() {
            var sut = new WinFileLinker();

            sut.CreateLink(WinFilePath, WinLinkedFilePath);

            Assert.IsTrue(_fileSystem.File.Exists(WinLinkedFilePath));
        }

        [PlatformSpecificTestMethod("Linux")]
        public void GivenRunningLinux__WhenLinkingFileWithUnixStylePath__FileShouldExist() {
            var sut = new LinuxFileLinker();

            sut.CreateLink(UnixFilePath, UnixLinkedFilePath);

            Assert.IsTrue(_fileSystem.File.Exists(UnixLinkedFilePath));
        }

        [PlatformSpecificTestMethod("Linux")]
        public void GivenRunningLinux__WhenLinkingFileWithWinStylePath__FileShouldExist() {
            var sut = new LinuxFileLinker();

            sut.CreateLink(WinFilePath, WinLinkedFilePath);

            Assert.IsTrue(_fileSystem.File.Exists(UnixLinkedFilePath));
        }

        private static string GetPlatformSourcePath() {
            return TestUtility.IsWindows() ? WinFilePath : UnixFilePath;
        }

        private static string GetPlatformTargetPath() {
            return TestUtility.IsWindows() ? WinLinkedFilePath : UnixLinkedFilePath;
        }
    }
}