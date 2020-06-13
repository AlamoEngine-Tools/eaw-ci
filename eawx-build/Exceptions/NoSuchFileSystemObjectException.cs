using System;

namespace EawXBuild.Exceptions {
    [Serializable]
    public class NoSuchFileSystemObjectException : Exception {
        public NoSuchFileSystemObjectException(string fileSystemObjectName) : base(
            $"No such file or directory: {fileSystemObjectName}") {
        }
    }
}