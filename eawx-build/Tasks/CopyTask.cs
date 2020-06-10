using System.IO.Abstractions;
using EawXBuild.Core;
using EawXBuild.Exceptions;

namespace EawXBuild.Tasks {
    public class CopyTask : ITask {
        private readonly IFileSystem _fileSystem;

        public CopyTask(IFileSystem fileSystem) {
            _fileSystem = fileSystem;
            Recursive = true;
        }

        public string Source { get; set; }
        public string Destination { get; set; }

        public bool Recursive { get; set; }

        public string FilePattern { get; set; }

        public void Run() {
            var directory = _fileSystem.DirectoryInfo.FromDirectoryName(Source);
            var sourceFile = _fileSystem.FileInfo.FromFileName(Source);

            if (directory.Exists) CreateDestDirectoryAndCopySourceDirectory(directory);
            else if (sourceFile.Exists) CopySingleFile(sourceFile, Destination);
            else throw new NoSuchFileSystemObjectException(Source);
        }

        private void CreateDestDirectoryAndCopySourceDirectory(IDirectoryInfo directory) {
            var destinationDirectory = _fileSystem.Directory.CreateDirectory(Destination);
            CopyDirectory(directory, destinationDirectory);
        }

        private void CopySingleFile(IFileInfo sourceFile, string destFilePath) {
            var destFile = _fileSystem.FileInfo.FromFileName(destFilePath);
            if (!destFile.Directory.Exists)
                destFile.Directory.Create();

            if (destFile.Exists && destFile.LastWriteTime > sourceFile.LastWriteTime)
                return;

            sourceFile.CopyTo(destFile.FullName, true);
        }

        private void CopyDirectory(IDirectoryInfo sourceDirectory, IDirectoryInfo destinationDirectory) {
            CopyFilesFromDirectory(sourceDirectory, destinationDirectory);
            if (!Recursive) return;

            CopySubDirectories(sourceDirectory, destinationDirectory);
        }

        private void CopyFilesFromDirectory(IDirectoryInfo sourceDirectory, IDirectoryInfo destinationDirectory) {
            var sourceFiles = FilePattern != null ? sourceDirectory.GetFiles(FilePattern) : sourceDirectory.GetFiles();
            foreach (var sourceFile in sourceFiles) {
                var destFileName = _fileSystem.Path.Combine(destinationDirectory.FullName, sourceFile.Name);
                CopySingleFile(sourceFile, destFileName);
            }
        }

        private void CopySubDirectories(IDirectoryInfo sourceDirectory, IDirectoryInfo destinationDirectory) {
            var subDirectories = sourceDirectory.GetDirectories();
            foreach (var subDirectory in subDirectories) {
                var destSubDirectory = destinationDirectory.CreateSubdirectory(subDirectory.Name);
                CopyDirectory(subDirectory, destSubDirectory);
            }
        }
    }
}