using System;

namespace EawXBuild.Exceptions {
    [Serializable]
    public class NoRelativePathException : Exception {
        public NoRelativePathException(string path) : base($"Path {path} is not a relative path") { }
    }
}