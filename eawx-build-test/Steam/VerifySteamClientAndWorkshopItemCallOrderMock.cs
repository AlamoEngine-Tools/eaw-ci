using System.Threading.Tasks;
using EawXBuild.Steam;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Steam {
    public class VerifySteamClientAndWorkshopItemCallOrderMock : ISteamWorkshop, IWorkshopItem {
        private string _eventOrder = "";

        private const string ErrorMessage =
            "Expected Init then await QueryWorkshopItemAsync, await UpdateItemAsync and finally Shutdown";

        public void Verify() {
            Assert.AreEqual("aqud", _eventOrder, ErrorMessage);
        }

        public Task<WorkshopItemPublishResult> PublishNewWorkshopItemAsync(IWorkshopItemChangeSet settings) {
            _eventOrder += "p";
            return Task.FromResult(new WorkshopItemPublishResult(0, PublishResult.Ok));
        }

        public void Init(uint appId) {
            _eventOrder += "a";
        }

        public async Task<IWorkshopItem> QueryWorkshopItemByIdAsync(ulong id) {
            await Task.Delay(100);
            _eventOrder += "q";
            return this;
        }

        public async Task<PublishResult> UpdateItemAsync(IWorkshopItemChangeSet settings) {
            await Task.Delay(100);
            _eventOrder += "u";
            return PublishResult.Ok;
        }

        public void Shutdown() {
            _eventOrder += "d";
        }

        public ulong ItemId => 0;

        public string Title => string.Empty;

        public string Description => string.Empty;
    }
}