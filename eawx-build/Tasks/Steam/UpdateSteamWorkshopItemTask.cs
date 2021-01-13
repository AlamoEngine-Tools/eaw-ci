using System;
using EawXBuild.Exceptions;
using EawXBuild.Steam;
using EawXBuild.Steam.Facepunch.Adapters;

namespace EawXBuild.Tasks.Steam {
    public class UpdateSteamWorkshopItemTask : SteamWorkshopTask {
        private readonly ISteamWorkshop _workshop;

        public UpdateSteamWorkshopItemTask(ISteamWorkshop workshop) {
            _workshop = workshop;
        }

        public ulong ItemId { get; set; }

        protected override void PublishToWorkshop() {
            ValidateItemId();
            _workshop.Init(AppId);
            try {
                var item = TryQueryWorkshopItemOrThrow();
                TryUpdateItemOrThrow(item);
            }
            finally {
                _workshop.Shutdown();
            }
        }

        private IWorkshopItem TryQueryWorkshopItemOrThrow() {
            var queryItemTask = _workshop.QueryWorkshopItemByIdAsync(ItemId);
            var item = queryItemTask.Result;
            if (item == null)
                throw new WorkshopItemNotFoundException("No workshop item with given Id");

            return item;
        }

        private void TryUpdateItemOrThrow(IWorkshopItem item) {
            var updateTask = item.UpdateItemAsync(ChangeSet);
            if (updateTask.Result is PublishResult.Failed)
                throw new ProcessFailedException("Workshop item update failed");
        }

        private void ValidateItemId() {
            if (ItemId == 0) throw new InvalidOperationException("No ItemId set");
        }
    }
}