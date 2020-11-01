using System.Threading.Tasks;
using EawXBuild.Steam;

namespace EawXBuildTest.Steam {
    public class WorkshopItemStub : IWorkshopItem {

        public PublishResult Result { get; set; } = PublishResult.Ok;
        public ulong ItemId { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public virtual async Task<PublishResult> UpdateItemAsync(WorkshopItemChangeSet settings) {
            return Result;
        }
    }

    public class WorkshopItemSpy : WorkshopItemStub {
        
        public WorkshopItemChangeSet ReceivedSettings { get; private set; }
        
        public override Task<PublishResult> UpdateItemAsync(WorkshopItemChangeSet settings) {
            ReceivedSettings = settings;
            return base.UpdateItemAsync(settings);
        }
    }
}