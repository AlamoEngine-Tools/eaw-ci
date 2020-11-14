#nullable enable
using System.IO;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Ugc;
using static EawXBuild.Steam.Facepunch.Adapters.Utilities;

namespace EawXBuild.Steam.Facepunch.Adapters {
    public class FacepunchSteamWorkshopAdapter : ISteamWorkshop {

        public static FacepunchSteamWorkshopAdapter Instance { get; } = new FacepunchSteamWorkshopAdapter();

        private FacepunchSteamWorkshopAdapter() { }

        private uint AppId { get; set; }

        public void Init(uint appId) {
            SteamClient.Init(appId);
            AppId = appId;
        }

        public void Shutdown() {
            SteamClient.Shutdown();
        }

        public async Task<WorkshopItemPublishResult> PublishNewWorkshopItemAsync(IWorkshopItemChangeSet settings) {
            var editor = Editor.NewCommunityFile;
            editor = EditorWithVisibility(settings.Visibility, ref editor)
                .ForAppId(AppId)
                .WithTitle(settings.Title)
                .WithDescription(settings.GetDescriptionTextFromFile())
                .WithContent(new DirectoryInfo(settings.ItemFolderPath))
                .InLanguage(settings.Language);
            
            var submitResult = await editor.SubmitAsync();
            var publishResult = submitResult.Success ? PublishResult.Ok : PublishResult.Failed;

            return new WorkshopItemPublishResult(submitResult.FileId, publishResult);
        }

        public async Task<IWorkshopItem?> QueryWorkshopItemByIdAsync(ulong id) {
            var result = await Item.GetAsync(id);

            return !result.HasValue ? null : new FacepunchWorkshopItemAdapter(result.Value, AppId);
        }
    }
}