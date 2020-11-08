using System;
using EawXBuild.Exceptions;
using EawXBuild.Steam;
using EawXBuild.Steam.Facepunch.Adapters;
using EawXBuild.Tasks;
using EawXBuildTest.Steam;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Tasks {
    [TestClass]
    public class UpdateSteamWorkshopItemTaskTest {
        private const uint ItemId = 1234;
        private const string Title = "My Workshop Item";
        private const string DescriptionFilePath = "path/to/description";
        private const string ExpectedDirectoryName = "path/to/directory";
        

        [TestMethod]
        public void
            GivenTaskWithItemId_Title_Description_Folder_And_Visibility__WhenRunningTask__ShouldPublishWithSettings() {
            var workshopItemSpy = new WorkshopItemSpy();
            var workshopStub = new SteamWorkshopStub {
                WorkshopItemsById = {{ItemId, workshopItemSpy}}
            };

            var sut = new UpdateSteamWorkshopItemTask(workshopStub) {
                ItemId = ItemId,
                ChangeSet = new WorkshopItemChangeSetStub {
                    ChangeSetValidationResult = (true, null),

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
        [ExpectedException(typeof(InvalidOperationException))]
        public void GivenTaskWithoutChangeSet__WhenRunningTask__ShouldThrowInvalidOperationException() {
            var workshopItemStub = new WorkshopItemStub();
            var workshopStub = new SteamWorkshopStub {
                WorkshopItemsById = {{ItemId, workshopItemStub}}
            };

            var sut = new UpdateSteamWorkshopItemTask(workshopStub) {
                ItemId = ItemId
            };

            sut.Run();
        }

        [TestMethod]
        [ExpectedException(typeof(ProcessFailedException))]
        public void GivenValidTask__WhenRunningTask__ShouldThrowProcessFailedException() {
            var workshopItemSpy = new WorkshopItemStub {Result = PublishResult.Failed};
            var workshopStub = new SteamWorkshopStub {
                WorkshopItemsById = {{ItemId, workshopItemSpy}}
            };

            var sut = new UpdateSteamWorkshopItemTask(workshopStub) {
                ItemId = ItemId,
                ChangeSet = MakeValidUpdateChangeSet()
            };

            sut.Run();
        }

        [TestMethod]
        [ExpectedException(typeof(WorkshopItemNotFoundException))]
        public void GivenTaskWithNonExistingItemId__WhenRunningTask__ShouldThrowWorkshopItemNotFoundException() {
            var workshopStub = new SteamWorkshopStub {
                WorkshopItemsById = {{ItemId, null}}
            };

            var sut = new UpdateSteamWorkshopItemTask(workshopStub) {
                ItemId = ItemId,
                ChangeSet = MakeValidUpdateChangeSet()
            };

            sut.Run();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GivenTaskWithoutItemId__WhenRunningTask__ShouldThrowInvalidOperationException() {
            var workshopDummy = new SteamWorkshopDummy();
            var sut = new UpdateSteamWorkshopItemTask(workshopDummy);

            sut.Run();
        }

        [TestMethod]
        public void GivenTaskWithInvalidNewChangeSet__WhenRunningTask__ShouldThrowExceptionFromChangeSet() {
            var workshopItemSpy = new WorkshopItemSpy();
            var workshopStub = new SteamWorkshopStub {
                WorkshopItemsById = {{ItemId, workshopItemSpy}}
            };

            var sut = new UpdateSteamWorkshopItemTask(workshopStub) {
                ItemId = ItemId,
                ChangeSet = new WorkshopItemChangeSetStub {
                    ChangeSetValidationResult = (false, new InvalidOperationException())
                }
            };

            Action actual = () => sut.Run();

            Assert.ThrowsException<InvalidOperationException>(actual);
        }

        [TestMethod]
        public void GivenTaskWithInvalidNewChangeSet__WhenRunningTask__ShouldNotUpdateItem() {
            var workshopItemSpy = new WorkshopItemSpy();
            var workshopStub = new SteamWorkshopStub {
                WorkshopItemsById = {{ItemId, workshopItemSpy}}
            };

            var sut = new UpdateSteamWorkshopItemTask(workshopStub) {
                ItemId = ItemId,
                ChangeSet = new WorkshopItemChangeSetStub {
                    ChangeSetValidationResult = (false, new InvalidOperationException())
                }
            };

            Action actual = () => sut.Run();
            Assert.ThrowsException<InvalidOperationException>(actual);

            Assert.IsNull(workshopItemSpy.ReceivedSettings);
        }
        
        private static WorkshopItemChangeSetStub MakeValidUpdateChangeSet() {
            return new WorkshopItemChangeSetStub {
                ChangeSetValidationResult = (true, null)
            };
        }
    }
}