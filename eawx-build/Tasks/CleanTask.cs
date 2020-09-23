using System.IO.Abstractions;
using EawXBuild.Core;

namespace EawXBuild.Tasks {
    public class CleanTask : ITask {
        private readonly IFileSystem fileSystem;

        public CleanTask(IFileSystem fileSystem = null) {
            this.fileSystem = fileSystem ?? new FileSystem();
        }

        public string Path { get; set; }

        public void Run() {
            fileSystem.File.Delete(Path);
        }
    }
}