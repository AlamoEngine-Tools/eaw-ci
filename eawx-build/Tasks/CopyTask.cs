using System.IO.Abstractions;
using EawXBuild.Core;

namespace EawXBuild.Tasks
{
    public class CopyTask : ITask
    {
        private IFileSystem fileSystem;

        public CopyTask(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
            Recursive = true;
        }

        public string Source { get; set; }
        public string Destination { get; set; }
        public bool Recursive { get; set; }

        public void Run()
        {
            var directory = fileSystem.DirectoryInfo.FromDirectoryName(Source);
            if (directory.Exists)
            {
                IDirectoryInfo destinationDirectory = fileSystem.Directory.CreateDirectory(Destination);
                CopyDirectory(directory, destinationDirectory);
                return;
            }

            var sourceFile = fileSystem.FileInfo.FromFileName(Source);
            if (sourceFile.Exists)
            {
                var destFile = fileSystem.FileInfo.FromFileName(Destination);
                if (!destFile.Directory.Exists)
                    destFile.Directory.Create();

                fileSystem.File.Copy(Source, Destination);
                return;
            }

            throw new NoSuchFileSystemObjectException(Destination);
        }

        private void CopyDirectory(IDirectoryInfo sourceDirectory, IDirectoryInfo destinationDirectory)
        {
            CopyFilesFromDirectory(sourceDirectory, destinationDirectory);
            if (!Recursive) return;

            CopySubDirectories(sourceDirectory, destinationDirectory);
        }

        private void CopyFilesFromDirectory(IDirectoryInfo sourceDirectory, IDirectoryInfo destinationDirectory)
        {
            IFileInfo[] sourceFiles = sourceDirectory.GetFiles();
            foreach (var file in sourceFiles)
            {
                string destFileName = fileSystem.Path.Combine(destinationDirectory.FullName, file.Name);
                file.CopyTo(destFileName);
            }
        }

        private void CopySubDirectories(IDirectoryInfo sourceDirectory, IDirectoryInfo destinationDirectory)
        {
            IDirectoryInfo[] subDirectories = sourceDirectory.GetDirectories();
            foreach (var subDirectory in subDirectories)
            {
                IDirectoryInfo destSubDirectory = destinationDirectory.CreateSubdirectory(subDirectory.Name);
                CopyDirectory(subDirectory, destSubDirectory);
            }
        }
    }
}