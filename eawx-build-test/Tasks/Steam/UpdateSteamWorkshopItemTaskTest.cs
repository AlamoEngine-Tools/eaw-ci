using System;
using System.Collections.Generic;
using EawXBuild.Exceptions;
using EawXBuild.Steam;
using EawXBuild.Steam.Facepunch.Adapters;
using EawXBuild.Tasks.Steam;
using EawXBuildTest.Steam;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Tasks.Steam
{
    [TestClass]
    public class UpdateSteamWorkshopItemTaskTest
    {
        private const uint AppId = 32470;
        private const ulong ItemId = 1234;
        private const string Title = "My Workshop Item";
        private const string DescriptionFilePath = "path/to/description";
        private const string ExpectedDirectoryName = "path/to/directory";
        private readonly HashSet<string> ExpectedTags = new HashSet<string> {"EAW", "FOC"};


        [TestMethod]
        public void
            GivenTaskWithItemId_Title_Description_Folder_And_Visibility__WhenRunningTask__ShouldPublishWithSettings()
        {
            WorkshopItemSpy workshopItemSpy = new WorkshopItemSpy();
            SteamWorkshopSpy workshopSpy = new SteamWorkshopSpy
            {
                WorkshopItemsById = {{ItemId, workshopItemSpy}}
            };

            UpdateSteamWorkshopItemTask sut = new UpdateSteamWorkshopItemTask(workshopSpy)
            {
                AppId = AppId,
                ItemId = ItemId,
                ChangeSet = new WorkshopItemChangeSetStub
                {
                    ChangeSetValidationResult = (true, null),

                    Title = Title,
                    DescriptionFilePath = DescriptionFilePath,
                    ItemFolderPath = ExpectedDirectoryName,
                    Visibility = WorkshopItemVisibility.Public,
                    Tags = ExpectedTags
                }
            };

            sut.Run();

            IWorkshopItemChangeSet actual = workshopItemSpy.ReceivedSettings;
            Assert.AreEqual(AppId, workshopSpy.AppId);
            Assert.AreEqual(Title, actual.Title);
            Assert.AreEqual(DescriptionFilePath, actual.DescriptionFilePath);
            Assert.AreEqual(WorkshopItemVisibility.Public, actual.Visibility);
            Assert.AreEqual(ExpectedDirectoryName, actual.ItemFolderPath);
            Assert.AreEqual(ExpectedTags, actual.Tags);
        }

        [TestMethod]
        public void GivenValidTask__WhenRunningTask__ShouldWaitForItemQueryToFinishBeforeUpdating()
        {
            VerifySteamClientAndWorkshopItemCallOrderMock callOrderMock =
                new VerifySteamClientAndWorkshopItemCallOrderMock();

            UpdateSteamWorkshopItemTask sut = new UpdateSteamWorkshopItemTask(callOrderMock)
            {
                AppId = AppId,
                ItemId = ItemId,
                ChangeSet = new WorkshopItemChangeSetStub
                {
                    ChangeSetValidationResult = (true, null)
                }
            };

            sut.Run();

            callOrderMock.Verify();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GivenTaskWithoutChangeSet__WhenRunningTask__ShouldThrowInvalidOperationException()
        {
            WorkshopItemStub workshopItemStub = new WorkshopItemStub();
            SteamWorkshopStub workshopStub = new SteamWorkshopStub
            {
                WorkshopItemsById = {{ItemId, workshopItemStub}}
            };

            UpdateSteamWorkshopItemTask sut = new UpdateSteamWorkshopItemTask(workshopStub)
            {
                ItemId = ItemId
            };

            sut.Run();
        }

        [TestMethod]
        [ExpectedException(typeof(ProcessFailedException))]
        public void GivenValidTask__WhenRunningTask_ButPublishFails__ShouldThrowProcessFailedException()
        {
            WorkshopItemStub workshopItemSpy = new WorkshopItemStub {Result = PublishResult.Failed};
            SteamWorkshopStub workshopStub = new SteamWorkshopStub
            {
                WorkshopItemsById = {{ItemId, workshopItemSpy}}
            };

            UpdateSteamWorkshopItemTask sut = new UpdateSteamWorkshopItemTask(workshopStub)
            {
                AppId = AppId,
                ItemId = ItemId,
                ChangeSet = CreateValidChangeSet()
            };

            sut.Run();
        }

        [TestMethod]
        public void GivenValidTask__WhenRunningTask_ButPublishFails__ShouldShutdownSteamClient()
        {
            WorkshopItemStub workshopItemSpy = new WorkshopItemStub {Result = PublishResult.Failed};
            SteamWorkshopSpy workshopSpy = new SteamWorkshopSpy
            {
                WorkshopItemsById = {{ItemId, workshopItemSpy}}
            };

            UpdateSteamWorkshopItemTask sut = new UpdateSteamWorkshopItemTask(workshopSpy)
            {
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
        public void GivenTaskWithNonExistingItemId__WhenRunningTask__ShouldThrowWorkshopItemNotFoundException()
        {
            SteamWorkshopStub workshopStub = new SteamWorkshopStub
            {
                WorkshopItemsById = {{ItemId, null}}
            };

            UpdateSteamWorkshopItemTask sut = new UpdateSteamWorkshopItemTask(workshopStub)
            {
                AppId = AppId,
                ItemId = ItemId,
                ChangeSet = CreateValidChangeSet()
            };

            sut.Run();
        }

        [TestMethod]
        public void GivenTaskWithNonExistingItemId__WhenRunningTask__ShouldShutDownSteamClient()
        {
            SteamWorkshopSpy workshopSpy = new SteamWorkshopSpy
            {
                WorkshopItemsById = {{ItemId, null}}
            };

            UpdateSteamWorkshopItemTask sut = new UpdateSteamWorkshopItemTask(workshopSpy)
            {
                AppId = AppId,
                ItemId = ItemId,
                ChangeSet = CreateValidChangeSet()
            };

            Action actual = () => sut.Run();
            Assert.ThrowsException<WorkshopItemNotFoundException>(actual);

            Assert.IsTrue(workshopSpy.WasShutdown);
        }

        [TestMethod]
        public void GivenTaskWithoutAppId__WhenRunningTask__ShouldThrowException()
        {
            SteamWorkshopDummy workshop = new SteamWorkshopDummy();
            UpdateSteamWorkshopItemTask sut = new UpdateSteamWorkshopItemTask(workshop)
            {
                ItemId = ItemId,
                ChangeSet = CreateValidChangeSet()
            };

            InvalidOperationException actual = Assert.ThrowsException<InvalidOperationException>(() => sut.Run());

            Assert.AreEqual("No AppId set", actual.Message);
        }

        [TestMethod]
        public void GivenTaskWithoutAppId__WhenRunningTask__ShouldNotInitWorkshop()
        {
            SteamWorkshopSpy workshop = new SteamWorkshopSpy();
            UpdateSteamWorkshopItemTask sut = new UpdateSteamWorkshopItemTask(workshop)
            {
                ItemId = ItemId,
                ChangeSet = CreateValidChangeSet()
            };

            InvalidOperationException actual = Assert.ThrowsException<InvalidOperationException>(() => sut.Run());

            Assert.IsFalse(workshop.WasInitialized);
        }

        [TestMethod]
        public void GivenTaskWithoutItemId__WhenRunningTask__ShouldThrowInvalidOperationException()
        {
            SteamWorkshopDummy workshopDummy = new SteamWorkshopDummy();
            UpdateSteamWorkshopItemTask sut = new UpdateSteamWorkshopItemTask(workshopDummy)
            {
                AppId = AppId,
                ChangeSet = CreateValidChangeSet()
            };

            InvalidOperationException actual = Assert.ThrowsException<InvalidOperationException>(() => sut.Run());

            Assert.AreEqual("No ItemId set", actual.Message);
        }

        [TestMethod]
        public void GivenTaskWithoutItemId__WhenRunningTask__ShouldNotInitWorkshop()
        {
            SteamWorkshopSpy workshop = new SteamWorkshopSpy();
            UpdateSteamWorkshopItemTask sut = new UpdateSteamWorkshopItemTask(workshop)
            {
                AppId = AppId,
                ChangeSet = CreateValidChangeSet()
            };

            InvalidOperationException actual = Assert.ThrowsException<InvalidOperationException>(() => sut.Run());

            Assert.IsFalse(workshop.WasInitialized);
        }

        [TestMethod]
        public void GivenTaskWithInvalidNewChangeSet__WhenRunningTask__ShouldThrowExceptionFromChangeSet()
        {
            WorkshopItemSpy workshopItemSpy = new WorkshopItemSpy();
            SteamWorkshopStub workshopStub = new SteamWorkshopStub
            {
                WorkshopItemsById = {{ItemId, workshopItemSpy}}
            };

            UpdateSteamWorkshopItemTask sut = new UpdateSteamWorkshopItemTask(workshopStub)
            {
                AppId = AppId,
                ItemId = ItemId,
                ChangeSet = new WorkshopItemChangeSetStub
                {
                    ChangeSetValidationResult = (false, new FormatException())
                }
            };

            Action actual = () => sut.Run();

            Assert.ThrowsException<FormatException>(actual);
        }

        [TestMethod]
        public void GivenTaskWithInvalidNewChangeSet__WhenRunningTask__ShouldNotUpdateItem()
        {
            WorkshopItemSpy workshopItemSpy = new WorkshopItemSpy();
            SteamWorkshopStub workshopStub = new SteamWorkshopStub
            {
                WorkshopItemsById = {{ItemId, workshopItemSpy}}
            };

            UpdateSteamWorkshopItemTask sut = new UpdateSteamWorkshopItemTask(workshopStub)
            {
                AppId = AppId,
                ItemId = ItemId,
                ChangeSet = new WorkshopItemChangeSetStub
                {
                    ChangeSetValidationResult = (false, new FormatException())
                }
            };

            Action actual = () => sut.Run();
            Assert.ThrowsException<FormatException>(actual);

            Assert.IsNull(workshopItemSpy.ReceivedSettings);
        }


        private static WorkshopItemChangeSetStub CreateValidChangeSet()
        {
            return new WorkshopItemChangeSetStub
            {
                ChangeSetValidationResult = (true, null)
            };
        }
    }
}