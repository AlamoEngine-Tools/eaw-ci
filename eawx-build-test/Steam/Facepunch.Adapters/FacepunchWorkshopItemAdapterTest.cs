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
    /// <summary>
    /// MOST TESTS IN THIS CLASS ARE CURRENTLY BEING IGNORED!
    /// It seems to take a while before changes on Workshop items are available via the Steam API which makes it hard to test
    /// The only test that can be run safely is the content changing test
    /// 
    /// These tests require an existing steam workshop item
    /// Set the environment variable EAW_CI_STEAM_WORKSHOP_ITEM_ID to the workshop item ID
    /// </summary>
    [TestClass]
    public class FacepunchWorkshopItemAdapterTest {
        private const uint AppId = 32470;
        private const string SteamUploadPath = "my_steam_upload";
        private const string Title = "EaW CI Test Upload New Title";
        private const string DescriptionFilePath = "description.txt";
        private const string Description = "The description";
        private IFileInfo _steamAppIdFile;
        private IDirectoryInfo _itemFolder;
        private IFileInfo _descriptionFile;

        private ulong _itemId;

        [TestInitialize]
        public void SetUp() {
            if (Environment.GetEnvironmentVariable("EAW_CI_TEST_STEAM_CLIENT") != "YES") return;

            var itemIdString = Environment.GetEnvironmentVariable("EAW_CI_STEAM_WORKSHOP_ITEM_ID");
            if (itemIdString == null) return;

            var fileSystem = new FileSystem();
            _steamAppIdFile = Utilities.CreateSteamAppIdFile(fileSystem);
            _itemFolder = Utilities.CreateItemFolderWithSingleFile(fileSystem, SteamUploadPath);
            _descriptionFile = Utilities.CreateDescriptionFile(fileSystem, DescriptionFilePath, Description);

            SteamClient.Init(AppId);
            _itemId = ulong.Parse(itemIdString);
            var item = GetItem(_itemId);

            var restoreSettingsTask = item.Edit()
                .ForAppId(AppId)
                .InLanguage("English")
                .WithPrivateVisibility()
                .WithTitle("EAW_CI_TEST_UPLOAD")
                .WithDescription("")
                .SubmitAsync();

            Task.WaitAll(restoreSettingsTask);
            Assert.AreEqual(Result.OK, restoreSettingsTask.Result.Result);
        }

        private static Item GetItem(ulong itemId) {
            var itemTask = Item.GetAsync(itemId);
            Task.WaitAll(itemTask);
            var item = itemTask.Result;
            Assert.IsNotNull(item);
            return item.Value;
        }

        [TestCleanup]
        public void TearDown() {
            if (Environment.GetEnvironmentVariable("EAW_CI_TEST_STEAM_CLIENT") != "YES") return;

            var itemIdString = Environment.GetEnvironmentVariable("EAW_CI_STEAM_WORKSHOP_ITEM_ID");
            if (itemIdString == null) return;

            _steamAppIdFile.Delete();
            _itemFolder.Delete(true);
            _descriptionFile.Delete();

            var item = GetItem(_itemId);
            var directory = new DirectoryInfo(item.Directory);
            if (directory.Exists) directory.Delete(true);

            SteamClient.Shutdown();
        }


        [TestMethodWithRequiredEnvironmentVariable("EAW_CI_TEST_STEAM_CLIENT", "YES")]
        public async Task GivenWorkshopItem__WhenUpdatingSuccessfully__ShouldReturnOk() {
            var item = await Item.GetAsync(_itemId);
            Assert.IsNotNull(item);

            var sut = new FacepunchWorkshopItemAdapter(item.Value);

            var actual = await sut.UpdateItemAsync(new WorkshopItemChangeSetDummy());

            Assert.AreEqual(PublishResult.Ok, actual);
        }

        [TestMethodWithRequiredEnvironmentVariable("EAW_CI_TEST_STEAM_CLIENT", "YES")]
        [Ignore]
        public async Task GivenWorkshopItemWithChangedTitle__WhenUpdating__TitleShouldHaveChanged() {
            var item = await Item.GetAsync(_itemId);
            Assert.IsNotNull(item);

            var sut = new FacepunchWorkshopItemAdapter(item.Value);

            var actual = await sut.UpdateItemAsync(new WorkshopItemChangeSetDummy {Title = Title});

            item = GetItem(_itemId);
            Assert.AreEqual(Title, item.Value.Title);
        }

        [TestMethodWithRequiredEnvironmentVariable("EAW_CI_TEST_STEAM_CLIENT", "YES")]
        [Ignore]
        public async Task GivenWorkshopItemWithChangedDescription__WhenUpdating__DescriptionShouldHaveChanged() {
            var item = await Item.GetAsync(_itemId);
            Assert.IsNotNull(item);

            var sut = new FacepunchWorkshopItemAdapter(item.Value);

            var actual = await sut.UpdateItemAsync(new WorkshopItemChangeSetDummy {
                DescriptionFilePath = DescriptionFilePath
            });

            item = GetItem(_itemId);
            Assert.AreEqual(Description, item.Value.Description);
        }

        [TestMethodWithRequiredEnvironmentVariable("EAW_CI_TEST_STEAM_CLIENT", "YES")]
        [Ignore]
        public async Task GivenWorkshopItemWithChangedVisibility__WhenUpdating__VisibilityShouldHaveChanged() {
            var item = await Item.GetAsync(_itemId);
            Assert.IsNotNull(item);

            var sut = new FacepunchWorkshopItemAdapter(item.Value);

            var actual = await sut.UpdateItemAsync(new WorkshopItemChangeSetDummy {
                Visibility = WorkshopItemVisibility.Public
            });

            item = GetItem(_itemId);
            Assert.IsTrue(item.Value.IsPublic);
        }

        [TestMethodWithRequiredEnvironmentVariable("EAW_CI_TEST_STEAM_CLIENT", "YES")]
        public async Task GivenWorkshopItemWithChangedItemFolderPath__WhenUpdating__ShouldHaveChangedFiles() {
            var item = GetItem(_itemId);

            var sut = new FacepunchWorkshopItemAdapter(item);

            var actual = await sut.UpdateItemAsync(new WorkshopItemChangeSetDummy {
                ItemFolderPath = _itemFolder.FullName
            });
            Assert.AreEqual(PublishResult.Ok, actual);

            item = GetItem(_itemId);
            await item.DownloadAsync();
            var itemDirectory = new DirectoryInfo(item.Directory);
            var subDirectory = itemDirectory.GetDirectories()[0];

            Assert.AreEqual("sub_dir", subDirectory.Name);
            Assert.AreEqual("file.txt", subDirectory.GetFiles()[0].Name);
        }
    }
}