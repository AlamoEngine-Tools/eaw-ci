using System;
using System.Collections.Generic;
using EawXBuild.Exceptions;
using EawXBuild.Steam;
using EawXBuild.Steam.Facepunch.Adapters;
using EawXBuild.Tasks.Steam;
using EawXBuildTest.Steam;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Tasks.Steam {
    [TestClass]
    public class UpdateSteamWorkshopItemTaskTest {
        private const uint AppId = 32470;
        private const ulong ItemId = 1234;
        private const string Title = "My Workshop Item";
        private const string DescriptionFilePath = "path/to/description";
        private const string ExpectedDirectoryName = "path/to/directory";
        private readonly HashSet<string> ExpectedTags = new HashSet<string> {"EAW", "FOC"};


        [TestMethod]
        public void
            GivenTaskWithItemId_Title_Description_Folder_And_Visibility__WhenRunningTask__ShouldPublishWithSettings() {
            var workshopItemSpy = new WorkshopItemSpy();
            var workshopSpy = new SteamWorkshopSpy {
                WorkshopItemsById = {{ItemId, workshopItemSpy}}
            };

            var sut = new UpdateSteamWorkshopItemTask(workshopSpy) {
                AppId = AppId,
                ItemId = ItemId,
                ChangeSet = new WorkshopItemChangeSetStub {
                    ChangeSetValidationResult = (true, null),

                    Title = Title,
                    DescriptionFilePath = DescriptionFilePath,
                    ItemFolderPath = ExpectedDirectoryName,
                    Visibility = WorkshopItemVisibility.Public,
                    Tags = ExpectedTags
                }
            };

            sut.Run();

            var actual = workshopItemSpy.ReceivedSettings;
            Assert.AreEqual(AppId, workshopSpy.AppId);
            Assert.AreEqual(Title, actual.Title);
            Assert.AreEqual(DescriptionFilePath, actual.DescriptionFilePath);
            Assert.AreEqual(WorkshopItemVisibility.Public, actual.Visibility);
            Assert.AreEqual(ExpectedDirectoryName, actual.ItemFolderPath);
            Assert.AreEqual(ExpectedTags, actual.Tags);
        }

        [TestMethod]
        public void GivenValidTask__WhenRunningTask__ShouldWaitForItemQueryToFinishBeforeUpdating() {
            var callOrderMock = new VerifySteamClientAndWorkshopItemCallOrderMock();

            var sut = new UpdateSteamWorkshopItemTask(callOrderMock) {
                AppId = AppId,
                ItemId = ItemId,
                ChangeSet = new WorkshopItemChangeSetStub {
                    ChangeSetValidationResult = (true, null)
                }
            };

            sut.Run();

            callOrderMock.Verify();
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
        public void GivenValidTask__WhenRunningTask_ButPublishFails__ShouldThrowProcessFailedException() {
            var workshopItemSpy = new WorkshopItemStub {Result = PublishResult.Failed};
            var workshopStub = new SteamWorkshopStub {
                WorkshopItemsById = {{ItemId, workshopItemSpy}}
            };

            var sut = new UpdateSteamWorkshopItemTask(workshopStub) {
                AppId = AppId,
                ItemId = ItemId,
                ChangeSet = CreateValidChangeSet()
            };

            sut.Run();
        }

        [TestMethod]
        public void GivenValidTask__WhenRunningTask_ButPublishFails__ShouldShutdownSteamClient() {
            var workshopItemSpy = new WorkshopItemStub {Result = PublishResult.Failed};
            var workshopSpy = new SteamWorkshopSpy {
                WorkshopItemsById = {{ItemId, workshopItemSpy}}
            };

            var sut = new UpdateSteamWorkshopItemTask(workshopSpy) {
                AppId = AppId,
                ItemId = ItemId,
                ChangeSet = CreateValidChangeSet()
            };

            Action actual = () => sut.Run();
            Assert.ThrowsException<ProcessFailedException>(actual);

            Assert.IsTrue(workshopSpy.WasShutdown);
        }

        [TestMethod]
        [ExpectedException(typeof(WorkshopItemNotFoundException))]
        public void GivenTaskWithNonExistingItemId__WhenRunningTask__ShouldThrowWorkshopItemNotFoundException() {
            var workshopStub = new SteamWorkshopStub {
                WorkshopItemsById = {{ItemId, null}}
            };

            var sut = new UpdateSteamWorkshopItemTask(workshopStub) {
                AppId = AppId,
                ItemId = ItemId,
                ChangeSet = CreateValidChangeSet()
            };

            sut.Run();
        }

        [TestMethod]
        public void GivenTaskWithNonExistingItemId__WhenRunningTask__ShouldShutDownSteamClient() {
            var workshopSpy = new SteamWorkshopSpy {
                WorkshopItemsById = {{ItemId, null}}
            };

            var sut = new UpdateSteamWorkshopItemTask(workshopSpy) {
                AppId = AppId,
                ItemId = ItemId,
                ChangeSet = CreateValidChangeSet()
            };

            Action actual = () => sut.Run();
            Assert.ThrowsException<WorkshopItemNotFoundException>(actual);

            Assert.IsTrue(workshopSpy.WasShutdown);
        }

        [TestMethod]
        public void GivenTaskWithoutAppId__WhenRunningTask__ShouldThrowException() {
            var workshop = new SteamWorkshopDummy();
            var sut = new UpdateSteamWorkshopItemTask(workshop) {
                ItemId = ItemId,
                ChangeSet = CreateValidChangeSet()
            };

            var actual = Assert.ThrowsException<InvalidOperationException>(() => sut.Run());

            Assert.AreEqual("No AppId set", actual.Message);
        }

        [TestMethod]
        public void GivenTaskWithoutAppId__WhenRunningTask__ShouldNotInitWorkshop() {
            var workshop = new SteamWorkshopSpy();
            var sut = new UpdateSteamWorkshopItemTask(workshop) {
                ItemId = ItemId,
                ChangeSet = CreateValidChangeSet()
            };

            var actual = Assert.ThrowsException<InvalidOperationException>(() => sut.Run());

            Assert.IsFalse(workshop.WasInitialized);
        }

        [TestMethod]
        public void GivenTaskWithoutItemId__WhenRunningTask__ShouldThrowInvalidOperationException() {
            var workshopDummy = new SteamWorkshopDummy();
            var sut = new UpdateSteamWorkshopItemTask(workshopDummy) {
                AppId = AppId,
                ChangeSet = CreateValidChangeSet()
            };

            var actual = Assert.ThrowsException<InvalidOperationException>(() => sut.Run());

            Assert.AreEqual("No ItemId set", actual.Message);
        }

        [TestMethod]
        public void GivenTaskWithoutItemId__WhenRunningTask__ShouldNotInitWorkshop() {
            var workshop = new SteamWorkshopSpy();
            var sut = new UpdateSteamWorkshopItemTask(workshop) {
                AppId = AppId,
                ChangeSet = CreateValidChangeSet()
            };

            var actual = Assert.ThrowsException<InvalidOperationException>(() => sut.Run());

            Assert.IsFalse(workshop.WasInitialized);
        }

        [TestMethod]
        public void GivenTaskWithInvalidNewChangeSet__WhenRunningTask__ShouldThrowExceptionFromChangeSet() {
            var workshopItemSpy = new WorkshopItemSpy();
            var workshopStub = new SteamWorkshopStub {
                WorkshopItemsById = {{ItemId, workshopItemSpy}}
            };

            var sut = new UpdateSteamWorkshopItemTask(workshopStub) {
                AppId = AppId,
                ItemId = ItemId,
                ChangeSet = new WorkshopItemChangeSetStub {
                    ChangeSetValidationResult = (false, new FormatException())
                }
            };

            Action actual = () => sut.Run();

            Assert.ThrowsException<FormatException>(actual);
        }

        [TestMethod]
        public void GivenTaskWithInvalidNewChangeSet__WhenRunningTask__ShouldNotUpdateItem() {
            var workshopItemSpy = new WorkshopItemSpy();
            var workshopStub = new SteamWorkshopStub {
                WorkshopItemsById = {{ItemId, workshopItemSpy}}
            };

            var sut = new UpdateSteamWorkshopItemTask(workshopStub) {
                AppId = AppId,
                ItemId = ItemId,
                ChangeSet = new WorkshopItemChangeSetStub {
                    ChangeSetValidationResult = (false, new FormatException())
                }
            };

            Action actual = () => sut.Run();
            Assert.ThrowsException<FormatException>(actual);

            Assert.IsNull(workshopItemSpy.ReceivedSettings);
        }


        private static WorkshopItemChangeSetStub CreateValidChangeSet() {
            return new WorkshopItemChangeSetStub {
                ChangeSetValidationResult = (true, null)
            };
        }
    }
}