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
        public void GivenWorkshopItemChangeSetWithValidMinimalSettings__WhenCallingIsValid__ShouldReturnTrueAndNoException() {
            var fileSystem = new MockFileSystem();
            const string itemFolderPath = "path/to/item/folder";
            fileSystem.AddDirectory(itemFolderPath);

            var sut = new WorkshopItemChangeSet(fileSystem) {
                Title = "Title",
                ItemFolderPath = itemFolderPath
            };

            var (isValid, exception) = sut.IsValidNewChangeSet();

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

            var (isValid, exception) = sut.IsValidNewChangeSet();

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

            var (isValid, exception) = sut.IsValidNewChangeSet();

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

            var (isValid, exception) = sut.IsValidNewChangeSet();

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

            var (isValid, exception) = sut.IsValidNewChangeSet();

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

            var (isValid, exception) = sut.IsValidNewChangeSet();

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

            var (isValid, exception) = sut.IsValidNewChangeSet();

            Assert.IsFalse(isValid);
            Assert.IsInstanceOfType(exception, typeof(NoRelativePathException));
        }
        
        [TestMethod]
        public void
            GivenWorkshopItemChangeSetWithValidItemFolder__WhenCallingIsValidUpdate__ShouldReturnTrueAndNoException() {
            const string itemFolderPath = "path/to/item/folder";
            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(itemFolderPath);

            var sut = new WorkshopItemChangeSet(fileSystem) {
                ItemFolderPath = itemFolderPath
            };

            var (isValid, exception) = sut.IsValidUpdateChangeSet();

            Assert.IsTrue(isValid);
            Assert.IsNull(exception);
        }
        
        [TestMethod]
        public void
            GivenWorkshopItemChangeSetWithNonExistingItemFolder__WhenCallingIsValidUpdate__ShouldReturnFalseAndDirectoryNotFoundException() {
            const string nonExistingFolder = "non/existing/folder";
            var fileSystem = new MockFileSystem();

            var sut = new WorkshopItemChangeSet(fileSystem) {
                ItemFolderPath = nonExistingFolder
            };

            var (isValid, exception) = sut.IsValidUpdateChangeSet();

            Assert.IsFalse(isValid);
            Assert.IsInstanceOfType(exception, typeof(DirectoryNotFoundException));
        }
        
        [TestMethod]
        public void
            GivenWorkshopItemChangeSetWithAbsoluteItemFolderPath__WhenCallingIsValidUpdate__ShouldReturnFalseAndNoRelativePathException() {
            const string absolutePath = "/absolute/path";
            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(absolutePath);
            
            var sut = new WorkshopItemChangeSet(fileSystem) {
                ItemFolderPath = absolutePath
            };
        
            var (isValid, exception) = sut.IsValidUpdateChangeSet();
        
            Assert.IsFalse(isValid);
            Assert.IsInstanceOfType(exception, typeof(NoRelativePathException));
        }
        
        [TestMethod]
        public void
            GivenWorkshopItemChangeSetWithValidDescriptionFile__WhenCallingIsValidUpdate__ShouldReturnTrueAndNoException() {
            const string descriptionFilePath = "path/to/description";
            var fileSystem = new MockFileSystem();
            fileSystem.AddFile(descriptionFilePath, MockFileData.NullObject);

            var sut = new WorkshopItemChangeSet(fileSystem) {
                DescriptionFilePath = descriptionFilePath
            };

            var (isValid, exception) = sut.IsValidUpdateChangeSet();

            Assert.IsTrue(isValid);
            Assert.IsNull(exception);
        }
        
        [TestMethod]
        public void
            GivenWorkshopItemChangeSetWithNonExistingDescriptionFile__WhenCallingIsValidUpdate__ShouldReturnFalseAndFileNotFoundException() {
            const string nonExistingFile = "non/existing/file";
            var fileSystem = new MockFileSystem();

            var sut = new WorkshopItemChangeSet(fileSystem) {
                DescriptionFilePath = nonExistingFile
            };

            var (isValid, exception) = sut.IsValidUpdateChangeSet();

            Assert.IsFalse(isValid);
            Assert.IsInstanceOfType(exception, typeof(FileNotFoundException));
        }
        
        [TestMethod]
        public void
            GivenWorkshopItemChangeSetWithAbsoluteDescriptionFilePath__WhenCallingIsValidUpdate__ShouldReturnFalseAndNoRelativePathException() {
            const string absolutePath = "/absolute/path";
            var fileSystem = new MockFileSystem();
            fileSystem.AddFile(absolutePath, absolutePath);
            
            var sut = new WorkshopItemChangeSet(fileSystem) {
                DescriptionFilePath = absolutePath
            };
        
            var (isValid, exception) = sut.IsValidUpdateChangeSet();
        
            Assert.IsFalse(isValid);
            Assert.IsInstanceOfType(exception, typeof(NoRelativePathException));
        }
        
        [TestMethod]
        public void
            GivenWorkshopItemChangeSetWithValidItemFolderAndAbsoluteDescriptionFilePath__WhenCallingIsValidUpdate__ShouldReturnFalseAndNoRelativePathException() {
            const string itemFolder = "path/to/folder";
            const string absolutePath = "/absolute/path";
            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(itemFolder);
            fileSystem.AddFile(absolutePath, MockFileData.NullObject);
            
            var sut = new WorkshopItemChangeSet(fileSystem) {
                ItemFolderPath = itemFolder,
                DescriptionFilePath = absolutePath
            };
        
            var (isValid, exception) = sut.IsValidUpdateChangeSet();
        
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
    }
}