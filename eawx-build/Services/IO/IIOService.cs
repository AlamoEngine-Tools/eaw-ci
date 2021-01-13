using System.IO.Abstractions;
using EawXBuild.Environment;

namespace EawXBuild.Services.IO {
    internal interface IIOService {
        ExitCode ValidatePath(string path, string relativePath = "", string fileExtension = "");
        string ResolvePath(string path, string relativePath = "", string fileExtension = "");
        bool IsValidPath(string path, string relativePath = "", string fileExtension = "");
        IFileSystem FileSystem { get; }
    }
}