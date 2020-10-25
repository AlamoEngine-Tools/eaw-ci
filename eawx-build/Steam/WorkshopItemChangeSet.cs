using System.IO;

namespace EawXBuild.Steam
{
    public class WorkshopItemChangeSet
    {
        public string Language { get; set; }
        
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        public DirectoryInfo Content { get; set; }
        
        public WorkshopItemVisibility Visibility { get; set; }
    }
}