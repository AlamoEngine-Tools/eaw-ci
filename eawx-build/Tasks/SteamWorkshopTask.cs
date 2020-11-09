using System;
using EawXBuild.Core;
using EawXBuild.Steam;

namespace EawXBuild.Tasks {
    public abstract class SteamWorkshopTask : ITask {
        
        public IWorkshopItemChangeSet ChangeSet { get; set; }
        
        public void Run() {
            ValidateChangeSet();
            PublishToWorkshop();
        }
        
        private void ValidateChangeSet() {
            if (ChangeSet == null) throw new InvalidOperationException("No change set given");
            var (isValid, exception) = ChangeSet.IsValidChangeSet();
            if (!isValid) throw exception;

            ChangeSet.Language ??= "English";
        }

        protected abstract void PublishToWorkshop();
    }
}