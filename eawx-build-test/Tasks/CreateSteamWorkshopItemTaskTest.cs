using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using EawXBuild.Exceptions;
using EawXBuild.Steam;
using EawXBuildTest.Steam;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Tasks {
    [TestClass]
    public class CreateSteamWorkshopItemTaskTest {
        private MockFileSystem _fileSystem;

        private const string Title = "My Workshop Item";
        private const string Description = "The description";
        private const string DescriptionFilePath = "path/to/description";
        private const string Language = "Spanish";
        private const string ExpectedDirectoryName = "path/to/directory";
        private const uint AppId = 32470;

        [TestInitialize]
        public void SetUp() {
            _fileSystem = new MockFileSystem();
        }

        [TestMethod]
        public void
            GivenTaskWithAppId_Title_Description_Language_Folder_And_Visibility__WhenRunningTask__ShouldPublishWithSettings() {
            _fileSystem.AddDirectory(ExpectedDirectoryName);
            _fileSystem.AddFile(DescriptionFilePath, new MockFileData(Description));

            var workshopSpy = MakeSteamWorkshopSpy();
            var sut = new CreateSteamWorkshopItemTask(workshopSpy, _fileSystem) {
                AppId = AppId,
                ChangeSet = new WorkshopItemChangeSet(_fileSystem) {
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
            _fileSystem.AddDirectory(ExpectedDirectoryName);

            var workshopSpy = MakeSteamWorkshopSpy();
            var sut = new CreateSteamWorkshopItemTask(workshopSpy, _fileSystem) {
                AppId = AppId,
                ChangeSet = new WorkshopItemChangeSet(_fileSystem) {
                    Title = Title,
                    ItemFolderPath = ExpectedDirectoryName
                }
            };

            sut.Run();

            var actual = workshopSpy.ReceivedSettings;
            AssertPublishedWithDefaultSettings(actual);
        }

        [TestMethod]
        public void
            GivenTaskWithAppId_Title_And_Folder__WhenRunningTask__ShouldSetAppIdBeforePublishing() {
            _fileSystem.AddDirectory(ExpectedDirectoryName);

            var workshopSpy = MakeSteamWorkshopSpy();
            var sut = new CreateSteamWorkshopItemTask(workshopSpy, _fileSystem) {
                AppId = AppId,
                ChangeSet = new WorkshopItemChangeSet(_fileSystem) {
                    Title = Title,
                    ItemFolderPath = ExpectedDirectoryName
                }
            };

            sut.Run();

            AssertAppIdSetBeforePublish(workshopSpy);
        }

        [TestMethod]
        [ExpectedException(typeof(ProcessFailedException))]
        public void GivenValidTask__WhenCallingRun_ButPublishFails__ShouldThrowProcessFailedException() {
            _fileSystem.AddDirectory(ExpectedDirectoryName);

            var workshopStub = new SteamWorkshopStub
                {WorkshopItemPublishResult = new WorkshopItemPublishResult(AppId, PublishResult.Failed)};
            var sut = new CreateSteamWorkshopItemTask(workshopStub, _fileSystem) {
                AppId = AppId,
                ChangeSet = new WorkshopItemChangeSet(_fileSystem) {
                    Title = Title,
                    ItemFolderPath = ExpectedDirectoryName
                }
            };

            sut.Run();
        }

        [TestMethod]
        public void GivenTaskWithoutAppId__WhenRunningTask__ShouldThrowException() {
            var workshop = new SteamWorkshopDummy();
            var sut = new CreateSteamWorkshopItemTask(workshop, _fileSystem);

            var actual = Assert.ThrowsException<InvalidOperationException>(() => sut.Run());

            Assert.AreEqual("No AppId set", actual.Message);
        }

        [TestMethod]
        public void GivenTaskWithoutChangeSet__WhenRunningTask__ShouldThrowException() {
            var workshop = new SteamWorkshopDummy();
            var sut = new CreateSteamWorkshopItemTask(workshop, _fileSystem) {AppId = AppId};

            var actual = Assert.ThrowsException<InvalidOperationException>(() => sut.Run());

            Assert.AreEqual("No change set given", actual.Message);
        }

        [TestMethod]
        public void GivenTaskWithPathToFileInsteadOfFolder__WhenRunningTask__ShouldThrowException() {
            _fileSystem.AddFile("path/to/file", new MockFileData(string.Empty));

            var workshop = new SteamWorkshopDummy();
            var sut = new CreateSteamWorkshopItemTask(workshop, _fileSystem) {
                AppId = AppId,
                ChangeSet = new WorkshopItemChangeSet(_fileSystem) {
                    Title = "Title",
                    ItemFolderPath = "path/to/file"
                }
            };

            var actual = Assert.ThrowsException<DirectoryNotFoundException>(() => sut.Run());

            Assert.AreEqual("Workshop item directory does not exist", actual.Message);
        }

        [TestMethod]
        [ExpectedException(typeof(NoRelativePathException))]
        public void GivenTaskWithNonRelativeItemPath__WhenRunningTask__ShouldThrowException() {
            var workshop = new SteamWorkshopDummy();

            var sut = new CreateSteamWorkshopItemTask(workshop, _fileSystem) {
                AppId = AppId,
                ChangeSet = new WorkshopItemChangeSet(_fileSystem) {
                    Title = "Title",
                    ItemFolderPath = "/absolute/path/to/file"
                }
            };

            sut.Run();
        }
        
        [TestMethod]
        [ExpectedException(typeof(NoRelativePathException))]
        public void GivenTaskWithNonRelativeDescriptionFilePath__WhenRunningTask__ShouldThrowException() {
            _fileSystem.AddDirectory("path/to/item");
            var workshop = new SteamWorkshopDummy();

            var sut = new CreateSteamWorkshopItemTask(workshop, _fileSystem) {
                AppId = AppId,
                ChangeSet = new WorkshopItemChangeSet(_fileSystem) {
                    Title = "Title",
                    ItemFolderPath = "path/to/item",
                    DescriptionFilePath = "/absolute/path/to/description"
                }
            };

            sut.Run();
        }
        
        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void GivenTaskWithNonExistingDescriptionFilePath__WhenRunningTask__ShouldThrowException() {
            _fileSystem.AddDirectory("path/to/item");
            var workshop = new SteamWorkshopDummy();

            var sut = new CreateSteamWorkshopItemTask(workshop, _fileSystem) {
                AppId = AppId,
                ChangeSet = new WorkshopItemChangeSet(_fileSystem) {
                    Title = "Title",
                    ItemFolderPath = "path/to/item",
                    DescriptionFilePath = "absolute/path/to/description"
                }
            };

            sut.Run();
        }

        private static void AssertAppIdSetBeforePublish(SteamWorkshopSpy workshopSpy) {
            Assert.AreEqual("ap", workshopSpy.CallOrder);
        }

        private static void AssertPublishedWithDefaultSettings(WorkshopItemChangeSet actual) {
            Assert.AreEqual("English", actual.Language);
            Assert.AreEqual(WorkshopItemVisibility.Private, actual.Visibility);
        }

        private static SteamWorkshopSpy MakeSteamWorkshopSpy() {
            return new SteamWorkshopSpy {
                WorkshopItemPublishResult = new WorkshopItemPublishResult(1, PublishResult.Ok)
            };
        }
    }
}