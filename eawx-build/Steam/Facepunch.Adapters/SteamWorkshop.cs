using System.IO;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Ugc;
using static EawXBuild.Steam.Facepunch.Steamworks.Utilities;

namespace EawXBuild.Steam.Facepunch.Adapters {
    public class SteamWorkshop : ISteamWorkshop {
        private uint _appId;
        private bool _clientStarted;

        private static SteamWorkshop _INSTANCE;
        public static SteamWorkshop Instance => _INSTANCE ??= new SteamWorkshop();

        private SteamWorkshop() { }

        public uint AppId {
            get => _appId;
            set {
                RestartSteamClient(value);
                _appId = value;
            }
        }

        private void RestartSteamClient(uint newAppId) {
            if (newAppId == _appId) return;
            if (_clientStarted)
                SteamClient.Shutdown();
            SteamClient.Init(newAppId);
            _clientStarted = true;
        }

        public async Task<PublishResult> PublishNewWorkshopItemAsync(WorkshopItemChangeSet settings) {
            var editor = Editor.NewCommunityFile;
            editor.ForAppId(AppId)
                .InLanguage(settings.Language)
                .WithTitle(settings.Title)
                .WithContent(new DirectoryInfo(settings.ItemFolder.FullName));

            SetEditorVisibility(settings, editor);
            var result = await editor.SubmitAsync();

            return result.Success ? PublishResult.Ok : PublishResult.Failed;
        }

        public async Task<IWorkshopItem> QueryWorkshopItemByIdAsync(ulong id) {
            var result = await Item.GetAsync(id);

            if (!result.HasValue)
                throw new WorkshopItemNotFoundException();

            return new WorkshopItem(result.Value);
        }
    }
}