using System;

namespace EawXBuild.Steam {
    public interface IWorkshopItemChangeSet {
        public string Language { get; set; }

        public string Title { get; set; }

        public string ItemFolderPath { get; set; }

        public WorkshopItemVisibility Visibility { get; set; }

        public string DescriptionFilePath { get; set; }

        (bool, Exception) IsValidNewChangeSet();

        public string GetDescriptionTextFromFile();
    }
}