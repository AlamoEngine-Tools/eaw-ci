using System.IO.Abstractions.TestingHelpers;
using EawXBuild.Services.IO;
using EawXBuildTest.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Services.IO
{
    [TestClass]
    public class IOServiceTest
    {
        private MockFileSystem _fileSystem;
        private FileSystemAssertions _assertions;


        [TestInitialize]
        public void SetUp()
        {
            if (TestUtility.IsWindows())
            {
                _fileSystem = new MockFileSystem();
                _fileSystem.AddDirectory("C:/data/test/path");
                _fileSystem.AddDirectory("C:/data/path");
                _fileSystem.AddFile("C:/data/test/path/test.dat", new MockFileData(string.Empty));
                _fileSystem.AddFile("C:/data/path/test.xml", new MockFileData(string.Empty));
                _assertions = new FileSystemAssertions(_fileSystem);
                _assertions.AssertDirectoryExists("C:/data/test/path");
                _assertions.AssertDirectoryExists("C:/data/path");
                _assertions.AssertFileExists("C:/data/test/path/test.dat");
                _assertions.AssertFileExists("C:/data/path/test.xml");
            }
            else if (TestUtility.IsLinuxOrMacOS())
            {
                _fileSystem = new MockFileSystem();
                _fileSystem.AddDirectory("/mnt/c/data/test/path");
                _fileSystem.AddDirectory("/mnt/c/data/path");
                _fileSystem.AddFile("/mnt/c/data/test/path/test.dat", new MockFileData(string.Empty));
                _fileSystem.AddFile("/mnt/c/data/path/test.xml", new MockFileData(string.Empty));
                _assertions = new FileSystemAssertions(_fileSystem);
                _assertions.AssertDirectoryExists("/mnt/c/data/test/path");
                _assertions.AssertDirectoryExists("/mnt/c/data/path");
                _assertions.AssertFileExists("/mnt/c/data/test/path/test.dat");
                _assertions.AssertFileExists("/mnt/c/data/path/test.xml");
            }
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
        [DataRow("C:/data/test/path", ".dat", true)]
        [DataRow("C:/data/test/path", ".xml", false)]
        [DataRow("C:/data/abba/path", ".xml", false)]
        [DataRow("C:/data/path", ".xml", true)]
        [DataRow("C:/da/path", ".xml", false)]
        [DataRow("C:/data/path", ".dat", false)]
        public void GivenAbsolutePathToFile__WithRootedPath__IsValidPath__IsExpected__WIN(string absoluteDirectoryPath,
            string fileExtension, bool expected)
        {
            if (TestUtility.IsWindows())
            {
                const string fileName = "test";
                IOService svc = new IOService(_fileSystem);
                Assert.AreEqual(expected, svc.IsValidPath(
                    _fileSystem.Path.Combine(absoluteDirectoryPath, fileName + fileExtension),
                    string.Empty, fileExtension));
            }
            else
            {
                Assert.Inconclusive("Test only runs on Windows.");
            }
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
        [DataRow("data/test/path", ".dat")]
        [DataRow("/test/path", ".xml")]
        [DataRow("abba/path", ".xml")]
        [DataRow("data/path", ".xml")]
        [DataRow("da/path", ".xml")]
        [DataRow("data/path", ".dat")]
        public void GivenRelativePathToFile__WithoutRootedPath__IsValidPath__IsFalse_WIN(string absoluteDirectoryPath,
            string fileExtension)
        {
            if (TestUtility.IsWindows())
            {
                const string fileName = "test";
                IOService svc = new IOService(_fileSystem);
                Assert.IsFalse(svc.IsValidPath(
                    _fileSystem.Path.Combine(absoluteDirectoryPath, fileName + fileExtension),
                    string.Empty, fileExtension));
            }
            else
            {
                Assert.Inconclusive("Test only runs on Windows.");
            }
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
        [DataRow("test/path", "C:/data", ".dat", true)]
        [DataRow("../../path", "C:/data/test/path", ".xml", true)]
        [DataRow("../../path", "C:/data/test/path", ".dat", false)]
        [DataRow("../path", "C:/data/test/path", ".xml", false)]
        public void GivenRelativePathToFile__IsValidPath__IsExpected_WIN(string relativeDirectoryPath, string basePath,
            string fileExtension, bool expected)
        {
            if (TestUtility.IsWindows())
            {
                const string fileName = "test";
                IOService svc = new IOService(_fileSystem);
                Assert.AreEqual(expected, svc.IsValidPath(
                    _fileSystem.Path.Combine(relativeDirectoryPath, fileName + fileExtension),
                    basePath, fileExtension));
            }
            else
            {
                Assert.Inconclusive("Test only runs on Windows.");
            }
        }
    }
}
