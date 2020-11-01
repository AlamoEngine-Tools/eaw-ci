using System.IO.Abstractions.TestingHelpers;
using EawXBuild.Steam;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Steam {
    [TestClass]
    public class WorkshopItemChangeSetTest {
        [TestMethod]
        public void
            GivenWorkshopItemChangeSetWithDescriptionFilePath__WhenCallingGetDescriptionTextFromFile__ShouldReturnDescriptionText() {
            const string expectedDescriptionText = "The description";
            var fileSystem = new MockFileSystem();
            fileSystem.AddFile("path/to/description", new MockFileData(expectedDescriptionText));
            var sut = new WorkshopItemChangeSet(fileSystem) {
                DescriptionFilePath = "path/to/description"
            };

            var actual = sut.GetDescriptionTextFromFile();

            Assert.AreEqual(expectedDescriptionText, actual);
        }
    }
}