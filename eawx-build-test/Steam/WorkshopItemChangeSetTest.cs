using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using EawXBuild.Exceptions;
using EawXBuild.Steam;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Steam
{
    [TestClass]
    public class WorkshopItemChangeSetTest
    {
        [TestMethod]
        public void
            GivenWorkshopItemChangeSetWithValidMinimalSettings__WhenCallingIsValid__ShouldReturnTrueAndNoException()
        {
            MockFileSystem fileSystem = new MockFileSystem();
            const string itemFolderPath = "path/to/item/folder";
            fileSystem.AddDirectory(itemFolderPath);

            WorkshopItemChangeSet sut = new WorkshopItemChangeSet(fileSystem)
            {
                Title = "Title",
                ItemFolderPath = itemFolderPath
            };

            (bool isValid, Exception exception) = sut.IsValidChangeSet();

            Assert.IsTrue(isValid);
            Assert.IsNull(exception);
        }

        [TestMethod]
        public void
            GivenWorkshopItemChangeSetWithoutTitle__WhenCallingIsValid__ShouldReturnFalseAndInvalidOperationException()
        {
            MockFileSystem fileSystem = new MockFileSystem();
            const string itemFolderPath = "path/to/item/folder";
            fileSystem.AddDirectory(itemFolderPath);
            WorkshopItemChangeSet sut = new WorkshopItemChangeSet(fileSystem)
            {
                ItemFolderPath = itemFolderPath
            };

            (bool isValid, Exception exception) = sut.IsValidChangeSet();

            Assert.IsFalse(isValid);
            Assert.IsInstanceOfType(exception, typeof(InvalidOperationException));
            Assert.AreEqual("No title set", exception.Message);
        }

        [TestMethod]
        public void
            GivenWorkshopItemWithoutItemFolderPath__WhenCallingIsValid__ShouldReturnFalseAndInvalidOperationException()
        {
            MockFileSystem fileSystem = new MockFileSystem();

            WorkshopItemChangeSet sut = new WorkshopItemChangeSet(fileSystem)
            {
                Title = "Title"
            };

            (bool isValid, Exception exception) = sut.IsValidChangeSet();

            Assert.IsFalse(isValid);
            Assert.IsInstanceOfType(exception, typeof(InvalidOperationException));
            Assert.AreEqual("No item folder set", exception.Message);
        }

        [TestMethod]
        public void
            GivenWorkshopItemChangeSetWithNonExistingItemFolder__WhenCallingIsValid__ShouldReturnFalseAndDirectoryNotFoundException()
        {
            MockFileSystem fileSystem = new MockFileSystem();

            const string nonExistingFolder = "non/existing/folder";
            WorkshopItemChangeSet sut = new WorkshopItemChangeSet(fileSystem)
            {
                Title = "Title",
                ItemFolderPath = nonExistingFolder
            };

            (bool isValid, Exception exception) = sut.IsValidChangeSet();

            Assert.IsFalse(isValid);
            Assert.IsInstanceOfType(exception, typeof(DirectoryNotFoundException));
        }

        [TestMethod]
        public void
            GivenWorkshopItemChangeSetWithAbsoluteItemFolderPath__WhenCallingIsValid__ShouldReturnFalseAndNoRelativePathException()
        {
            const string absolutePath = "/absolute/path";
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(absolutePath);

            WorkshopItemChangeSet sut = new WorkshopItemChangeSet(fileSystem)
            {
                Title = "Title",
                ItemFolderPath = absolutePath
            };

            (bool isValid, Exception exception) = sut.IsValidChangeSet();

            Assert.IsFalse(isValid);
            Assert.IsInstanceOfType(exception, typeof(NoRelativePathException));
        }

        [TestMethod]
        public void
            GivenWorkshopItemChangeSetWithNonExistingDescriptionFile__WhenCallingIsValid__ShouldReturnFalseAndFileNotFoundException()
        {
            const string itemFolderPath = "path/to/item/folder";
            const string descriptionFilePath = "non/existing/file";

            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(itemFolderPath);
            WorkshopItemChangeSet sut = new WorkshopItemChangeSet(fileSystem)
            {
                Title = "Title",
                ItemFolderPath = itemFolderPath,
                DescriptionFilePath = descriptionFilePath
            };

            (bool isValid, Exception exception) = sut.IsValidChangeSet();

            Assert.IsFalse(isValid);
            Assert.IsInstanceOfType(exception, typeof(FileNotFoundException));
        }

        [TestMethod]
        public void
            GivenWorkshopItemChangeSetWithAbsoluteDescriptionFilePath__WhenCallingIsValid__ShouldReturnFalseAndNoRelativePathException()
        {
            const string itemFolderPath = "path/to/item/folder";
            const string descriptionFilePath = "/absolute/path/to/file";

            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(itemFolderPath);
            fileSystem.AddFile(descriptionFilePath, MockFileData.NullObject);

            WorkshopItemChangeSet sut = new WorkshopItemChangeSet(fileSystem)
            {
                Title = "Title",
                ItemFolderPath = itemFolderPath,
                DescriptionFilePath = descriptionFilePath
            };

            (bool isValid, Exception exception) = sut.IsValidChangeSet();

            Assert.IsFalse(isValid);
            Assert.IsInstanceOfType(exception, typeof(NoRelativePathException));
        }

        [TestMethod]
        public void
            GivenWorkshopItemChangeSetWithDescriptionFilePath__WhenCallingGetDescriptionTextFromFile__ShouldReturnDescriptionText()
        {
            const string expectedDescriptionText = "The description";
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddFile("path/to/description", new MockFileData(expectedDescriptionText));
            WorkshopItemChangeSet sut = new WorkshopItemChangeSet(fileSystem)
            {
                DescriptionFilePath = "path/to/description"
            };

            string actual = sut.GetDescriptionTextFromFile();

            Assert.AreEqual(expectedDescriptionText, actual);
        }

        [TestMethod]
        public void
            GivenWorkshopItemChangeSetWithoutDescriptionFilePath__WhenCallingGetDescriptionTextFromFile__ShouldReturnEmptyString()
        {
            string expectedDescriptionText = string.Empty;
            MockFileSystem fileSystem = new MockFileSystem();
            WorkshopItemChangeSet sut = new WorkshopItemChangeSet(fileSystem);

            string actual = sut.GetDescriptionTextFromFile();

            Assert.AreEqual(expectedDescriptionText, actual);
        }
    }
}