using System;
using System.Threading.Tasks;
using EawXBuild.Exceptions;
using EawXBuild.Steam;
using EawXBuild.Steam.Facepunch.Adapters;

namespace EawXBuild.Tasks {
    public class UpdateSteamWorkshopItemTask : SteamWorkshopTask {
        private readonly ISteamWorkshop _workshop;

        public UpdateSteamWorkshopItemTask(ISteamWorkshop workshop) {
            _workshop = workshop;
        }

        public uint ItemId { get; set; }

        protected override void PublishToWorkshop() {
            ValidateItemId();
            var item = QueryWorkshopItem();
            UpdateItem(item);
        }

        private void ValidateItemId() {
            if (ItemId == 0) throw new InvalidOperationException("No item ID set");
        }

        private IWorkshopItem QueryWorkshopItem() {
            var queryItemTask = _workshop.QueryWorkshopItemByIdAsync(ItemId);
            Task.WaitAll(queryItemTask);
            var item = queryItemTask.Result;
            return item ?? throw new WorkshopItemNotFoundException("No workshop item with given Id");
        }

        private void UpdateItem(IWorkshopItem item) {
            var updateTask = item.UpdateItemAsync(ChangeSet);
            Task.WaitAll(updateTask);
            if (updateTask.Result is PublishResult.Failed)
                throw new ProcessFailedException("Workshop item update failed");
        }
    }
}