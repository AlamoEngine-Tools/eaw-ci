using System;
using System.Collections.Generic;
using EawXBuild.Configuration.FrontendAgnostic;
using EawXBuild.Steam;
using EawXBuild.Tasks;
using EawXBuild.Tasks.Steam;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Configuration.FrontendAgnostic {
    [TestClass]
    public class UpdateSteamWorkshopItemTaskBuilderTest {
        private const uint AppId = 32470;
        private const ulong ItemId = 1234;
        private const string Title = "My title";
        private const string DescriptionFilePath = "path/to/description";
        private const string ItemFolderPath = "path/to/item";
        private const string Language = "Spanish";
        private readonly HashSet<string> Tags = new HashSet<string> {"EAW", "FOC"};

        [TestMethod]
        public void GivenTaskBuilder_WhenConfiguringTask_ShouldReturnConfiguredTask() {
            var sut = new UpdateSteamWorkshopItemTaskBuilder();

            var actual = (UpdateSteamWorkshopItemTask) sut.With("AppId", AppId)
                .With("ItemId", ItemId)
                .With("Title", Title)
                .With("DescriptionFilePath", DescriptionFilePath)
                .With("ItemFolderPath", ItemFolderPath)
                .With("Visibility", WorkshopItemVisibility.Public)
                .With("Language", Language)
                .With("Tags", Tags)
                .Build();

            Assert.AreEqual(AppId, actual.AppId);
            Assert.AreEqual(ItemId, actual.ItemId);
            Assert.AreEqual(Title, actual.ChangeSet.Title);
            Assert.AreEqual(DescriptionFilePath, actual.ChangeSet.DescriptionFilePath);
            Assert.AreEqual(ItemFolderPath, actual.ChangeSet.ItemFolderPath);
            Assert.AreEqual(WorkshopItemVisibility.Public, actual.ChangeSet.Visibility);
            Assert.AreEqual(Language, actual.ChangeSet.Language);
            Assert.AreEqual(Tags, actual.ChangeSet.Tags);
        }

        [TestMethod]
        public void
            GivenTaskBuilder__WhenConfiguringWithPrivateVisibility__ShouldReturnConfiguredTaskWithPrivateVisibility() {
            var sut = new UpdateSteamWorkshopItemTaskBuilder();

            var actual = (UpdateSteamWorkshopItemTask) sut.With("Visibility", WorkshopItemVisibility.Private).Build();

            Assert.AreEqual(WorkshopItemVisibility.Private, actual.ChangeSet.Visibility);
        }

        [TestMethod]
        public void GivenTaskBuilder__WhenConfiguringWithNullVisibility__ShouldReturnTaskWithPrivateVisibility() {
            var sut = new UpdateSteamWorkshopItemTaskBuilder();

            var actual = (UpdateSteamWorkshopItemTask) sut.With("Visibility", null).Build();

            Assert.AreEqual(WorkshopItemVisibility.Private, actual.ChangeSet.Visibility);
        }

        [TestMethod]
        public void GivenTaskBuilder_WhenConfiguringTaskWithAppIdOfTypeInt_ShouldReturnTaskWithAppIdOfUInt() {
            var sut = new UpdateSteamWorkshopItemTaskBuilder();

            const int appId = 32470;
            var actual = (UpdateSteamWorkshopItemTask) sut.With("AppId", appId).Build();

            const uint expected = appId;
            Assert.AreEqual(expected, actual.AppId);
        }

        [TestMethod]
        public void GivenTaskBuilder_WhenConfiguringTaskWithItemIdOfTypeInt_ShouldReturnTaskWithItemIdOfULong() {
            var sut = new UpdateSteamWorkshopItemTaskBuilder();

            const int itemId = 123456;
            var actual = (UpdateSteamWorkshopItemTask) sut.With("ItemId", itemId).Build();

            const ulong expected = itemId;
            Assert.AreEqual(expected, actual.ItemId);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WhenCallingWithInvalidConfigOption__ShouldThrowInvalidOperationException() {
            var sut = new UpdateSteamWorkshopItemTaskBuilder();

            sut.With("InvalidOption", string.Empty);
        }
    }
}