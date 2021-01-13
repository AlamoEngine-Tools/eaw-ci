using System;
using EawXBuild.Core;
using EawXBuild.Steam;
using EawXBuild.Steam.Facepunch.Adapters;
using EawXBuild.Tasks.Steam;

namespace EawXBuild.Configuration.FrontendAgnostic {
    public class UpdateSteamWorkshopItemTaskBuilder : BaseSteamWorkshopTaskBuilder {
        public override ITaskBuilder With(string name, object value) {
            Task ??= CreateTaskWithChangeSet(ChangeSet);

            if (!name.Equals("ItemId")) return base.With(name, value);
            var updateTask = (UpdateSteamWorkshopItemTask) Task;
            updateTask.ItemId = Convert.ToUInt64(value);
            return this;
        }

        protected override SteamWorkshopTask CreateTaskWithChangeSet(IWorkshopItemChangeSet changeSet) {
            return new UpdateSteamWorkshopItemTask(FacepunchSteamWorkshopAdapter.Instance) {
                ChangeSet = changeSet
            };
        }
    }
}