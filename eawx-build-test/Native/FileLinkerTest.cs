using System.IO.Abstractions;
using EawXBuild.Native;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Native {
    [TestClass]
    public class FileLinkerTest {
        private readonly IFileSystem _fileSystem = new FileSystem();
        private const string UnixFilePath = "./testFile.txt";
        private const string UnixLinkedFilePath = "./linkedFile.txt";
        
        private const string WinFilePath = @".\testFile.txt";
        private const string WinLinkedFilePath = @".\linkedFile.txt";

        [TestInitialize]
        public void SetUp() {
            _fileSystem.File.Create(UnixFilePath);
        }

        [TestCleanup]
        public void TearDown() {
            _fileSystem.File.Delete(UnixFilePath);
            _fileSystem.File.Delete(UnixLinkedFilePath);
            Assert.IsFalse(_fileSystem.File.Exists(UnixFilePath));
            Assert.IsFalse(_fileSystem.File.Exists(UnixLinkedFilePath));
        }

        [TestMethod]
        public void GivenRunningMacOS__WhenLinkingFileWithUnixStylePath__FileShouldExist() {
            if (!TestUtility.IsMacOS()) Assert.Inconclusive();
            
            var sut = new MacOSFileLinker();
            
            sut.CreateLink(UnixFilePath, UnixLinkedFilePath);
            
            Assert.IsTrue(_fileSystem.File.Exists(UnixLinkedFilePath));
        }
        
        [TestMethod]
        public void GivenRunningMacOS__WhenLinkingFileWithWinStylePath__FileShouldExist() {
            if (!TestUtility.IsMacOS()) Assert.Inconclusive();
            
            var sut = new MacOSFileLinker();
            
            sut.CreateLink(WinFilePath, WinLinkedFilePath);
            
            Assert.IsTrue(_fileSystem.File.Exists(UnixLinkedFilePath));
        }
        
        [TestMethod]
        public void GivenRunningWindows__WhenLinkingFileWithUnixStylePath__FileShouldExist() {
            if (!TestUtility.IsWindows()) Assert.Inconclusive();
            
            var sut = new WinFileLinker();
            
            sut.CreateLink(UnixFilePath, UnixLinkedFilePath);
            
            Assert.IsTrue(_fileSystem.File.Exists(WinLinkedFilePath));
        }
        
        [TestMethod]
        public void GivenRunningWindows__WhenLinkingFileWithWinStylePath__FileShouldExist() {
            if (!TestUtility.IsWindows()) Assert.Inconclusive();
            
            var sut = new WinFileLinker();
            
            sut.CreateLink(WinFilePath, WinLinkedFilePath);
            
            Assert.IsTrue(_fileSystem.File.Exists(WinLinkedFilePath));
        }
        
        [TestMethod]
        public void GivenRunningLinux__WhenLinkingFileWithUnixStylePath__FileShouldExist() {
            if (!TestUtility.IsLinux()) Assert.Inconclusive();
            
            var sut = new LinuxFileLinker();
            
            sut.CreateLink(UnixFilePath, UnixLinkedFilePath);
            
            Assert.IsTrue(_fileSystem.File.Exists(UnixLinkedFilePath));
        }
        
        [TestMethod]
        public void GivenRunningLinux__WhenLinkingFileWithWinStylePath__FileShouldExist() {
            if (!TestUtility.IsLinux()) Assert.Inconclusive();
            
            var sut = new LinuxFileLinker();
            
            sut.CreateLink(WinFilePath, WinLinkedFilePath);
            
            Assert.IsTrue(_fileSystem.File.Exists(UnixLinkedFilePath));
        }
    }
}