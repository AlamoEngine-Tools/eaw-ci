using System;
using System.Collections.Generic;
using EawXBuild.Exceptions;
using EawXBuild.Steam;
using EawXBuild.Tasks.Steam;
using EawXBuildTest.Steam;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Tasks.Steam
{
    [TestClass]
    public class CreateSteamWorkshopItemTaskTest
    {
        private const string Title = "My Workshop Item";
        private const string DescriptionFilePath = "path/to/description";
        private const string Language = "Spanish";
        private const string ExpectedDirectoryName = "path/to/directory";
        private const uint AppId = 32470;
        private readonly HashSet<string> ExpectedTags = new HashSet<string> {"EAW", "FOC"};

        private static CreateSteamWorkshopItemTask MakeSutWithWorkshopAndChangeSet(ISteamWorkshop workshop,
            IWorkshopItemChangeSet changeSet)
        {
            return new CreateSteamWorkshopItemTask(workshop)
            {
                AppId = AppId,
                ChangeSet = changeSet
            };
        }

        [TestMethod]
        public void
            GivenTaskWithAppId_Title_Description_Language_Folder_And_Visibility__WhenRunningTask__ShouldPublishWithSettings()
        {
            SteamWorkshopSpy workshopSpy = MakeSteamWorkshopSpy();
            WorkshopItemChangeSetStub changeSetStub = new WorkshopItemChangeSetStub
            {
                ChangeSetValidationResult = (true, null),

                Title = Title,
                DescriptionFilePath = DescriptionFilePath,
                ItemFolderPath = ExpectedDirectoryName,
                Language = Language,
                Visibility = WorkshopItemVisibility.Public,
                Tags = ExpectedTags
            };

            CreateSteamWorkshopItemTask sut = MakeSutWithWorkshopAndChangeSet(workshopSpy, changeSetStub);

            sut.Run();

            IWorkshopItemChangeSet actual = workshopSpy.ReceivedSettings;
            Assert.AreEqual(AppId, workshopSpy.AppId);
            Assert.AreEqual(Title, actual.Title);
            Assert.AreEqual(DescriptionFilePath, actual.DescriptionFilePath);
            Assert.AreEqual(Language, actual.Language);
            Assert.AreEqual(WorkshopItemVisibility.Public, actual.Visibility);
            Assert.AreEqual(ExpectedDirectoryName, actual.ItemFolderPath);
            Assert.AreEqual(ExpectedTags, actual.Tags);
        }

        [TestMethod]
        public void
            GivenConfiguredTaskWithoutLanguage_And_Visibility__WhenRunningTask__ShouldPublishWith_EmptyDescription_PrivateVisibility_And_EnglishLanguage()
        {
            SteamWorkshopSpy workshopSpy = MakeSteamWorkshopSpy();
            CreateSteamWorkshopItemTask sut = MakeSutWithWorkshopAndChangeSet(workshopSpy, CreateValidChangeSet());

            sut.Run();

            IWorkshopItemChangeSet actual = workshopSpy.ReceivedSettings;
            AssertPublishedWithDefaultSettings(actual);
        }

        [TestMethod]
        public void
            GivenTaskWithAppId__WhenRunningTask__ShouldSetAppIdThenPublishThenShutdown()
        {
            SteamWorkshopSpy workshopSpy = MakeSteamWorkshopSpy();
            CreateSteamWorkshopItemTask sut = MakeSutWithWorkshopAndChangeSet(workshopSpy, CreateValidChangeSet());

            sut.Run();

            AssertSetAppIdThenPublishThenShutdown(workshopSpy);
        }

        [TestMethod]
        public void
            GivenValidTask__WhenRunningTask__ShouldShutdownAfterPublishFinished()
        {
            VerifyAwaitPublishTaskMock workshopMock = new VerifyAwaitPublishTaskMock
            {
                WorkshopItemPublishResult = new WorkshopItemPublishResult(1, PublishResult.Ok)
            };
            CreateSteamWorkshopItemTask sut = MakeSutWithWorkshopAndChangeSet(workshopMock, CreateValidChangeSet());

            sut.Run();

            workshopMock.Verify();
        }

        [TestMethod]
        public void GivenTaskWithInvalidNewChangeSet__WhenRunningTask__ShouldThrowExceptionFromChangeSet()
        {
            SteamWorkshopSpy workshopSpy = MakeSteamWorkshopSpy();
            CreateSteamWorkshopItemTask sut = MakeSutWithWorkshopAndChangeSet(workshopSpy, CreateInvalidChangeSet());

            Action actual = () => sut.Run();

            Assert.ThrowsException<InvalidOperationException>(actual);
        }

        [TestMethod]
        public void GivenTaskWithInvalidNewChangeSet__WhenRunningTask__ShouldNotPublishToSteamWorkshop()
        {
            SteamWorkshopSpy workshopSpy = MakeSteamWorkshopSpy();
            CreateSteamWorkshopItemTask sut = MakeSutWithWorkshopAndChangeSet(workshopSpy, CreateInvalidChangeSet());

            Action actual = () => sut.Run();
            Assert.ThrowsException<InvalidOperationException>(actual);

            Assert.IsNull(workshopSpy.ReceivedSettings);
        }

        [TestMethod]
        [ExpectedException(typeof(ProcessFailedException))]
        public void GivenValidTask__WhenRunningTask_ButPublishFails__ShouldThrowProcessFailedException()
        {
            SteamWorkshopStub workshopStub = new SteamWorkshopStub
            {
                WorkshopItemPublishResult = new WorkshopItemPublishResult(AppId, PublishResult.Failed)
            };

            CreateSteamWorkshopItemTask sut = MakeSutWithWorkshopAndChangeSet(workshopStub, CreateValidChangeSet());

            sut.Run();
        }

        [TestMethod]
        public void
            GivenValidTask__WhenRunningTask_ButPublishFails__ShouldShutdownSteamClient()
        {
            SteamWorkshopSpy workshopSpy = new SteamWorkshopSpy
            {
                WorkshopItemPublishResult = new WorkshopItemPublishResult(AppId, PublishResult.Failed)
            };

            CreateSteamWorkshopItemTask sut = MakeSutWithWorkshopAndChangeSet(workshopSpy, CreateValidChangeSet());

            Action action = () => sut.Run();

            Assert.ThrowsException<ProcessFailedException>(action);
            Assert.IsTrue(workshopSpy.WasShutdown);
        }

        [TestMethod]
        public void GivenTaskWithoutAppId__WhenRunningTask__ShouldThrowException()
        {
            SteamWorkshopDummy workshop = new SteamWorkshopDummy();
            CreateSteamWorkshopItemTask sut = new CreateSteamWorkshopItemTask(workshop)
            {
                ChangeSet = CreateValidChangeSet()
            };

            InvalidOperationException actual = Assert.ThrowsException<InvalidOperationException>(() => sut.Run());

            Assert.AreEqual("No AppId set", actual.Message);
        }

        [TestMethod]
        public void GivenTaskWithoutChangeSet__WhenRunningTask__ShouldThrowException()
        {
            SteamWorkshopDummy workshop = new SteamWorkshopDummy();
            CreateSteamWorkshopItemTask sut = new CreateSteamWorkshopItemTask(workshop) {AppId = AppId};

            InvalidOperationException actual = Assert.ThrowsException<InvalidOperationException>(() => sut.Run());

            Assert.AreEqual("No change set given", actual.Message);
        }

        private static void AssertSetAppIdThenPublishThenShutdown(SteamWorkshopSpy workshopSpy)
        {
            Assert.AreEqual("apd", workshopSpy.CallOrder);
        }

        private static void AssertPublishedWithDefaultSettings(IWorkshopItemChangeSet actual)
        {
            Assert.AreEqual("English", actual.Language);
            Assert.AreEqual(WorkshopItemVisibility.Private, actual.Visibility);
        }

        private static SteamWorkshopSpy MakeSteamWorkshopSpy()
        {
            return new SteamWorkshopSpy
            {
                WorkshopItemPublishResult = new WorkshopItemPublishResult(1, PublishResult.Ok)
            };
        }

        private static WorkshopItemChangeSetStub CreateValidChangeSet()
        {
            return new WorkshopItemChangeSetStub
            {
                ChangeSetValidationResult = (true, null)
            };
        }

        private static WorkshopItemChangeSetStub CreateInvalidChangeSet()
        {
            return new WorkshopItemChangeSetStub
            {
                ChangeSetValidationResult = (false, new InvalidOperationException())
            };
        }
    }
}