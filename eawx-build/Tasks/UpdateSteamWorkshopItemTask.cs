using System.IO.Abstractions;
using System.Threading.Tasks;
using EawXBuild.Core;
using EawXBuild.Steam;

namespace EawXBuild.Tasks {
    public class UpdateSteamWorkshopItemTask : ITask {
        private readonly ISteamWorkshop _workshop;
        private readonly IFileSystem _fileSystem;

        public UpdateSteamWorkshopItemTask(ISteamWorkshop workshop, IFileSystem fileSystem) {
            _workshop = workshop;
            _fileSystem = fileSystem;
        }

        public uint ItemId { get; set; }
        public string Title { get; set; }
        public string DescriptionFilePath { get; set; }
        public string ItemFolderPath { get; set; }
        public WorkshopItemVisibility Visibility { get; set; }

        public void Run() {
            var queryItemTask = _workshop.QueryWorkshopItemByIdAsync(ItemId);
            Task.WaitAll(queryItemTask);
            var item = queryItemTask.Result;
            var updateTask = item.UpdateItemAsync(new WorkshopItemChangeSet {
                Title = Title,
                Description = GetDescriptionFromFile(),
                ItemFolder = _fileSystem.DirectoryInfo.FromDirectoryName(ItemFolderPath),
                Visibility = Visibility,
            });
        }
        
        private string GetDescriptionFromFile() {
            var description = string.Empty;
            if (string.IsNullOrEmpty(DescriptionFilePath)) return description;
            
            var descriptionFile = _fileSystem.FileInfo.FromFileName(DescriptionFilePath);
            if (!descriptionFile.Exists) return description;
            
            using var reader = descriptionFile.OpenText();
            description = reader.ReadToEnd();
            return description;
        }
    }
}