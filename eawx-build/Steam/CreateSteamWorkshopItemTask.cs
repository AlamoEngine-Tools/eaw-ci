using System;
using System.Threading.Tasks;
using EawXBuild.Core;
using EawXBuild.Exceptions;

namespace EawXBuild.Steam {
    public class CreateSteamWorkshopItemTask : ITask {
        private readonly ISteamWorkshop _workshop;

        public CreateSteamWorkshopItemTask(ISteamWorkshop workshop) {
            _workshop = workshop;
        }

        public uint AppId { get; set; }
        public IWorkshopItemChangeSet ChangeSet { get; set; }

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
        }

        private void ValidateAppId() {
            if (AppId == 0) throw new InvalidOperationException("No AppId set");
        }

        private void ValidateChangeSet() {
            if (ChangeSet == null) throw new InvalidOperationException("No change set given");
            var (isValid, exception) = ChangeSet.IsValidNewChangeSet();
            if (!isValid) throw exception;

            ChangeSet.Language ??= "English";
        }
    }
}