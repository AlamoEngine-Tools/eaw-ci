using System.Threading.Tasks;

namespace EawXBuild.Steam {
    public interface IWorkshopItem {
        public ulong ItemId { get; }

        Task<PublishResult> UpdateItemAsync(WorkshopItemChangeSet settings);
    }
}