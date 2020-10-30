using System.IO.Abstractions;

namespace EawXBuild.Steam
{
    public class WorkshopItemChangeSet
    {
        public string Language { get; set; }
        
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        public IDirectoryInfo ItemFolder { get; set; }
        
        public WorkshopItemVisibility Visibility { get; set; }
    }
}