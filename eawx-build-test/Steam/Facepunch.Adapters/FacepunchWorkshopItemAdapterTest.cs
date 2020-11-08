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
    /// THIS TEST CLASS IS CURRENTLY BEING IGNORED!
    /// It seems to take a while before changes on Workshop items are available via the Steam API which makes it hard to test
    ///
    /// 
    /// These tests require an existing steam workshop item
    /// Set the environment variable EAW_CI_STEAM_WORKSHOP_ITEM_ID to the workshop item ID
    /// </summary>
    [TestClass]
    // [Ignore]
    public class FacepunchWorkshopItemAdapterTest {
        private const uint AppId = 32470;
        private const string SteamUploadPath = "my_steam_upload";
        private const string Title = "eaw-ci Test upload";
        private const string DescriptionFilePath = "description.txt";
        private const string Description = "The description";
        private FileInfo _steamAppIdFile;
        private IDirectoryInfo _itemFolder;

        private ulong _itemId;

        [TestInitialize]
        public void SetUp() {
            var itemIdString = Environment.GetEnvironmentVariable("EAW_CI_STEAM_WORKSHOP_ITEM_ID");
            if (itemIdString == null) return;

            var fileSystem = new FileSystem();
            CreateItemFolderWithSingleFile(fileSystem);
            CreateDescriptionFile(fileSystem);
            CreateSteamAppIdFile();

            SteamClient.Init(AppId);
            var item = GetItem(itemIdString);

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

        private Item GetItem(string itemIdString) {
            _itemId = ulong.Parse(itemIdString);
            var itemTask = Item.GetAsync(_itemId);
            Task.WaitAll(itemTask);
            var item = itemTask.Result;
            Assert.IsNotNull(item);
            return item.Value;
        }

        [TestCleanup]
        public void TearDown() {
            SteamClient.Shutdown();
            _steamAppIdFile.Delete();
            _itemFolder.Delete(true);
        }

        private void CreateSteamAppIdFile() {
            _steamAppIdFile = new FileInfo("steam_appid.txt");
            var streamWriter = _steamAppIdFile.AppendText();
            streamWriter.WriteLine("32470");
            streamWriter.Close();
        }

        private static void CreateDescriptionFile(IFileSystem fileSystem) {
            var writer = fileSystem.File.CreateText(DescriptionFilePath);
            writer.Write(Description);
            writer.Close();
        }

        private void CreateItemFolderWithSingleFile(IFileSystem fileSystem) {
            _itemFolder = fileSystem.DirectoryInfo.FromDirectoryName(SteamUploadPath);
            _itemFolder.Create();
            _itemFolder.CreateSubdirectory("sub_dir");
            Console.Out.WriteLine(_itemFolder.FullName);
            using var streamWriter = fileSystem.File.CreateText(SteamUploadPath + "/sub_dir/file.txt");
            streamWriter.Write("My awesome content!");
            streamWriter.Close();
        }

        [TestMethod]
        public async Task GivenWorkshopItem__WhenUpdatingSuccessfully__ShouldReturnOk() {
            if (Environment.GetEnvironmentVariable("EAW_CI_TEST_STEAM_CLIENT") != "YES") Assert.Inconclusive();

            var item = await Item.GetAsync(_itemId);
            Assert.IsNotNull(item);

            var sut = new FacepunchWorkshopItemAdapter(item.Value, AppId);

            var actual = await sut.UpdateItemAsync(new WorkshopItemChangeSetDummy());

            Assert.AreEqual(PublishResult.Ok, actual);
        }

        [TestMethod]
        public async Task GivenWorkshopItemWithChangedTitle__WhenUpdating__TitleShouldHaveChanged() {
            if (Environment.GetEnvironmentVariable("EAW_CI_TEST_STEAM_CLIENT") != "YES") Assert.Inconclusive();

            var item = await Item.GetAsync(_itemId);
            Assert.IsNotNull(item);

            var sut = new FacepunchWorkshopItemAdapter(item.Value, AppId);

            var actual = await sut.UpdateItemAsync(new WorkshopItemChangeSetDummy {Title = Title});

            item = await Item.GetAsync(_itemId);
            Assert.IsNotNull(item);
            Assert.AreEqual(Title, item.Value.Title);
        }

        [TestMethod]
        public async Task GivenWorkshopItemWithChangedDescription__WhenUpdating__DescriptionShouldHaveChanged() {
            if (Environment.GetEnvironmentVariable("EAW_CI_TEST_STEAM_CLIENT") != "YES") Assert.Inconclusive();

            var item = await Item.GetAsync(_itemId);
            Assert.IsNotNull(item);

            var sut = new FacepunchWorkshopItemAdapter(item.Value, AppId);

            var actual = await sut.UpdateItemAsync(new WorkshopItemChangeSetDummy {
                DescriptionFilePath = DescriptionFilePath
            });

            item = await Item.GetAsync(_itemId);
            Assert.IsNotNull(item);
            Assert.AreEqual(Description, item.Value.Description);
        }

        [TestMethod]
        public async Task GivenWorkshopItemWithChangedVisibility__WhenUpdating__VisibilityShouldHaveChanged() {
            if (Environment.GetEnvironmentVariable("EAW_CI_TEST_STEAM_CLIENT") != "YES") Assert.Inconclusive();

            var item = await Item.GetAsync(_itemId);
            Assert.IsNotNull(item);

            var sut = new FacepunchWorkshopItemAdapter(item.Value, AppId);

            var actual = await sut.UpdateItemAsync(new WorkshopItemChangeSetDummy {
                Visibility = WorkshopItemVisibility.Public
            });

            item = await Item.GetAsync(_itemId);
            Assert.IsNotNull(item);
            Assert.IsTrue(item.Value.IsPublic);
        }

        [TestMethod]
        public async Task GivenWorkshopItemWithChangedItemFolderPath__WhenUpdating__ShouldHaveChangedFiles() {
            if (Environment.GetEnvironmentVariable("EAW_CI_TEST_STEAM_CLIENT") != "YES") Assert.Inconclusive();
            var item = await Item.GetAsync(_itemId);
            Assert.IsNotNull(item);

            var sut = new FacepunchWorkshopItemAdapter(item.Value, AppId);

            var actual = await sut.UpdateItemAsync(new WorkshopItemChangeSetDummy {
                ItemFolderPath = _itemFolder.FullName
            });
            Assert.AreEqual(PublishResult.Ok, actual);

            item = await Item.GetAsync(_itemId);
            Assert.IsNotNull(item);
            var download = await item.Value.DownloadAsync();
            Assert.IsTrue(download);
        }
    }
}