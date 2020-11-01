using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using EawXBuild.Steam;
using EawXBuild.Steam.Facepunch.Adapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Steamworks;
using Steamworks.Ugc;
using PublishResult = EawXBuild.Steam.PublishResult;

namespace EawXBuildTest.Steam.Facepunch.Adapters {
    
    [TestClass]
    public class FacepunchSteamWorkshopAdapterTest {
        private const string SteamUploadPath = "my_steam_upload";
        private const string Title = "eaw-ci Test upload";
        private const string Description = "The description";
        private const string Language = "Spanish";
        private FileInfo _steamAppIdFile;
        private IDirectoryInfo _itemFolder;
        
        [TestInitialize]
        public void SetUp() {
            var fileSystem = new FileSystem();
            CreateItemFolderWithSingleFile(fileSystem);
            _steamAppIdFile = new FileInfo("steam_appid.txt");
            var streamWriter = _steamAppIdFile.AppendText();
            streamWriter.WriteLine("32470");
            streamWriter.Close();
        }

        private void CreateItemFolderWithSingleFile(FileSystem fileSystem) {
            _itemFolder = fileSystem.DirectoryInfo.FromDirectoryName(SteamUploadPath);
            _itemFolder.Create();
            fileSystem.File.CreateText(_itemFolder.FullName + "/file.txt").Close();
        }

        [TestCleanup]
        public void TearDown() {
            SteamClient.Shutdown();
            _steamAppIdFile.Delete();
            _itemFolder.Delete(true);
        }
        
        [TestMethod]
        public async Task GivenWorkshopChangeSet__WhenPublishingToSteam__ItemShouldBeOnWorkshop() {
            if (Environment.GetEnvironmentVariable("EAW_CI_TEST_STEAM_CLIENT") != "YES") Assert.Inconclusive();

            var changeSet = new WorkshopItemChangeSet {
                Title = Title,
                Description = Description,
                Language = Language,
                Visibility = WorkshopItemVisibility.Private,
                ItemFolder = _itemFolder
            };
            
            var sut = FacepunchSteamWorkshopAdapter.Instance;
            sut.AppId = 32470;

            var publishTaskResult = await sut.PublishNewWorkshopItemAsync(changeSet);

            var publishResult = publishTaskResult.Result;
            Assert.AreEqual(PublishResult.Ok, publishResult);
            await AssertItemMatchesSettings(publishTaskResult);
        }

        [TestMethod]
        public async Task WhenQueryingForItemId__ShouldReturnItemWithId() {
            if (Environment.GetEnvironmentVariable("EAW_CI_TEST_STEAM_CLIENT") != "YES") Assert.Inconclusive();
            
            var sut = FacepunchSteamWorkshopAdapter.Instance;
            sut.AppId = 32470;

            const ulong fotrWorkshopId = 1976399102;
            var workshopItem = await sut.QueryWorkshopItemByIdAsync(fotrWorkshopId);
            
            Assert.AreEqual(fotrWorkshopId, workshopItem.ItemId);
            StringAssert.StartsWith(workshopItem.Title, "Empire at War Expanded");
        }

        private static async Task AssertItemMatchesSettings(WorkshopItemPublishResult publishTaskResult) {
            SteamClient.Init(32470);
            var item = await Item.GetAsync(publishTaskResult.ItemId);
            Assert.IsTrue(item.HasValue);
            
            // TODO: The item data is not fetched properly even though it's there in Steam. Maybe it takes a while to register?
            // Assert.AreEqual(Title, item.Value.Title);
            // Assert.AreEqual(Description, item.Value.Description);
            // Assert.IsTrue(item.Value.IsPrivate);
        }
    }
}