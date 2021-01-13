using System.IO;
using System.IO.Abstractions;

namespace EawXBuildTest.Steam.Facepunch.Adapters {
    public class Utilities {
        public static IFileInfo CreateSteamAppIdFile(IFileSystem fileSystem) {
            var steamAppIdFile = fileSystem.FileInfo.FromFileName("steam_appid.txt");
            var streamWriter = steamAppIdFile.AppendText();
            streamWriter.WriteLine("32470");
            streamWriter.Close();

            return steamAppIdFile;
        }

        public static IFileInfo CreateDescriptionFile(IFileSystem fileSystem, string descriptionFilePath,
            string description) {
            var writer = fileSystem.File.CreateText(descriptionFilePath);
            writer.Write(description);
            writer.Close();
            var descriptionFile = fileSystem.FileInfo.FromFileName(descriptionFilePath);
            return descriptionFile;
        }

        public static IDirectoryInfo CreateItemFolderWithSingleFile(IFileSystem fileSystem, string steamUploadPath) {
            var itemFolder = fileSystem.DirectoryInfo.FromDirectoryName(steamUploadPath);
            itemFolder.Create();
            itemFolder.CreateSubdirectory("sub_dir");
            using var streamWriter = fileSystem.File.CreateText(steamUploadPath + "/sub_dir/file.txt");
            streamWriter.Write("My awesome content!");
            streamWriter.Close();

            return itemFolder;
        }
    }
}