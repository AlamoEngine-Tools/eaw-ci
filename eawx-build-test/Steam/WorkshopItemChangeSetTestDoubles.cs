using System;
using EawXBuild.Steam;

namespace EawXBuildTest.Steam {
    public class WorkshopItemChangeSetDummy : IWorkshopItemChangeSet {
        public string Language { get; set; }
        public string Title { get; set; }
        public string ItemFolderPath { get; set; }
        public WorkshopItemVisibility Visibility { get; set; }
        public string DescriptionFilePath { get; set; }
        
        public virtual (bool, Exception) IsValidNewChangeSet() {
            return (false, null);
        }

        public virtual (bool, Exception) IsValidUpdateChangeSet() {
            return (false, null);
        }

        public string GetDescriptionTextFromFile() {
            return string.Empty;
        }
    }

    public class WorkshopItemChangeSetStub : WorkshopItemChangeSetDummy {
        public (bool, Exception) ChangeSetValidationResult { get; set; }
        public (bool, Exception) ValidUpdateChangeSet { get; set; }
        
        public override (bool, Exception) IsValidNewChangeSet() {
            return ChangeSetValidationResult;
        }

        public override (bool, Exception) IsValidUpdateChangeSet() {
            return ValidUpdateChangeSet;
        }
    }
}