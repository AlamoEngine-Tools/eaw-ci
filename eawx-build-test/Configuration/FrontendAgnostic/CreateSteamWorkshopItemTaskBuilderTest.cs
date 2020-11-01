using System;
using EawXBuild.Configuration.FrontendAgnostic;
using EawXBuild.Steam;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Configuration.FrontendAgnostic {
    [TestClass]
    public class CreateSteamWorkshopItemTaskBuilderTest {
        private const uint AppId = 32470;
        private const string Title = "My title";
        private const string DescriptionFilePath = "path/to/description";
        private const string ItemFolderPath = "path/to/item";
        private const string PublicVisibility = "Public";
        private const string PrivateVisibility = "Private";
        private const string Language = "Spanish";

        [TestMethod]
        public void GivenCreateSteamWorkshopItemTaskBuilder__WhenConfiguring__ShouldReturnConfiguredTask() {
            var sut = new CreateSteamWorkshopItemTaskBuilder();

            var actual = (CreateSteamWorkshopItemTask) sut
                .With("AppId", AppId)
                .With("Title", Title)
                .With("DescriptionFilePath", DescriptionFilePath)
                .With("ItemFolderPath", ItemFolderPath)
                .With("Visibility", PublicVisibility)
                .With("Language", Language)
                .Build();

            Assert.AreEqual(AppId, actual.AppId);
            Assert.AreEqual(Title, actual.ChangeSet.Title);
            Assert.AreEqual(DescriptionFilePath, actual.ChangeSet.DescriptionFilePath);
            Assert.AreEqual(ItemFolderPath, actual.ChangeSet.ItemFolderPath);
            Assert.AreEqual(WorkshopItemVisibility.Public, actual.ChangeSet.Visibility);
            Assert.AreEqual(Language, actual.ChangeSet.Language);
        }

        [TestMethod]
        public void
            GivenCreateSteamWorkshopItemTaskBuilder__WhenConfiguringWithPrivateVisibility__ShouldReturnConfiguredTaskWithPrivateVisibility() {
            var sut = new CreateSteamWorkshopItemTaskBuilder();

            var actual = (CreateSteamWorkshopItemTask) sut.With("Visibility", PrivateVisibility).Build();

            Assert.AreEqual(WorkshopItemVisibility.Private, actual.ChangeSet.Visibility);
        }
        
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WhenCallingWithInvalidConfigOption__ShouldThrowInvalidOperationException() {
            var sut = new CreateSteamWorkshopItemTaskBuilder();

            sut.With("InvalidOption", string.Empty);
        }
    }
}