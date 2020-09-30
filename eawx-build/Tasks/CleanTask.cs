using System.IO.Abstractions;
using System.Threading;
using EawXBuild.Exceptions;

namespace EawXBuild.Tasks {
    public class CleanTask : SynchronizedTask {
        private readonly IFileSystem _fileSystem;

        public CleanTask(IFileSystem fileSystem = null) {
            _fileSystem = fileSystem ?? new FileSystem();
        }

        public string Path { get; set; }

        public override string ToString()
        {
            return $"Cleaning Path '{Path}'";
        }

        protected override void RunSynchronized(CancellationToken token)
        {
            // Note: It's probably a good idea to put the execution of the cleanup outside of the main job.
            // The cleanup get's a separate job where cancellation tokens are NOT shared. The cleanup token does not
            // listen to the app exit event. Thus we can be sure that cleanup will be performed in any case.
            if (token.IsCancellationRequested)
                return;
            if (_fileSystem.Path.IsPathRooted(Path)) throw new NoRelativePathException(Path);
            if (_fileSystem.Directory.Exists(Path)) _fileSystem.Directory.Delete(Path, true);
            else _fileSystem.File.Delete(Path);
        }
    }
}