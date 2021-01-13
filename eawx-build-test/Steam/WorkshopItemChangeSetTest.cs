using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using EawXBuild.Exceptions;
using EawXBuild.Steam;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Steam {
    [TestClass]
    public class WorkshopItemChangeSetTest {
        [TestMethod]
        public void
            GivenWorkshopItemChangeSetWithValidMinimalSettings__WhenCallingIsValid__ShouldReturnTrueAndNoException() {
            var fileSystem = new MockFileSystem();
            const string itemFolderPath = "path/to/item/folder";
            fileSystem.AddDirectory(itemFolderPath);

            var sut = new WorkshopItemChangeSet(fileSystem) {
                Title = "Title",
                ItemFolderPath = itemFolderPath
            };

            var (isValid, exception) = sut.IsValidChangeSet();

            Assert.IsTrue(isValid);
            Assert.IsNull(exception);
        }

        [TestMethod]
        public void
            GivenWorkshopItemChangeSetWithoutTitle__WhenCallingIsValid__ShouldReturnFalseAndInvalidOperationException() {
            var fileSystem = new MockFileSystem();
            const string itemFolderPath = "path/to/item/folder";
            fileSystem.AddDirectory(itemFolderPath);
            var sut = new WorkshopItemChangeSet(fileSystem) {
                ItemFolderPath = itemFolderPath
            };

            var (isValid, exception) = sut.IsValidChangeSet();

            Assert.IsFalse(isValid);
            Assert.IsInstanceOfType(exception, typeof(InvalidOperationException));
            Assert.AreEqual("No title set", exception.Message);
        }

        [TestMethod]
        public void
            GivenWorkshopItemWithoutItemFolderPath__WhenCallingIsValid__ShouldReturnFalseAndInvalidOperationException() {
            var fileSystem = new MockFileSystem();

            var sut = new WorkshopItemChangeSet(fileSystem) {
                Title = "Title",
            };

            var (isValid, exception) = sut.IsValidChangeSet();

            Assert.IsFalse(isValid);
            Assert.IsInstanceOfType(exception, typeof(InvalidOperationException));
            Assert.AreEqual("No item folder set", exception.Message);
        }

        [TestMethod]
        public void
            GivenWorkshopItemChangeSetWithNonExistingItemFolder__WhenCallingIsValid__ShouldReturnFalseAndDirectoryNotFoundException() {
            var fileSystem = new MockFileSystem();

            const string nonExistingFolder = "non/existing/folder";
            var sut = new WorkshopItemChangeSet(fileSystem) {
                Title = "Title",
                ItemFolderPath = nonExistingFolder
            };

            var (isValid, exception) = sut.IsValidChangeSet();

            Assert.IsFalse(isValid);
            Assert.IsInstanceOfType(exception, typeof(DirectoryNotFoundException));
        }

        [TestMethod]
        public void
            GivenWorkshopItemChangeSetWithAbsoluteItemFolderPath__WhenCallingIsValid__ShouldReturnFalseAndNoRelativePathException() {
            const string absolutePath = "/absolute/path";
            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(absolutePath);

            var sut = new WorkshopItemChangeSet(fileSystem) {
                Title = "Title",
                ItemFolderPath = absolutePath
            };

            var (isValid, exception) = sut.IsValidChangeSet();

            Assert.IsFalse(isValid);
            Assert.IsInstanceOfType(exception, typeof(NoRelativePathException));
        }

        [TestMethod]
        public void
            GivenWorkshopItemChangeSetWithNonExistingDescriptionFile__WhenCallingIsValid__ShouldReturnFalseAndFileNotFoundException() {
            const string itemFolderPath = "path/to/item/folder";
            const string descriptionFilePath = "non/existing/file";

            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(itemFolderPath);
            var sut = new WorkshopItemChangeSet(fileSystem) {
                Title = "Title",
                ItemFolderPath = itemFolderPath,
                DescriptionFilePath = descriptionFilePath
            };

            var (isValid, exception) = sut.IsValidChangeSet();

            Assert.IsFalse(isValid);
            Assert.IsInstanceOfType(exception, typeof(FileNotFoundException));
        }

        [TestMethod]
        public void
            GivenWorkshopItemChangeSetWithAbsoluteDescriptionFilePath__WhenCallingIsValid__ShouldReturnFalseAndNoRelativePathException() {
            const string itemFolderPath = "path/to/item/folder";
            const string descriptionFilePath = "/absolute/path/to/file";

            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(itemFolderPath);
            fileSystem.AddFile(descriptionFilePath, MockFileData.NullObject);

            var sut = new WorkshopItemChangeSet(fileSystem) {
                Title = "Title",
                ItemFolderPath = itemFolderPath,
                DescriptionFilePath = descriptionFilePath
            };

            var (isValid, exception) = sut.IsValidChangeSet();

            Assert.IsFalse(isValid);
            Assert.IsInstanceOfType(exception, typeof(NoRelativePathException));
        }

        [TestMethod]
        public void
            GivenWorkshopItemChangeSetWithDescriptionFilePath__WhenCallingGetDescriptionTextFromFile__ShouldReturnDescriptionText() {
            const string expectedDescriptionText = "The description";
            var fileSystem = new MockFileSystem();
            fileSystem.AddFile("path/to/description", new MockFileData(expectedDescriptionText));
            var sut = new WorkshopItemChangeSet(fileSystem) {
                DescriptionFilePath = "path/to/description"
            };

            var actual = sut.GetDescriptionTextFromFile();

            Assert.AreEqual(expectedDescriptionText, actual);
        }

        [TestMethod]
        public void
            GivenWorkshopItemChangeSetWithoutDescriptionFilePath__WhenCallingGetDescriptionTextFromFile__ShouldReturnEmptyString() {
            var expectedDescriptionText = string.Empty;
            var fileSystem = new MockFileSystem();
            var sut = new WorkshopItemChangeSet(fileSystem);

            var actual = sut.GetDescriptionTextFromFile();

            Assert.AreEqual(expectedDescriptionText, actual);
        }
    }
}