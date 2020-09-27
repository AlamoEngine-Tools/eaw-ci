using System.IO.Abstractions;
using EawXBuild.Core;
using EawXBuild.Exceptions;

namespace EawXBuild.Tasks {
    public class CleanTask : ITask {
        private readonly IFileSystem fileSystem;

        public CleanTask(IFileSystem fileSystem = null) {
            this.fileSystem = fileSystem ?? new FileSystem();
        }

        public string Path { get; set; }

        public void Run() {
            if (fileSystem.Path.IsPathRooted(Path)) throw new NoRelativePathException(Path);
            if (fileSystem.Directory.Exists(Path)) fileSystem.Directory.Delete(Path, true);
            else fileSystem.File.Delete(Path);
        }
    }
}