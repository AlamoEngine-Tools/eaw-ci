using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using EawXBuild.Exceptions;

namespace EawXBuild.Steam
{
    public class WorkshopItemChangeSet : IWorkshopItemChangeSet
    {
        private readonly IFileSystem _fileSystem;

        public WorkshopItemChangeSet(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public string Language { get; set; } = "English";
        public string Title { get; set; }
        public string ItemFolderPath { get; set; }
        public WorkshopItemVisibility Visibility { get; set; }
        public string DescriptionFilePath { get; set; }

        public HashSet<string> Tags { get; set; }

        public string GetDescriptionTextFromFile()
        {
            if (DescriptionFilePath == null) return string.Empty;
            IFileInfo fileInfo = _fileSystem.FileInfo.FromFileName(DescriptionFilePath);
            using StreamReader reader = fileInfo.OpenText();

            return reader.ReadToEnd();
        }

        public (bool isValid, Exception exception) IsValidChangeSet()
        {
            if (Title == null) return (false, new InvalidOperationException("No title set"));

            (bool isValid, Exception exception) result = ValidateItemFolderPath();
            return !result.isValid ? result : ValidateDescriptionFilePath();
        }

        private (bool isValid, Exception exception) ValidateItemFolderPath()
        {
            if (ItemFolderPath == null)
                return (false, new InvalidOperationException("No item folder set"));

            if (!_fileSystem.Directory.Exists(ItemFolderPath))
                return (false, new DirectoryNotFoundException());

            (bool isValid, Exception exception) result = ValidateRelativePath(ItemFolderPath);
            return !result.isValid ? result : (true, null);
        }

        private (bool isValid, Exception exception) ValidateRelativePath(string path)
        {
            return _fileSystem.Path.IsPathRooted(path)
                ? (false, new NoRelativePathException(path))
                : (true, null);
        }

        private (bool isValid, Exception exception) ValidateDescriptionFilePath()
        {
            if (DescriptionFilePath == null) return (true, null);
            if (!_fileSystem.File.Exists(DescriptionFilePath)) return (false, new FileNotFoundException());
            (bool isRelativePath, Exception exception) result = ValidateRelativePath(DescriptionFilePath);
            return !result.isRelativePath ? result : (true, null);
        }
    }
}