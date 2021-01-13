using System;
using System.Collections.Generic;
using EawXBuild.Steam;

namespace EawXBuildTest.Steam {
    public class WorkshopItemChangeSetDummy : IWorkshopItemChangeSet {
        public string Language { get; set; }
        public string Title { get; set; }
        public string ItemFolderPath { get; set; }
        public WorkshopItemVisibility Visibility { get; set; }
        public string DescriptionFilePath { get; set; }

        public HashSet<string> Tags { get; set; }

        public virtual (bool, Exception) IsValidChangeSet() {
            return (false, null);
        }

        public string GetDescriptionTextFromFile() {
            return string.Empty;
        }
    }

    public class WorkshopItemChangeSetStub : WorkshopItemChangeSetDummy {
        public (bool, Exception) ChangeSetValidationResult { get; set; }

        public override (bool, Exception) IsValidChangeSet() {
            return ChangeSetValidationResult;
        }
    }
}