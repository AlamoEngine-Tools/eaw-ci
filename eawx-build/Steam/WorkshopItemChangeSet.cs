using System;
using System.IO;
using System.IO.Abstractions;
using EawXBuild.Exceptions;

namespace EawXBuild.Steam {
    public class WorkshopItemChangeSet : IWorkshopItemChangeSet {
        private readonly IFileSystem _fileSystem;

        public WorkshopItemChangeSet(IFileSystem fileSystem) {
            _fileSystem = fileSystem;
        }

        public string Language { get; set; } = "English";
        public string Title { get; set; }
        public string ItemFolderPath { get; set; }
        public WorkshopItemVisibility Visibility { get; set; }
        public string DescriptionFilePath { get; set; }

        public string GetDescriptionTextFromFile() {
            var fileInfo = _fileSystem.FileInfo.FromFileName(DescriptionFilePath);
            using var reader = fileInfo.OpenText();

            return reader.ReadToEnd();
        }

        public (bool, Exception) IsValidNewChangeSet() {
            if (Title == null) return (false, new InvalidOperationException("No title set"));

            (bool isValid, Exception exception) result = ValidateItemFolderPath();
            if (!result.isValid) return result;

            return ValidateNullableDescriptionFilePath();
        }

        public (bool, Exception) IsValidUpdateChangeSet() {
            (bool isValid, Exception) result = ValidateNullableItemFolderPath();
            if (!result.isValid) return result;

            return ValidateNullableDescriptionFilePath();
        }

        private (bool, Exception) ValidateItemFolderPath() {
            return ItemFolderPath == null
                ? (false, new InvalidOperationException("No item folder set"))
                : ValidateNullableItemFolderPath();
        }

        private (bool, Exception) ValidateNullableItemFolderPath() {
            if (ItemFolderPath == null) return (true, null);

            if (!_fileSystem.Directory.Exists(ItemFolderPath))
                return (false, new DirectoryNotFoundException());

            (bool isRelativePath, Exception exception) result = ValidateRelativePath(ItemFolderPath);
            return !result.isRelativePath ? result : (true, null);
        }

        private (bool, Exception) ValidateRelativePath(string path) {
            return _fileSystem.Path.IsPathRooted(path)
                ? (false, new NoRelativePathException(path))
                : (true, null);
        }

        private (bool, Exception) ValidateNullableDescriptionFilePath() {
            if (DescriptionFilePath == null) return (true, null);
            if (!_fileSystem.File.Exists(DescriptionFilePath)) return (false, new FileNotFoundException());
            (bool isRelativePath, Exception exception) result = ValidateRelativePath(DescriptionFilePath);
            return !result.isRelativePath ? result : (true, null);
        }
    }
}