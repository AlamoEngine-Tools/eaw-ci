using System.Threading.Tasks;

namespace EawXBuild.Steam
{
    public interface ISteamWorkshop
    {

        void Init(uint appId);

        void Shutdown();

        Task<WorkshopItemPublishResult> PublishNewWorkshopItemAsync(IWorkshopItemChangeSet settings);

        Task<IWorkshopItem> QueryWorkshopItemByIdAsync(ulong id);
    }
}