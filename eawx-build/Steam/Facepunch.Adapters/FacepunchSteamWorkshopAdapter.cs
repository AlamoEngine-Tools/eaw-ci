#nullable enable
using System.IO;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Ugc;
using static EawXBuild.Steam.Facepunch.Adapters.Utilities;

namespace EawXBuild.Steam.Facepunch.Adapters {
    public class FacepunchSteamWorkshopAdapter : ISteamWorkshop {
        private bool _clientStarted;

        private static FacepunchSteamWorkshopAdapter _INSTANCE;
        public static FacepunchSteamWorkshopAdapter Instance => _INSTANCE ??= new FacepunchSteamWorkshopAdapter();

        private FacepunchSteamWorkshopAdapter() { }

        public uint AppId { get; set; }

        private void RestartSteamClient(uint newAppId) {
            if (newAppId == SteamClient.AppId && _clientStarted) return;
            if (_clientStarted)
                SteamClient.Shutdown();
            SteamClient.Init(newAppId);
            _clientStarted = true;
        }

        public async Task<WorkshopItemPublishResult> PublishNewWorkshopItemAsync(IWorkshopItemChangeSet settings) {
            RestartSteamClient(AppId);
            var editor = Editor.NewCommunityFile;
            editor = EditorWithVisibility(settings.Visibility, ref editor)
                .ForAppId(AppId)
                .WithTitle(settings.Title)
                .WithDescription(settings.GetDescriptionTextFromFile())
                .WithContent(new DirectoryInfo(settings.ItemFolderPath))
                .InLanguage(settings.Language);
            
            var submitResult = await editor.SubmitAsync();
            var publishResult = submitResult.Success ? PublishResult.Ok : PublishResult.Failed;
            ShutdownSteamClient();

            return new WorkshopItemPublishResult(submitResult.FileId, publishResult);
        }

        public async Task<IWorkshopItem?> QueryWorkshopItemByIdAsync(ulong id) {
            RestartSteamClient(AppId);
            var result = await Item.GetAsync(id);
            ShutdownSteamClient();

            return !result.HasValue ? null : new FacepunchWorkshopItemAdapter(result.Value, AppId);
        }

        private void ShutdownSteamClient() {
            SteamClient.Shutdown();
            _clientStarted = false;
        }
    }
}