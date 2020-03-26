using System.IO.Abstractions;
using EawXBuild.Core;

namespace EawXBuild
{
    public class CleanTask : ITask
    {
        private IFileSystem fileSystem;

        public CleanTask(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public string Path { get; set; }

        public void Run()
        {
            fileSystem.File.Delete(Path);
        }
    }
}