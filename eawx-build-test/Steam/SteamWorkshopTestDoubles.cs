using System.Threading.Tasks;
using EawXBuild.Steam;
using EawXBuild.Steam.Facepunch.Adapters;

namespace EawXBuildTest.Steam {
    public class SteamWorkshopDummy : ISteamWorkshop {
        public virtual uint AppId { get; set; }

        public virtual async Task<WorkshopItemPublishResult> PublishNewWorkshopItemAsync(WorkshopItemChangeSet settings) {
            return null;
        }

        public virtual async Task<IWorkshopItem> QueryWorkshopItemByIdAsync(ulong id) {
            return null;
        }
    }

    public class SteamWorkshopStub : SteamWorkshopDummy {
        public PublishResult Result { get; set; } = PublishResult.Ok;

        public override async Task<WorkshopItemPublishResult> PublishNewWorkshopItemAsync(
            WorkshopItemChangeSet settings) {
            return new WorkshopItemPublishResult(0, Result);
        }
    }

    public class SteamWorkshopSpy : SteamWorkshopStub {
        private uint _appId;

        public override uint AppId {
            get => _appId;
            set {
                _appId = value;
                CallOrder += "a";
            }
        }

        public WorkshopItemChangeSet ReceivedSettings { get; private set; }

        public string CallOrder { get; private set; } = "";

        public override async Task<WorkshopItemPublishResult> PublishNewWorkshopItemAsync(
            WorkshopItemChangeSet settings) {
            ReceivedSettings = settings;

            await Task.CompletedTask;
            CallOrder += "p";

            return new WorkshopItemPublishResult(0, Result);
        }

        public override async Task<IWorkshopItem> QueryWorkshopItemByIdAsync(ulong id) {
            await Task.CompletedTask;
            CallOrder += "q";

            return null;
        }
    }
}