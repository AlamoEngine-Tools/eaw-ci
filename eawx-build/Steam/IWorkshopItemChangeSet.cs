using System;
using System.Collections.Generic;

namespace EawXBuild.Steam
{
    public interface IWorkshopItemChangeSet
    {
        public string Language { get; set; }

        public string Title { get; set; }

        public string ItemFolderPath { get; set; }

        public WorkshopItemVisibility Visibility { get; set; }

        public string DescriptionFilePath { get; set; }

        public HashSet<string> Tags { get; set; }

        (bool isValid, Exception? exception) IsValidChangeSet();

        public string GetDescriptionTextFromFile();
    }
}