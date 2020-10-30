using System;
using EawXBuild.Configuration.FrontendAgnostic;
using EawXBuild.Steam;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Configuration.FrontendAgnostic {
    [TestClass]
    public class CreateSteamWorkshopItemTaskBuilderTest {
        private const uint AppId = 32470;
        private const string Title = "My title";
        private const string Description = "The description";
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
                .With("Description", Description)
                .With("ItemFolderPath", ItemFolderPath)
                .With("Visibility", PublicVisibility)
                .With("Language", Language)
                .Build();

            Assert.AreEqual(AppId, actual.AppId);
            Assert.AreEqual(Title, actual.Title);
            Assert.AreEqual(Description, actual.Description);
            Assert.AreEqual(ItemFolderPath, actual.ItemFolderPath);
            Assert.AreEqual(WorkshopItemVisibility.Public, actual.Visibility);
            Assert.AreEqual(Language, actual.Language);
        }

        [TestMethod]
        public void
            GivenCreateSteamWorkshopItemTaskBuilder__WhenConfiguringWithPrivateVisibility__ShouldReturnConfiguredTaskWithPrivateVisibility() {
            var sut = new CreateSteamWorkshopItemTaskBuilder();

            var actual = (CreateSteamWorkshopItemTask) sut.With("Visibility", PrivateVisibility).Build();

            Assert.AreEqual(WorkshopItemVisibility.Private, actual.Visibility);
        }
        
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WhenCallingWithInvalidConfigOption__ShouldThrowInvalidOperationException() {
            var sut = new CreateSteamWorkshopItemTaskBuilder();

            sut.With("InvalidOption", string.Empty);
        }
    }
}