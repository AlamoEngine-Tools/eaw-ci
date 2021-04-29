using System.Threading.Tasks;

namespace EawXBuild.Steam
{
    public interface IWorkshopItem
    {
        public ulong ItemId { get; }
        string Title { get; }
        string Description { get; }


        Task<PublishResult> UpdateItemAsync(IWorkshopItemChangeSet settings);
    }
}