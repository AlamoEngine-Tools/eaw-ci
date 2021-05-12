using System.IO.Abstractions;
using EawXBuild.Environment;

namespace EawXBuild.Services.IO
{
    internal interface IIOHelperService
    {
        IFileSystem FileSystem { get; }
        ExitCode ValidatePath(string path, string relativePath = "", string fileExtension = "");
        string ResolvePath(string path, string relativePath = "", string fileExtension = "");
        bool IsValidPath(string path, string relativePath = "", string fileExtension = "");
    }
}