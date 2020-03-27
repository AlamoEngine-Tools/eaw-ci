using System.IO.Abstractions;
using EawXBuild.Core;

namespace EawXBuild.Tasks
{
    public class CleanTask : ITask
    {
        private readonly IFileSystem fileSystem;

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