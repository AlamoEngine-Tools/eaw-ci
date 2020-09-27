using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Security.AccessControl;

namespace EawXBuildTest.Tasks {
    public class MockFileSystemWithFileInfoCopySpy : IFileSystem {
        private readonly CachedFileInfoCopySpyFactory _cachedFileInfoCopySpyFactory;

        public MockFileSystemWithFileInfoCopySpy() {
            _cachedFileInfoCopySpyFactory = new CachedFileInfoCopySpyFactory(FileSystem);
        }

        public MockFileSystem FileSystem { get; set; } = new MockFileSystem();
        public IFileInfoFactory FileInfo => _cachedFileInfoCopySpyFactory;
        public IFile File => FileSystem.File;
        public IDirectory Directory => FileSystem.Directory;
        public IFileStreamFactory FileStream => FileSystem.FileStream;
        public IPath Path => FileSystem.Path;
        public IDirectoryInfoFactory DirectoryInfo => FileSystem.DirectoryInfo;
        public IDriveInfoFactory DriveInfo => FileSystem.DriveInfo;
        public IFileSystemWatcherFactory FileSystemWatcher => FileSystem.FileSystemWatcher;
    }


    public class CachedFileInfoCopySpyFactory : IFileInfoFactory {
        private readonly MockFileSystem _fileSystem;
        private readonly Dictionary<string, IFileInfo> fileCache = new Dictionary<string, IFileInfo>();

        public CachedFileInfoCopySpyFactory(MockFileSystem fileSystem) {
            _fileSystem = fileSystem;
        }

        public IFileInfo FromFileName(string fileName) {
            if (fileCache.ContainsKey(fileName)) return fileCache[fileName];
            var file = new FileInfoCopySpy(_fileSystem, fileName);
            fileCache[fileName] = file;
            return file;
        }
    }

    public class FileInfoCopySpy : IFileInfo {
        private readonly MockFileInfo _fileInfo;

        public FileInfoCopySpy(IMockFileDataAccessor fileSystem, string path) {
            _fileInfo = new MockFileInfo(fileSystem, path);
        }

        public Boolean FileWasCopied { get; set; }

        public void Delete() {
            _fileInfo.Delete();
        }

        public void Refresh() {
            _fileInfo.Refresh();
        }

        public IFileSystem FileSystem => _fileInfo.FileSystem;

        public FileAttributes Attributes {
            get => _fileInfo.Attributes;
            set => _fileInfo.Attributes = value;
        }

        public DateTime CreationTime {
            get => _fileInfo.CreationTime;
            set => _fileInfo.CreationTime = value;
        }

        public DateTime CreationTimeUtc {
            get => _fileInfo.CreationTimeUtc;
            set => _fileInfo.CreationTimeUtc = value;
        }

        public bool Exists => _fileInfo.Exists;
        public string Extension => _fileInfo.Extension;
        public string FullName => _fileInfo.FullName;

        public DateTime LastAccessTime {
            get => _fileInfo.LastAccessTime;
            set => _fileInfo.LastAccessTime = value;
        }

        public DateTime LastAccessTimeUtc {
            get => _fileInfo.LastAccessTimeUtc;
            set => _fileInfo.LastAccessTimeUtc = value;
        }

        public DateTime LastWriteTime {
            get => _fileInfo.LastWriteTime;
            set => _fileInfo.LastWriteTime = value;
        }

        public DateTime LastWriteTimeUtc {
            get => _fileInfo.LastWriteTimeUtc;
            set => _fileInfo.LastWriteTimeUtc = value;
        }

        public string Name => _fileInfo.Name;

        public StreamWriter AppendText() {
            return _fileInfo.AppendText();
        }

        public IFileInfo CopyTo(string destFileName) {
            FileWasCopied = true;
            return _fileInfo.CopyTo(destFileName);
        }

        public IFileInfo CopyTo(string destFileName, bool overwrite) {
            FileWasCopied = true;
            return _fileInfo.CopyTo(destFileName, overwrite);
        }

        public Stream Create() {
            return _fileInfo.Create();
        }

        public StreamWriter CreateText() {
            return _fileInfo.CreateText();
        }

        public void Decrypt() {
            _fileInfo.Decrypt();
        }

        public void Encrypt() {
            _fileInfo.Encrypt();
        }

        public FileSecurity GetAccessControl() {
            return _fileInfo.GetAccessControl();
        }

        public FileSecurity GetAccessControl(AccessControlSections includeSections) {
            return _fileInfo.GetAccessControl(includeSections);
        }

        public void MoveTo(string destFileName) {
            _fileInfo.MoveTo(destFileName);
        }

        public Stream Open(FileMode mode) {
            return _fileInfo.Open(mode);
        }

        public Stream Open(FileMode mode, FileAccess access) {
            return _fileInfo.Open(mode, access);
        }

        public Stream Open(FileMode mode, FileAccess access, FileShare share) {
            return _fileInfo.Open(mode, access, share);
        }

        public Stream OpenRead() {
            return _fileInfo.OpenRead();
        }

        public StreamReader OpenText() {
            return _fileInfo.OpenText();
        }

        public Stream OpenWrite() {
            return _fileInfo.OpenWrite();
        }

        public IFileInfo Replace(string destinationFileName, string destinationBackupFileName) {
            return _fileInfo.Replace(destinationFileName, destinationBackupFileName);
        }

        public IFileInfo Replace(string destinationFileName, string destinationBackupFileName,
            bool ignoreMetadataErrors) {
            return _fileInfo.Replace(destinationFileName, destinationBackupFileName, ignoreMetadataErrors);
        }

        public void SetAccessControl(FileSecurity fileSecurity) {
            _fileInfo.SetAccessControl(fileSecurity);
        }

        public IDirectoryInfo Directory => _fileInfo.Directory;
        public string DirectoryName => _fileInfo.DirectoryName;

        public bool IsReadOnly {
            get => _fileInfo.IsReadOnly;
            set => _fileInfo.IsReadOnly = value;
        }

        public long Length => _fileInfo.Length;
    }
}