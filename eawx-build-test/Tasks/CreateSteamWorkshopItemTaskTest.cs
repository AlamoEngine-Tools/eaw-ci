using System;
using EawXBuild.Exceptions;
using EawXBuild.Steam;
using EawXBuildTest.Steam;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Tasks {
    [TestClass]
    public class CreateSteamWorkshopItemTaskTest {
        private const string Title = "My Workshop Item";
        private const string Description = "The description";
        private const string DescriptionFilePath = "path/to/description";
        private const string Language = "Spanish";
        private const string ExpectedDirectoryName = "path/to/directory";
        private const uint AppId = 32470;
        

        [TestMethod]
        public void
            GivenTaskWithAppId_Title_Description_Language_Folder_And_Visibility__WhenRunningTask__ShouldPublishWithSettings() {
            var workshopSpy = MakeSteamWorkshopSpy();
            var sut = new CreateSteamWorkshopItemTask(workshopSpy) {
                AppId = AppId,
                ChangeSet = new WorkshopItemChangeSetStub {
                    ChangeSetValidationResult = (true, null),

                    Title = Title,
                    DescriptionFilePath = DescriptionFilePath,
                    ItemFolderPath = ExpectedDirectoryName,
                    Language = Language,
                    Visibility = WorkshopItemVisibility.Public
                }
            };

            sut.Run();

            var actual = workshopSpy.ReceivedSettings;
            Assert.AreEqual(AppId, workshopSpy.AppId);
            Assert.AreEqual(Title, actual.Title);
            Assert.AreEqual(DescriptionFilePath, actual.DescriptionFilePath);
            Assert.AreEqual(Language, actual.Language);
            Assert.AreEqual(WorkshopItemVisibility.Public, actual.Visibility);
            Assert.AreEqual(ExpectedDirectoryName, actual.ItemFolderPath);
        }

        [TestMethod]
        public void
            GivenConfiguredTaskWithoutLanguage_And_Visibility__WhenRunningTask__ShouldPublishWith_EmptyDescription_PrivateVisibility_And_EnglishLanguage() {
            var workshopSpy = MakeSteamWorkshopSpy();
            var sut = new CreateSteamWorkshopItemTask(workshopSpy) {
                AppId = AppId,
                ChangeSet = CreateValidChangeSet()
            };

            sut.Run();

            var actual = workshopSpy.ReceivedSettings;
            AssertPublishedWithDefaultSettings(actual);
        }

        [TestMethod]
        public void
            GivenTaskWithAppId__WhenRunningTask__ShouldSetAppIdBeforePublishing() {
            var workshopSpy = MakeSteamWorkshopSpy();
            var sut = new CreateSteamWorkshopItemTask(workshopSpy) {
                AppId = AppId,
                ChangeSet = CreateValidChangeSet()
            };

            sut.Run();

            AssertAppIdSetBeforePublish(workshopSpy);
        }
        
        [TestMethod]
        public void GivenTaskWithInvalidNewChangeSet__WhenRunningTask__ShouldThrowExceptionFromChangeSet() {
            var workshopSpy = MakeSteamWorkshopSpy();
            var sut = new CreateSteamWorkshopItemTask(workshopSpy) {
                AppId = AppId,
                ChangeSet = CreateInvalidChangeSet()
            };

            Action actual = () => sut.Run();

            Assert.ThrowsException<InvalidOperationException>(actual);
        }

        [TestMethod]
        public void GivenTaskWithInvalidNewChangeSet__WhenRunningTask__ShouldNotPublishToSteamWorkshop() {
            var workshopSpy = MakeSteamWorkshopSpy();
            var sut = new CreateSteamWorkshopItemTask(workshopSpy) {
                AppId = AppId,
                ChangeSet = CreateInvalidChangeSet()
            };

            Action actual = () => sut.Run();
            Assert.ThrowsException<InvalidOperationException>(actual);

            Assert.IsNull(workshopSpy.ReceivedSettings);
        }

        [TestMethod]
        [ExpectedException(typeof(ProcessFailedException))]
        public void GivenValidTask__WhenCallingRun_ButPublishFails__ShouldThrowProcessFailedException() {
            var workshopStub = new SteamWorkshopStub {
                WorkshopItemPublishResult = new WorkshopItemPublishResult(AppId, PublishResult.Failed)
            };

            var sut = new CreateSteamWorkshopItemTask(workshopStub) {
                AppId = AppId,
                ChangeSet = CreateValidChangeSet()
            };

            sut.Run();
        }

        [TestMethod]
        public void GivenTaskWithoutAppId__WhenRunningTask__ShouldThrowException() {
            var workshop = new SteamWorkshopDummy();
            var sut = new CreateSteamWorkshopItemTask(workshop);

            var actual = Assert.ThrowsException<InvalidOperationException>(() => sut.Run());

            Assert.AreEqual("No AppId set", actual.Message);
        }

        [TestMethod]
        public void GivenTaskWithoutChangeSet__WhenRunningTask__ShouldThrowException() {
            var workshop = new SteamWorkshopDummy();
            var sut = new CreateSteamWorkshopItemTask(workshop) {AppId = AppId};

            var actual = Assert.ThrowsException<InvalidOperationException>(() => sut.Run());

            Assert.AreEqual("No change set given", actual.Message);
        }

        private static void AssertAppIdSetBeforePublish(SteamWorkshopSpy workshopSpy) {
            Assert.AreEqual("ap", workshopSpy.CallOrder);
        }

        private static void AssertPublishedWithDefaultSettings(IWorkshopItemChangeSet actual) {
            Assert.AreEqual("English", actual.Language);
            Assert.AreEqual(WorkshopItemVisibility.Private, actual.Visibility);
        }

        private static SteamWorkshopSpy MakeSteamWorkshopSpy() {
            return new SteamWorkshopSpy {
                WorkshopItemPublishResult = new WorkshopItemPublishResult(1, PublishResult.Ok)
            };
        }
        
        private static WorkshopItemChangeSetStub CreateValidChangeSet() {
            return new WorkshopItemChangeSetStub {
                ChangeSetValidationResult = (true, null)
            };
        }
        
        private static WorkshopItemChangeSetStub CreateInvalidChangeSet() {
            return new WorkshopItemChangeSetStub {
                ChangeSetValidationResult = (false, new InvalidOperationException())
            };
        }
    }
}