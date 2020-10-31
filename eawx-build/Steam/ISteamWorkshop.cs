using System.Threading.Tasks;

namespace EawXBuild.Steam
{
    public interface ISteamWorkshop
    {
        uint AppId { get; set; }

        Task<WorkshopItemPublishResult> PublishNewWorkshopItemAsync(WorkshopItemChangeSet settings);

        Task<IWorkshopItem> QueryWorkshopItemByIdAsync(ulong id);
    }
}