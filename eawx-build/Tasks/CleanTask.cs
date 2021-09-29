using System.IO.Abstractions;
using EawXBuild.Core;
using EawXBuild.Exceptions;
using EawXBuild.Reporting;

namespace EawXBuild.Tasks
{
    public class CleanTask : ITask
    {
        private readonly IFileSystem _fileSystem;

        public CleanTask(IFileSystem? fileSystem = null)
        {
            _fileSystem = fileSystem ?? new FileSystem();
        }

        public string Path { get; set; } = "";

        public string Id { get; set; } = "";
        public string Name { get; set; } = "";

        public void Run(Report? report = null)
        {
            report?.AddMessage(new Message($"Deleting file {Path}"));
            if (_fileSystem.Path.IsPathRooted(Path)) throw new NoRelativePathException(Path);
            if (_fileSystem.Directory.Exists(Path)) _fileSystem.Directory.Delete(Path, true);
            else _fileSystem.File.Delete(Path);
        }
    }
}