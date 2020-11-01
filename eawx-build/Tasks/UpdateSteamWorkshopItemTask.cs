using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using EawXBuild.Core;
using EawXBuild.Exceptions;
using EawXBuild.Steam;
using EawXBuild.Steam.Facepunch.Adapters;

namespace EawXBuild.Tasks {
    public class UpdateSteamWorkshopItemTask : ITask {
        private readonly ISteamWorkshop _workshop;
        private readonly IFileSystem _fileSystem;

        public UpdateSteamWorkshopItemTask(ISteamWorkshop workshop, IFileSystem fileSystem) {
            _workshop = workshop;
            _fileSystem = fileSystem;
        }

        public uint ItemId { get; set; }

        public WorkshopItemChangeSet ChangeSet { get; set; }

        public void Run() {
            ValidateItemId();
            ValidateChangeSet();
            var item = QueryWorkshopItem();
            UpdateItem(item);
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

        private void ValidateItemId() {
            if (ItemId == 0) throw new InvalidOperationException("No item ID set");
        }

        private void ValidateChangeSet() {
            if (!string.IsNullOrEmpty(ChangeSet.ItemFolderPath)
                && !_fileSystem.Directory.Exists(ChangeSet.ItemFolderPath))
                throw new DirectoryNotFoundException();
            if (!string.IsNullOrEmpty(ChangeSet.DescriptionFilePath)
                && !_fileSystem.File.Exists(ChangeSet.DescriptionFilePath))
                throw new FileNotFoundException();
        }
    }
}