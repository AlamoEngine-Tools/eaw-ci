using System.Threading.Tasks;

namespace EawXBuild.Steam
{
    public interface ISteamWorkshop
    {
        int AppId { get; set; }

        Task<PublishResult> PublishNewWorkshopItemAsync(WorkshopItemChangeSet settings);

        Task<IWorkshopItem> QueryWorkshopItemByIdAsync(int id);
    }
}