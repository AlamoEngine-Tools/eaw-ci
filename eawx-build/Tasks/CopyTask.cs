using System.IO.Abstractions;
using EawXBuild.Core;
using EawXBuild.Exceptions;
using EawXBuild.Reporting;
namespace EawXBuild.Tasks
{
    public class CopyTask : ITask
    {
        private readonly ICopyPolicy _copyPolicy;
        private readonly IFileSystem _fileSystem;

        public CopyTask(ICopyPolicy copyPolicy, IFileSystem? fileSystem = null)
        {
            _fileSystem = fileSystem ?? new FileSystem();
            _copyPolicy = copyPolicy;
            Recursive = true;
        }

        public string Source { get; set; }
        public string Destination { get; set; }

        public bool Recursive { get; set; }

        public string FilePattern { get; set; }
        public bool AlwaysOverwrite { get; set; }

        public string Id { get; set; }
        public string Name { get; set; }

        public void Run(Report? report = null)
        {
            CheckRelativePaths();

            var directory = _fileSystem.DirectoryInfo.FromDirectoryName(Source);
            var sourceFile = _fileSystem.FileInfo.FromFileName(Source);

            if (directory.Exists) CreateDestDirectoryAndCopySourceDirectory(directory, report);
            else if (sourceFile.Exists) CopySingleFile(sourceFile, Destination, report);
            else throw new NoSuchFileSystemObjectException(Source);
        }

        private void CheckRelativePaths()
        {
            var path = _fileSystem.Path;
            var anyRooted = path.IsPathRooted(Source) || path.IsPathRooted(Destination);
            if (anyRooted) throw new NoRelativePathException(Source);
        }

        private void CreateDestDirectoryAndCopySourceDirectory(IDirectoryInfo directory, Report? report = null)
        {
            var destinationDirectory = _fileSystem.Directory.CreateDirectory(Destination);
            CopyDirectory(directory, destinationDirectory, report);
        }

        private void CopySingleFile(IFileInfo sourceFile, string destFilePath, Report? report = null)
        {
            var destFile = _fileSystem.FileInfo.FromFileName(destFilePath);
            if (!destFile.Directory.Exists)
                destFile.Directory.Create();

            if (destFile.Exists && destFile.LastWriteTime > sourceFile.LastWriteTime && !AlwaysOverwrite)
                return;

            report?.AddMessage(new Message($"Copying file {sourceFile.FullName} to {destFile.FullName}"));
            _copyPolicy.CopyTo(sourceFile, destFile, true);
        }

        private void CopyDirectory(IDirectoryInfo sourceDirectory, IDirectoryInfo destinationDirectory, Report? report = null)
        {
            CopyFilesFromDirectory(sourceDirectory, destinationDirectory, report);
            if (!Recursive) return;

            CopySubDirectories(sourceDirectory, destinationDirectory, report);
        }

        private void CopyFilesFromDirectory(IDirectoryInfo sourceDirectory, IDirectoryInfo destinationDirectory, Report? report = null)
        {
            var sourceFiles =
                FilePattern != null ? sourceDirectory.GetFiles(FilePattern) : sourceDirectory.GetFiles();
            foreach (var sourceFile in sourceFiles)
            {
                var destFileName = _fileSystem.Path.Combine(destinationDirectory.FullName, sourceFile.Name);
                CopySingleFile(sourceFile, destFileName, report);
            }
        }

        private void CopySubDirectories(IDirectoryInfo sourceDirectory, IDirectoryInfo destinationDirectory, Report? report = null)
        {
            var subDirectories = sourceDirectory.GetDirectories();
            foreach (var subDirectory in subDirectories)
            {
                var destSubDirectory = destinationDirectory.CreateSubdirectory(subDirectory.Name);
                CopyDirectory(subDirectory, destSubDirectory, report);
            }
        }
    }
}