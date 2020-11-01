using System.IO.Abstractions.TestingHelpers;
using EawXBuild.Steam;
using EawXBuild.Tasks;
using EawXBuildTest.Steam;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Tasks {
    [TestClass]
    public class UpdateSteamWorkshopItemTaskTest {
        private MockFileSystem _fileSystem;

        private const uint ItemId = 1234;
        private const string Title = "My Workshop Item";
        private const string Description = "The description";
        private const string DescriptionFilePath = "path/to/description";
        private const string ExpectedDirectoryName = "path/to/directory";

        [TestInitialize]
        public void SetUp() {
            _fileSystem = new MockFileSystem();
        }


        [TestMethod]
        public void
            GivenTaskWithAppId_Title_Description_Language_Folder_And_Visibility__WhenRunningTask__ShouldPublishWithSettings() {
            _fileSystem.AddDirectory(ExpectedDirectoryName);
            _fileSystem.AddFile(DescriptionFilePath, new MockFileData(Description));

            var workshopItemSpy = new WorkshopItemSpy();
            var workshopSpy = new SteamWorkshopStub {
                WorkshopItemsById = {{ItemId, workshopItemSpy}}
            };

            var sut = new UpdateSteamWorkshopItemTask(workshopSpy, _fileSystem) {
                ItemId = ItemId,
                Title = Title,
                DescriptionFilePath = DescriptionFilePath,
                ItemFolderPath = ExpectedDirectoryName,
                Visibility = WorkshopItemVisibility.Public
            };

            sut.Run();

            var expectedDirectory = _fileSystem.DirectoryInfo.FromDirectoryName(ExpectedDirectoryName);

            var actual = workshopItemSpy.ReceivedSettings;
            Assert.AreEqual(Title, actual.Title);
            Assert.AreEqual(Description, actual.Description);
            Assert.AreEqual(WorkshopItemVisibility.Public, actual.Visibility);
            Assert.AreEqual(expectedDirectory.FullName, actual.ItemFolder.FullName);
        }
    }
}