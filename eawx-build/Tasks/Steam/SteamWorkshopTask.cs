using System;
using EawXBuild.Core;
using EawXBuild.Reporting;
using EawXBuild.Steam;

namespace EawXBuild.Tasks.Steam
{
    public abstract class SteamWorkshopTask : ITask
    {
        public uint AppId { get; set; }

        public IWorkshopItemChangeSet ChangeSet { get; set; }
        public string Id { get; set; }

        public string Name { get; set; }

        public void Run(Report report = null)
        {
            ValidateAppId();
            ValidateChangeSet();
            PublishToWorkshop();
        }

        private void ValidateAppId()
        {
            if (AppId == 0) throw new InvalidOperationException("No AppId set");
        }

        private void ValidateChangeSet()
        {
            if (ChangeSet == null) throw new InvalidOperationException("No change set given");
            (bool isValid, Exception exception) = ChangeSet.IsValidChangeSet();
            if (!isValid) throw exception;

            ChangeSet.Language ??= "English";
        }

        protected abstract void PublishToWorkshop();
    }
}