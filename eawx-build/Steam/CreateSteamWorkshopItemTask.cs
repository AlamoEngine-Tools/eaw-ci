using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using EawXBuild.Core;
using EawXBuild.Exceptions;

namespace EawXBuild.Steam {
    public class CreateSteamWorkshopItemTask : ITask {
        private readonly ISteamWorkshop _workshop;
        private readonly IFileSystem _fileSystem;

        public CreateSteamWorkshopItemTask(ISteamWorkshop workshop, IFileSystem fileSystem) {
            _workshop = workshop;
            _fileSystem = fileSystem;
        }

        public uint AppId { get; set; }
        public string Title { get; set; }
        public string ItemFolderPath { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Language { get; set; } = "English";
        public WorkshopItemVisibility Visibility { get; set; } = WorkshopItemVisibility.Private;

        public void Run() {
            ValidateSettings();
            _workshop.AppId = AppId;
            var taskResult = _workshop.PublishNewWorkshopItemAsync(MakeWorkshopSettings());
            Task.WaitAll(taskResult);
            if (taskResult.Result.Result is PublishResult.Failed)
                throw new ProcessFailedException("Failed to publish to Steam Workshop");
        }

        private void ValidateSettings() {
            if (AppId == 0) throw new InvalidOperationException("No AppId set");
            if (string.IsNullOrEmpty(Title)) throw new InvalidOperationException("Workshop item has no title");
            if (_fileSystem.Path.IsPathRooted(ItemFolderPath)) throw new NoRelativePathException(ItemFolderPath);
            if (!_fileSystem.Directory.Exists(ItemFolderPath))
                throw new DirectoryNotFoundException("Workshop item directory does not exist");
        }

        private WorkshopItemChangeSet MakeWorkshopSettings() {
            return new WorkshopItemChangeSet {
                Title = Title,
                Description = Description,
                ItemFolder = _fileSystem.DirectoryInfo.FromDirectoryName(ItemFolderPath),
                Visibility = Visibility,
                Language = Language
            };
        }
    }
}