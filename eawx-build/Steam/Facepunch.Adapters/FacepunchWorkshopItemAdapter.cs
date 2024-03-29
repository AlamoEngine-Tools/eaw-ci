using System;
using System.IO;
using System.Threading.Tasks;
using Steamworks.Ugc;

namespace EawXBuild.Steam.Facepunch.Adapters
{
    public class FacepunchWorkshopItemAdapter : IWorkshopItem
    {
        private Item _item;

        public FacepunchWorkshopItemAdapter(Item item)
        {
            _item = item;
        }

        public ulong ItemId => _item.Id;
        public string Title => _item.Title;
        public string? Description { get; set; }

        public async Task<PublishResult> UpdateItemAsync(IWorkshopItemChangeSet settings)
        {
            Editor editor = _item.Edit();
            editor
                .WithTitle(settings.Title);
            UpdateDescription(settings, ref editor);
            UpdateVisibility(settings, ref editor);
            UpdateContent(settings, ref editor);
            UpdateTags(settings, ref editor);

            Steamworks.Ugc.PublishResult result = await editor.SubmitAsync();

            return result.Success ? PublishResult.Ok : PublishResult.Failed;
        }

        private static void UpdateDescription(IWorkshopItemChangeSet settings, ref Editor editor)
        {
            string? description = null;
            if (settings.DescriptionFilePath != null)
            {
                FileInfo descriptionFile = new FileInfo(settings.DescriptionFilePath);
                using StreamReader streamReader = descriptionFile.OpenText();
                description = streamReader.ReadToEnd();
            }

            editor.WithDescription(description);
        }

        private static void UpdateVisibility(IWorkshopItemChangeSet settings, ref Editor editor)
        {
            switch (settings.Visibility)
            {
                case WorkshopItemVisibility.Public:
                    editor.WithPublicVisibility();
                    break;
                case WorkshopItemVisibility.Private:
                    editor.WithPrivateVisibility();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void UpdateContent(IWorkshopItemChangeSet settings, ref Editor editor)
        {
            if (settings.ItemFolderPath != null)
                editor.WithContent(settings.ItemFolderPath);
        }

        private static void UpdateTags(IWorkshopItemChangeSet settings, ref Editor editor)
        {
            foreach (string tag in settings.Tags) editor.WithTag(tag);
        }
    }
}