using System;
using Steamworks.Ugc;

namespace EawXBuild.Steam.Facepunch.Steamworks {
    public static class Utilities {
        public static void SetEditorVisibility(WorkshopItemChangeSet settings, Editor editor) {
            switch (settings.Visibility) {
                case WorkshopItemVisibility.Private:
                    editor.WithPrivateVisibility();
                    break;
                case WorkshopItemVisibility.Public:
                    editor.WithPublicVisibility();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}