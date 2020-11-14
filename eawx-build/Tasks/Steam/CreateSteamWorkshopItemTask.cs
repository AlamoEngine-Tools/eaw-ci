using EawXBuild.Exceptions;
using EawXBuild.Steam;

namespace EawXBuild.Tasks.Steam {
    public class CreateSteamWorkshopItemTask : SteamWorkshopTask {
        private readonly ISteamWorkshop _workshop;

        public CreateSteamWorkshopItemTask(ISteamWorkshop workshop) {
            _workshop = workshop;
        }

        protected override void PublishToWorkshop() {
            _workshop.Init(AppId);
            var publishTask = _workshop.PublishNewWorkshopItemAsync(ChangeSet);
            var publishResult = publishTask.Result;
            _workshop.Shutdown();
            if (publishResult.Result is PublishResult.Failed)
                throw new ProcessFailedException("Failed to publish to Steam Workshop");
        }
    }
}