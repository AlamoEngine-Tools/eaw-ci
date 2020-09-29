using System.IO.Abstractions;
using System.Threading;
using EawXBuild.Core;
using EawXBuild.Exceptions;

namespace EawXBuild.Tasks {
    public class CleanTask : TaskBase {
        private readonly IFileSystem _fileSystem;

        public CleanTask(IFileSystem fileSystem = null) {
            this._fileSystem = fileSystem ?? new FileSystem();
        }

        public string Path { get; set; }

        protected override void RunCore(CancellationToken token)
        {
            if (_fileSystem.Path.IsPathRooted(Path)) throw new NoRelativePathException(Path);
            if (_fileSystem.Directory.Exists(Path)) _fileSystem.Directory.Delete(Path, true);
            else _fileSystem.File.Delete(Path);
        }
    }
}