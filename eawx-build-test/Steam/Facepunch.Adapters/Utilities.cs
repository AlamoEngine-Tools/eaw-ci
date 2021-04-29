using System.IO;
using System.IO.Abstractions;

namespace EawXBuildTest.Steam.Facepunch.Adapters
{
    public class Utilities
    {
        public static IFileInfo CreateSteamAppIdFile(IFileSystem fileSystem)
        {
            IFileInfo steamAppIdFile = fileSystem.FileInfo.FromFileName("steam_appid.txt");
            StreamWriter streamWriter = steamAppIdFile.AppendText();
            streamWriter.WriteLine("32470");
            streamWriter.Close();

            return steamAppIdFile;
        }

        public static IFileInfo CreateDescriptionFile(IFileSystem fileSystem, string descriptionFilePath,
            string description)
        {
            StreamWriter writer = fileSystem.File.CreateText(descriptionFilePath);
            writer.Write(description);
            writer.Close();
            IFileInfo descriptionFile = fileSystem.FileInfo.FromFileName(descriptionFilePath);
            return descriptionFile;
        }

        public static IDirectoryInfo CreateItemFolderWithSingleFile(IFileSystem fileSystem, string steamUploadPath)
        {
            IDirectoryInfo itemFolder = fileSystem.DirectoryInfo.FromDirectoryName(steamUploadPath);
            itemFolder.Create();
            itemFolder.CreateSubdirectory("sub_dir");
            using StreamWriter streamWriter = fileSystem.File.CreateText(steamUploadPath + "/sub_dir/file.txt");
            streamWriter.Write("My awesome content!");
            streamWriter.Close();

            return itemFolder;
        }
    }
}