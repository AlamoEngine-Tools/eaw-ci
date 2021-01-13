using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using EawXBuild.Environment;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("eawx-build-test")]

namespace EawXBuild.Services.IO {
    internal class IOService : IIOService {
        private readonly ILogger<IOService> _logger;
        [NotNull] public IFileSystem FileSystem { get; }

        public IOService(IFileSystem fileSystem, ILoggerFactory loggerFactory = null) {
            _logger = loggerFactory?.CreateLogger<IOService>();
            FileSystem = fileSystem ?? new FileSystem();
            _logger?.LogTrace($"{nameof(IOService)} initialised successfully.");
        }

        public ExitCode ValidatePath([NotNull] string path, [NotNull] string relativePath = "",
            [NotNull] string fileExtension = "") {
            Debug.Assert(path != null, nameof(path) + " != null");
            if (path.IndexOfAny(FileSystem.Path.GetInvalidPathChars()) == -1) {
                try {
                    ResolvePath(path, relativePath, fileExtension);
                    return ExitCode.Success;
                }
                catch (ArgumentNullException e) {
                    _logger?.LogError($"{e}");
                    return ExitCode.ExUsage;
                }
                catch (System.Security.SecurityException e) {
                    _logger?.LogError($"{e}");
                    return ExitCode.ExNoperm;
                }
                catch (ArgumentException e) {
                    _logger?.LogError($"{e}");
                    return ExitCode.ExUsage;
                }
                catch (UnauthorizedAccessException e) {
                    _logger?.LogError($"{e}");
                    return ExitCode.ExNoperm;
                }
                catch (PathTooLongException e) {
                    _logger?.LogError($"{e}");
                    return ExitCode.ExOserr;
                }
                catch (NotSupportedException e) {
                    _logger?.LogError($"{e}");
                    return ExitCode.ExOserr;
                }
                catch (FileNotFoundException e) {
                    _logger?.LogError($"{e}");
                    return ExitCode.ExIoerr;
                }
                catch (IOException e) {
                    _logger?.LogError($"{e}");
                    return ExitCode.ExIoerr;
                }
                catch (Exception e) {
                    _logger?.LogError($"{e}");
                    return ExitCode.GenericError;
                }
            }

            _logger?.LogError($"The provided path {path} contains unsupported characters.");
            return ExitCode.ExOserr;
        }

        public string ResolvePath(string path, string relativePath = "", string fileExtension = "") {
            var fullyQualifiedPath = GetFullyQualifiedPath(path, relativePath);
            var fileInfo = FileSystem.FileInfo.FromFileName(fullyQualifiedPath);
            if (!fileInfo.Exists) throw new FileNotFoundException($"The file {path} does not exist");
            if (string.IsNullOrEmpty(fileExtension)) return fullyQualifiedPath;
            if (!FullPathEndsWithExtension(fullyQualifiedPath, fileExtension))
                throw new IOException("An error occurred!");

            fullyQualifiedPath = fullyQualifiedPath.Trim();
            return fullyQualifiedPath;
        }

        private string GetFullyQualifiedPath(string path, string relativePath) {
            if (FileSystem.Path.IsPathRooted(path))
                return FileSystem.Path.GetFullPath(path);

            var fullyQualifiedPath = string.IsNullOrEmpty(relativePath)
                ? FileSystem.Path.GetFullPath(relativePath)
                : FileSystem.Path.GetFullPath(FileSystem.Path.Combine(relativePath, path));

            return fullyQualifiedPath;
        }

        private bool FullPathEndsWithExtension(string fullyQualifiedPath, string fileExtension) {
            return FileSystem.Path.GetExtension(fullyQualifiedPath)
                .Equals(fileExtension, StringComparison.InvariantCultureIgnoreCase);
        }


        public bool IsValidPath(string path, string relativePath = "", string fileExtension = "") {
            return ValidatePath(path, relativePath, fileExtension) == ExitCode.Success;
        }
    }
}