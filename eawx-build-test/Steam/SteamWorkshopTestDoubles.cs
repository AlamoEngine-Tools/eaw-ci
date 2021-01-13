using System.Collections.Generic;
using System.Threading.Tasks;
using EawXBuild.Steam;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Steam {
    public class SteamWorkshopDummy : ISteamWorkshop {
        public virtual void Init(uint appId) { }

        public virtual void Shutdown() { }

        public virtual async Task<WorkshopItemPublishResult> PublishNewWorkshopItemAsync(
            IWorkshopItemChangeSet settings) {
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
            IWorkshopItemChangeSet settings) {
            return WorkshopItemPublishResult;
        }

        public override Task<IWorkshopItem> QueryWorkshopItemByIdAsync(ulong id) {
            return Task.FromResult(WorkshopItemsById[id]);
        }
    }

    public class SteamWorkshopSpy : SteamWorkshopStub {
        public uint AppId { get; private set; }

        public IWorkshopItemChangeSet ReceivedSettings { get; private set; }

        public string CallOrder { get; private set; } = "";

        public bool WasInitialized { get; private set; }

        public bool WasShutdown { get; private set; }

        public override void Init(uint appId) {
            AppId = appId;
            CallOrder += "a";
            WasInitialized = true;
        }

        public override void Shutdown() {
            CallOrder += "d";
            WasShutdown = true;
        }

        public override Task<WorkshopItemPublishResult> PublishNewWorkshopItemAsync(IWorkshopItemChangeSet settings) {
            ReceivedSettings = settings;
            CallOrder += "p";

            return Task.FromResult(WorkshopItemPublishResult);
        }

        public override Task<IWorkshopItem> QueryWorkshopItemByIdAsync(ulong id) {
            CallOrder += "q";

            return base.QueryWorkshopItemByIdAsync(id);
        }
    }

    public class VerifyAwaitPublishTaskMock : SteamWorkshopSpy {
        private string _eventOrder = "";

        public override async Task<WorkshopItemPublishResult> PublishNewWorkshopItemAsync(
            IWorkshopItemChangeSet settings) {
            var publishTask = base.PublishNewWorkshopItemAsync(settings);
            Task.WaitAll(publishTask);
            var workshopItemPublishResult = publishTask.Result;

            await Task.Delay(500);
            _eventOrder += "p";
            return workshopItemPublishResult;
        }

        public override void Shutdown() {
            _eventOrder += "d";
        }

        public void Verify() {
            Assert.AreEqual("pd", _eventOrder);
        }
    }
}