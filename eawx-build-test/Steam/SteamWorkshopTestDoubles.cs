using System.Collections.Generic;
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

        public WorkshopItemPublishResult WorkshopItemPublishResult { get; set; }

        public Dictionary<ulong, IWorkshopItem> WorkshopItemsById { get; } = new Dictionary<ulong, IWorkshopItem>();
        
        public override async Task<WorkshopItemPublishResult> PublishNewWorkshopItemAsync(
            WorkshopItemChangeSet settings) {
            return WorkshopItemPublishResult;
        }
        
        public override Task<IWorkshopItem> QueryWorkshopItemByIdAsync(ulong id) {
            return Task.FromResult(WorkshopItemsById[id]);
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

            return WorkshopItemPublishResult;
        }

        public override async Task<IWorkshopItem> QueryWorkshopItemByIdAsync(ulong id) {
            await Task.CompletedTask;
            CallOrder += "q";

            return null;
        }
    }
}