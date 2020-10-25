using System.Threading.Tasks;

namespace EawXBuild.Steam
{
    public interface IWorkshopItem
    {
        Task<PublishResult> UpdateItemAsync(WorkshopItemChangeSet settings);
    }
}