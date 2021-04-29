using EawXBuild.Steam;
using EawXBuild.Steam.Facepunch.Adapters;
using EawXBuild.Tasks.Steam;

namespace EawXBuild.Configuration.FrontendAgnostic
{
    public class CreateSteamWorkshopItemTaskBuilder : BaseSteamWorkshopTaskBuilder
    {
        protected override SteamWorkshopTask CreateTaskWithChangeSet(IWorkshopItemChangeSet changeSet)
        {
            return new CreateSteamWorkshopItemTask(FacepunchSteamWorkshopAdapter.Instance)
            {
                ChangeSet = changeSet
            };
        }
    }
}