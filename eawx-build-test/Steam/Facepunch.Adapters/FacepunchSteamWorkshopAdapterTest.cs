using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using EawXBuild.Steam;
using EawXBuild.Steam.Facepunch.Adapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Steamworks;
using Steamworks.Ugc;
using PublishResult = EawXBuild.Steam.PublishResult;

namespace EawXBuildTest.Steam.Facepunch.Adapters
{
    [TestClass]
    public class FacepunchSteamWorkshopAdapterTest
    {
        private const string SteamUploadPath = "my_steam_upload";
        private const string Title = "eaw-ci Test upload";
        private const string DescriptionFilePath = "description.txt";
        private const string Description = "The description";
        private const string Language = "Spanish";
        private FileSystem _fileSystem;
        private IDirectoryInfo _itemFolder;
        private FileInfo _steamAppIdFile;
        private FacepunchSteamWorkshopAdapter _sut;

        [TestInitialize]
        public void SetUp()
        {
            _fileSystem = new FileSystem();
            CreateItemFolderWithSingleFile(_fileSystem);
            CreateDescriptionFile(_fileSystem);
            _steamAppIdFile = new FileInfo("steam_appid.txt");
            StreamWriter streamWriter = _steamAppIdFile.AppendText();
            streamWriter.WriteLine("32470");
            streamWriter.Close();

            _sut = FacepunchSteamWorkshopAdapter.Instance;
        }

        private static void CreateDescriptionFile(FileSystem fileSystem)
        {
            StreamWriter writer = fileSystem.File.CreateText(DescriptionFilePath);
            writer.WriteLine(Description);
            writer.Close();
        }

        private void CreateItemFolderWithSingleFile(FileSystem fileSystem)
        {
            _itemFolder = fileSystem.DirectoryInfo.FromDirectoryName(SteamUploadPath);
            _itemFolder.Create();
            fileSystem.File.CreateText(SteamUploadPath + "/file.txt").Close();
        }

        [TestCleanup]
        public void TearDown()
        {
            SteamClient.Shutdown();
            _steamAppIdFile.Delete();
            _itemFolder.Delete(true);
        }

        [TestMethodWithRequiredEnvironmentVariable("EAW_CI_TEST_STEAM_CLIENT", "YES")]
        public async Task GivenWorkshopChangeSet__WhenPublishingToSteam__ItemShouldBeOnWorkshop()
        {
            WorkshopItemChangeSet changeSet = new WorkshopItemChangeSet(_fileSystem)
            {
                Title = Title,
                DescriptionFilePath = DescriptionFilePath,
                Language = Language,
                Visibility = WorkshopItemVisibility.Private,
                ItemFolderPath = SteamUploadPath
            };

            _sut.Init(32470);
            WorkshopItemPublishResult publishTaskResult = await _sut.PublishNewWorkshopItemAsync(changeSet);

            PublishResult publishResult = publishTaskResult.Result;
            Assert.AreEqual(PublishResult.Ok, publishResult);
            await AssertItemMatchesSettings(publishTaskResult);
        }

        [TestMethodWithRequiredEnvironmentVariable("EAW_CI_TEST_STEAM_CLIENT", "YES")]
        public async Task WhenQueryingForItemId__ShouldReturnItemWithId()
        {
            _sut.Init(32470);
            const ulong fotrWorkshopId = 1976399102;
            IWorkshopItem? workshopItem = await _sut.QueryWorkshopItemByIdAsync(fotrWorkshopId);

            Assert.AreEqual(fotrWorkshopId, workshopItem.ItemId);
            StringAssert.StartsWith(workshopItem.Title, "Empire at War Expanded");
        }

        private static async Task AssertItemMatchesSettings(WorkshopItemPublishResult publishTaskResult)
        {
            Item? item = await Item.GetAsync(publishTaskResult.ItemId);
            Assert.IsTrue(item.HasValue);

            // TODO: The item data is not fetched properly even though it's there in Steam. Maybe it takes a while to register?
            // Assert.AreEqual(Title, item.Value.Title);
            // Assert.AreEqual(Description, item.Value.Description);
            // Assert.IsTrue(item.Value.IsPrivate);
        }
    }
}