using System;
using System.Threading.Tasks;
using EawXBuild.Exceptions;
using EawXBuild.Steam;

namespace EawXBuild.Tasks {
    public class CreateSteamWorkshopItemTask : SteamWorkshopTask {
        private readonly ISteamWorkshop _workshop;

        public CreateSteamWorkshopItemTask(ISteamWorkshop workshop) {
            _workshop = workshop;
        }

        public uint AppId { get; set; }

        protected override void PublishToWorkshop() {
            ValidateAppId();
            PublishItem();
        }
        
        private void ValidateAppId() {
            if (AppId == 0) throw new InvalidOperationException("No AppId set");
        }

        private void PublishItem() {
            _workshop.AppId = AppId;
            var taskResult = _workshop.PublishNewWorkshopItemAsync(ChangeSet);
            Task.WaitAll(taskResult);
            if (taskResult.Result.Result is PublishResult.Failed)
                throw new ProcessFailedException("Failed to publish to Steam Workshop");
        }
    }
}