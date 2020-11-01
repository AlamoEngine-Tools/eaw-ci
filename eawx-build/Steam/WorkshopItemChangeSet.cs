using System.IO.Abstractions;

namespace EawXBuild.Steam {
    public class WorkshopItemChangeSet {
        private readonly IFileSystem _fileSystem;

        public WorkshopItemChangeSet(IFileSystem fileSystem) {
            _fileSystem = fileSystem;
        }

        public string Language { get; set; } = "English";
        public string Title { get; set; }
        public string ItemFolderPath { get; set; }
        public WorkshopItemVisibility Visibility { get; set; }
        public string DescriptionFilePath { get; set; }

        public override string ToString() {
            return Title
                   + "\n" + DescriptionFilePath
                   + "\n" + Language
                   + "\n" + ItemFolderPath
                   + "\n" + Visibility;
        }

        public string GetDescriptionTextFromFile() {
            var fileInfo = _fileSystem.FileInfo.FromFileName(DescriptionFilePath);
            using var reader = fileInfo.OpenText();

            return reader.ReadToEnd();
        }
    }
}