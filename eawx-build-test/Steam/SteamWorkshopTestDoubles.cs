using System.Threading.Tasks;
using EawXBuild.Steam;

namespace EawXBuildTest.Steam {
    public class SteamWorkshopDummy : ISteamWorkshop {
        public virtual uint AppId { get; set; }

        public virtual async Task<PublishResult> PublishNewWorkshopItemAsync(WorkshopItemChangeSet settings) {
            return PublishResult.Failed;
        }

        public virtual async Task<IWorkshopItem> QueryWorkshopItemByIdAsync(ulong id) {
            return null;
        }
    }

    public class SteamWorkshopStub : SteamWorkshopDummy {
        public PublishResult Result { get; set; }
    }

    public class SteamWorkshopSpy : SteamWorkshopDummy {
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

        public override async Task<PublishResult> PublishNewWorkshopItemAsync(WorkshopItemChangeSet settings) {
            ReceivedSettings = settings;

            await Task.CompletedTask;
            CallOrder += "p";

            return PublishResult.Ok;
        }

        public override async Task<IWorkshopItem> QueryWorkshopItemByIdAsync(ulong id) {
            await Task.CompletedTask;
            CallOrder += "q";

            return null;
        }
    }
}