using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using EawXBuild.Exceptions;
using EawXBuild.Steam;
using EawXBuild.Steam.Facepunch.Adapters;
using EawXBuild.Tasks;
using EawXBuildTest.Steam;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Tasks {
    [TestClass]
    public class UpdateSteamWorkshopItemTaskTest {
        private MockFileSystem _fileSystem;

        private const uint ItemId = 1234;
        private const string Title = "My Workshop Item";
        private const string DescriptionFilePath = "path/to/description";
        private const string ExpectedDirectoryName = "path/to/directory";

        [TestInitialize]
        public void SetUp() {
            _fileSystem = new MockFileSystem();
        }

        [TestMethod]
        public void
            GivenTaskWithItemId_Title_Description_Folder_And_Visibility__WhenRunningTask__ShouldPublishWithSettings() {
            _fileSystem.AddDirectory(ExpectedDirectoryName);
            _fileSystem.AddFile(DescriptionFilePath, new MockFileData(string.Empty));

            var workshopItemSpy = new WorkshopItemSpy();
            var workshopStub = new SteamWorkshopStub {
                WorkshopItemsById = {{ItemId, workshopItemSpy}}
            };

            var sut = new UpdateSteamWorkshopItemTask(workshopStub, _fileSystem) {
                ItemId = ItemId,
                ChangeSet = new WorkshopItemChangeSet(_fileSystem) {
                    Title = Title,
                    DescriptionFilePath = DescriptionFilePath,
                    ItemFolderPath = ExpectedDirectoryName,
                    Visibility = WorkshopItemVisibility.Public
                }
            };

            sut.Run();

            var actual = workshopItemSpy.ReceivedSettings;
            Assert.AreEqual(Title, actual.Title);
            Assert.AreEqual(DescriptionFilePath, actual.DescriptionFilePath);
            Assert.AreEqual(WorkshopItemVisibility.Public, actual.Visibility);
            Assert.AreEqual(ExpectedDirectoryName, actual.ItemFolderPath);
        }

        [TestMethod]
        [ExpectedException(typeof(ProcessFailedException))]
        public void GivenValidTask__WhenRunningTask__ShouldThrowProcessFailedException() {
            _fileSystem.AddDirectory(ExpectedDirectoryName);
            _fileSystem.AddFile(DescriptionFilePath, new MockFileData(string.Empty));

            var workshopItemSpy = new WorkshopItemStub {Result = PublishResult.Failed};
            var workshopStub = new SteamWorkshopStub {
                WorkshopItemsById = {{ItemId, workshopItemSpy}}
            };

            var sut = new UpdateSteamWorkshopItemTask(workshopStub, _fileSystem) {
                ItemId = ItemId,
                ChangeSet = new WorkshopItemChangeSet(_fileSystem) {
                    Title = Title,
                    DescriptionFilePath = DescriptionFilePath,
                    ItemFolderPath = ExpectedDirectoryName,
                    Visibility = WorkshopItemVisibility.Public
                }
            };

            sut.Run();
        }

        [TestMethod]
        [ExpectedException(typeof(WorkshopItemNotFoundException))]
        public void GivenTaskWithNonExistingItemId__WhenRunningTask__ShouldThrowWorkshopItemNotFoundException() {
            _fileSystem.AddDirectory(ExpectedDirectoryName);

            var workshopStub = new SteamWorkshopStub {
                WorkshopItemsById = {{ItemId, null}}
            };

            var sut = new UpdateSteamWorkshopItemTask(workshopStub, _fileSystem) {
                ItemId = ItemId,
                ChangeSet = new WorkshopItemChangeSet(_fileSystem) {
                    ItemFolderPath = ExpectedDirectoryName
                }
            };

            sut.Run();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GivenTaskWithoutItemId__WhenRunningTask__ShouldThrowInvalidOperationException() {
            var workshopDummy = new SteamWorkshopDummy();
            var sut = new UpdateSteamWorkshopItemTask(workshopDummy, _fileSystem);

            sut.Run();
        }

        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void GivenTaskWithNonExistingItemFolder__WhenRunningTask__ShouldThrowDirectoryNotFoundException() {
            var workshopItemSpy = new WorkshopItemSpy();
            var workshopStub = new SteamWorkshopStub {
                WorkshopItemsById = {{ItemId, workshopItemSpy}}
            };

            var sut = new UpdateSteamWorkshopItemTask(workshopStub, _fileSystem) {
                ItemId = ItemId,
                ChangeSet = new WorkshopItemChangeSet(_fileSystem) {
                    ItemFolderPath = ExpectedDirectoryName
                }
            };

            sut.Run();
        }
        
        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void GivenTaskWithNonExistingDescriptionFile__WhenRunningTask__ShouldThrowFileNotFoundException() {
            var workshopItemSpy = new WorkshopItemSpy();
            var workshopStub = new SteamWorkshopStub {
                WorkshopItemsById = {{ItemId, workshopItemSpy}}
            };

            var sut = new UpdateSteamWorkshopItemTask(workshopStub, _fileSystem) {
                ItemId = ItemId,
                ChangeSet = new WorkshopItemChangeSet(_fileSystem) {
                    DescriptionFilePath = DescriptionFilePath
                }
            };

            sut.Run();
        }
    }
}