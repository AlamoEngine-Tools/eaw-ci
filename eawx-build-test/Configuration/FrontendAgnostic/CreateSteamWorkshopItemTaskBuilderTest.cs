using System;
using System.Collections.Generic;
using EawXBuild.Configuration.FrontendAgnostic;
using EawXBuild.Steam;
using EawXBuild.Tasks;
using EawXBuild.Tasks.Steam;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Configuration.FrontendAgnostic {
    [TestClass]
    public class CreateSteamWorkshopItemTaskBuilderTest {
        private const uint AppId = 32470;
        private const string Title = "My title";
        private const string DescriptionFilePath = "path/to/description";
        private const string ItemFolderPath = "path/to/item";
        private const string Language = "Spanish";
        private readonly HashSet<string> Tags = new HashSet<string> {"EAW", "FOC"};

        [TestMethod]
        public void GivenCreateSteamWorkshopItemTaskBuilder__WhenConfiguring__ShouldReturnConfiguredTask() {
            var sut = new CreateSteamWorkshopItemTaskBuilder();

            var actual = (CreateSteamWorkshopItemTask) sut
                .With("AppId", AppId)
                .With("Title", Title)
                .With("DescriptionFilePath", DescriptionFilePath)
                .With("ItemFolderPath", ItemFolderPath)
                .With("Visibility", WorkshopItemVisibility.Public)
                .With("Language", Language)
                .With("Tags", Tags)
                .Build();

            Assert.AreEqual(AppId, actual.AppId);
            Assert.AreEqual(Title, actual.ChangeSet.Title);
            Assert.AreEqual(DescriptionFilePath, actual.ChangeSet.DescriptionFilePath);
            Assert.AreEqual(ItemFolderPath, actual.ChangeSet.ItemFolderPath);
            Assert.AreEqual(WorkshopItemVisibility.Public, actual.ChangeSet.Visibility);
            Assert.AreEqual(Language, actual.ChangeSet.Language);
            Assert.AreEqual(Tags, actual.ChangeSet.Tags);
        }

        [TestMethod]
        public void
            GivenCreateSteamWorkshopItemTaskBuilder__WhenConfiguringWithPrivateVisibility__ShouldReturnConfiguredTaskWithPrivateVisibility() {
            var sut = new CreateSteamWorkshopItemTaskBuilder();

            var actual = (CreateSteamWorkshopItemTask) sut.With("Visibility", WorkshopItemVisibility.Private).Build();

            Assert.AreEqual(WorkshopItemVisibility.Private, actual.ChangeSet.Visibility);
        }
        
        [TestMethod]
        public void GivenCreateSteamWorkshopItemTaskBuilder__WhenConfiguringWithNullVisibility__ShouldReturnTaskWithPrivateVisibility() {
            var sut = new CreateSteamWorkshopItemTaskBuilder();

            var actual = (CreateSteamWorkshopItemTask) sut.With("Visibility", null).Build();

            Assert.AreEqual(WorkshopItemVisibility.Private, actual.ChangeSet.Visibility);
        }
        
        [TestMethod]
        public void GivenCreateSteamWorkshopItemTaskBuilder_WhenConfiguringTaskWithAppIdOfTypeInt_ShouldReturnTaskWithAppIdOfUInt() {
            var sut = new CreateSteamWorkshopItemTaskBuilder();

            const int appId = 32470;
            var actual = (CreateSteamWorkshopItemTask) sut.With("AppId", appId).Build();

            const uint expected = appId;
            Assert.AreEqual(expected, actual.AppId);
        }
        
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WhenCallingWithInvalidConfigOption__ShouldThrowInvalidOperationException() {
            var sut = new CreateSteamWorkshopItemTaskBuilder();

            sut.With("InvalidOption", string.Empty);
        }
    }
}