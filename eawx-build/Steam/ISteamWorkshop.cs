using System.Threading.Tasks;

namespace EawXBuild.Steam
{
    public interface ISteamWorkshop
    {
        uint AppId { get; set; }

        Task<PublishResult> PublishNewWorkshopItemAsync(WorkshopItemChangeSet settings);

        Task<IWorkshopItem> QueryWorkshopItemByIdAsync(ulong id);
    }
}