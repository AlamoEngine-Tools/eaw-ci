using System.Threading.Tasks;
using EawXBuild.Steam;

namespace EawXBuildTest.Steam {
    public class WorkshopItemStub : IWorkshopItem {
        public ulong ItemId { get; set; }

        public async Task<PublishResult> UpdateItemAsync(WorkshopItemChangeSet settings) {
            return PublishResult.Failed;
        }
    }
}