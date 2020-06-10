using System;

namespace EawXBuild.Exceptions {
    public class NoRelativePathException : Exception {
        public NoRelativePathException(string path) : base($"Path {path} is not a relative path") {
        }
    }
}