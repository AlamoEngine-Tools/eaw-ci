using System;
using Steamworks.Ugc;

namespace EawXBuild.Steam.Facepunch.Adapters
{
    public static class Utilities
    {
        public static ref Editor EditorWithVisibility(WorkshopItemVisibility visibility, ref Editor editor)
        {
            editor = visibility switch
            {
                WorkshopItemVisibility.Private => editor.WithPrivateVisibility(),
                WorkshopItemVisibility.Public => editor.WithPublicVisibility(),
                _ => throw new ArgumentOutOfRangeException()
            };

            return ref editor;
        }
    }
}