using System;
using System.Threading.Tasks;
using EawXBuild.Core;
using EawXBuild.Exceptions;
using EawXBuild.Steam;
using EawXBuild.Steam.Facepunch.Adapters;

namespace EawXBuild.Tasks {
    public class UpdateSteamWorkshopItemTask : ITask {
        private readonly ISteamWorkshop _workshop;

        public UpdateSteamWorkshopItemTask(ISteamWorkshop workshop) {
            _workshop = workshop;
        }

        public uint ItemId { get; set; }

        public IWorkshopItemChangeSet ChangeSet { get; set; }

        public void Run() {
            ValidateItemId();
            ValidateChangeSet();

            var item = QueryWorkshopItem();
            UpdateItem(item);
        }

        private void ValidateItemId() {
            if (ItemId == 0) throw new InvalidOperationException("No item ID set");
        }

        private void ValidateChangeSet() {
            if (ChangeSet == null) throw new InvalidOperationException("No change set given");
            var (isValid, exception) = ChangeSet.IsValidNewChangeSet();
            if (!isValid) throw exception;
        }

        private IWorkshopItem QueryWorkshopItem() {
            var queryItemTask = _workshop.QueryWorkshopItemByIdAsync(ItemId);
            Task.WaitAll(queryItemTask);
            var item = queryItemTask.Result;
            if (item == null)
                throw new WorkshopItemNotFoundException("No workshop item with given Id");
            return item;
        }

        private void UpdateItem(IWorkshopItem item) {
            var updateTask = item.UpdateItemAsync(ChangeSet);
            Task.WaitAll(updateTask);
            if (updateTask.Result is PublishResult.Failed)
                throw new ProcessFailedException("Workshop item update failed");
        }
    }
}