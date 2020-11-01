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
        public WorkshopItemChangeSet ChangeSet { get; set; }

        public void Run() {
            ValidateSettings();
            _workshop.AppId = AppId;
            var taskResult = _workshop.PublishNewWorkshopItemAsync(ChangeSet);
            Task.WaitAll(taskResult);
            if (taskResult.Result.Result is PublishResult.Failed)
                throw new ProcessFailedException("Failed to publish to Steam Workshop");
        }

        private void ValidateSettings() {
            ValidateAppId();
            ValidateChangeSet();
            ValidateItemFolderPath();
            ValidateDescriptionFilePath();
        }

        private void ValidateAppId() {
            if (AppId == 0) throw new InvalidOperationException("No AppId set");
        }

        private void ValidateChangeSet() {
            if (ChangeSet == null) throw new InvalidOperationException("No change set given");
        }

        private void ValidateItemFolderPath() {
            if (_fileSystem.Path.IsPathRooted(ChangeSet.ItemFolderPath))
                throw new NoRelativePathException(ChangeSet.ItemFolderPath);
            if (!_fileSystem.Directory.Exists(ChangeSet.ItemFolderPath))
                throw new DirectoryNotFoundException("Workshop item directory does not exist");
        }

        private void ValidateDescriptionFilePath() {
            if (_fileSystem.Path.IsPathRooted(ChangeSet.DescriptionFilePath))
                throw new NoRelativePathException(ChangeSet.DescriptionFilePath);
            if (!string.IsNullOrEmpty(ChangeSet.DescriptionFilePath) && !_fileSystem.File.Exists(ChangeSet.DescriptionFilePath))
                throw new FileNotFoundException("Description file does not exist");
        }
    }
}