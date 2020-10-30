using System.IO;
using System.Threading.Tasks;
using Steamworks.Ugc;
using static EawXBuild.Steam.Facepunch.Steamworks.Utilities;

namespace EawXBuild.Steam.Facepunch.Adapters {
    public class WorkshopItem : IWorkshopItem {
        private Item _item;
        public WorkshopItem(Item item) {
            _item = item;
        }

        public async Task<PublishResult> UpdateItemAsync(WorkshopItemChangeSet settings) {
            var editor = _item.Edit();
            editor
                .InLanguage(settings.Language)
                .WithTitle(settings.Title)
                .WithContent(new DirectoryInfo(settings.ItemFolder.FullName));

            SetEditorVisibility(settings, editor);
            var result = await editor.SubmitAsync();

            return result.Success ? PublishResult.Ok : PublishResult.Failed;
        }
        
    }
}