using System.IO;
using System.Threading.Tasks;
using Steamworks.Ugc;
using static EawXBuild.Steam.Facepunch.Adapters.Utilities;

namespace EawXBuild.Steam.Facepunch.Adapters {
    public class WorkshopItem : IWorkshopItem {
        private Item _item;
        private readonly uint _appId;

        public WorkshopItem(Item item, uint appId) {
            _item = item;
            _appId = appId;
        }

        public ulong ItemId => _item.Id;
        public string Title => _item.Title;
        public string Description { get; set; }

        public async Task<PublishResult> UpdateItemAsync(WorkshopItemChangeSet settings) {
            var editor = _item.Edit();
            editor
                .InLanguage(settings.Language)
                .WithTitle(settings.Title)
                .WithContent(new DirectoryInfo(settings.ItemFolder.FullName));

            EditorWithVisibility(settings.Visibility, ref editor);
            var result = await editor.SubmitAsync();

            return result.Success ? PublishResult.Ok : PublishResult.Failed;
        }
        
    }
}