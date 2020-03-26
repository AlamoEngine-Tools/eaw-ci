namespace EawXBuild
{
    [System.Serializable]
    public class NoSuchFileSystemObjectException : System.Exception
    {
        public NoSuchFileSystemObjectException(string fileSystemObjectName) : base($"No such file or directory: {fileSystemObjectName}") { }
    }
}