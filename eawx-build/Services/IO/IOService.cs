using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using EawXBuild.Environment;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("eawx-build-test")]

namespace EawXBuild.Services.IO
{
    internal class IOService : IIOService
    {
        private readonly ILogger<IOService> _logger;
        [NotNull] public IFileSystem FileSystem { get; }

        public IOService(IFileSystem fileSystem, ILoggerFactory loggerFactory = null)
        {
            _logger = loggerFactory?.CreateLogger<IOService>();
            FileSystem = fileSystem ?? new FileSystem();
            _logger?.LogTrace($"{nameof(IOService)} initialised successfully.");
        }

        public ExitCode ValidatePath([NotNull] string path, [NotNull] string relativePath = "",
            [NotNull] string fileExtension = "")
        {
            Debug.Assert(path != null, nameof(path) + " != null");
            if (path.IndexOfAny(FileSystem.Path.GetInvalidPathChars()) == -1)
            {
                try
                {
                    string p = ResolvePath(path, relativePath, fileExtension);
                    return ExitCode.Success;
                }
                catch (ArgumentNullException e)
                {
                    _logger?.LogError($"{e}");
                    return ExitCode.ExUsage;
                }
                catch (System.Security.SecurityException e)
                {
                    _logger?.LogError($"{e}");
                    return ExitCode.ExNoperm;
                }
                catch (ArgumentException e)
                {
                    _logger?.LogError($"{e}");
                    return ExitCode.ExUsage;
                }
                catch (UnauthorizedAccessException e)
                {
                    _logger?.LogError($"{e}");
                    return ExitCode.ExNoperm;
                }
                catch (PathTooLongException e)
                {
                    _logger?.LogError($"{e}");
                    return ExitCode.ExOserr;
                }
                catch (NotSupportedException e)
                {
                    _logger?.LogError($"{e}");
                    return ExitCode.ExOserr;
                }
                catch (FileNotFoundException e)
                {
                    _logger?.LogError($"{e}");
                    return ExitCode.ExIoerr;
                }
                catch (IOException e)
                {
                    _logger?.LogError($"{e}");
                    return ExitCode.ExIoerr;
                }
                catch (Exception e)
                {
                    _logger?.LogError($"{e}");
                    return ExitCode.GenericError;
                }
            }

            _logger?.LogError($"The provided path {path} contains unsupported characters.");
            return ExitCode.ExOserr;
        }

        public string ResolvePath(string path, string relativePath = "", string fileExtension = "")
        {
            string fullyQualifiedPath = "";
            if (!FileSystem.Path.IsPathRooted(path))
            {
                fullyQualifiedPath = string.IsNullOrEmpty(relativePath)
                    ? FileSystem.Path.GetFullPath(relativePath)
                    : FileSystem.Path.GetFullPath(FileSystem.Path.Combine(relativePath, path));
            }
            else
            {
                fullyQualifiedPath = FileSystem.Path.GetFullPath(path);
            }

            IFileInfo fileInfo = FileSystem.FileInfo.FromFileName(fullyQualifiedPath);
            bool throwEx = fileInfo.Length == -1;
            throwEx = fileInfo.IsReadOnly;
            if (string.IsNullOrEmpty(fileExtension)) return fullyQualifiedPath;
            if (!FileSystem.Path.GetExtension(fullyQualifiedPath)
                .Equals(fileExtension, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new IOException("An error occurred!");
            }

            fullyQualifiedPath = fullyQualifiedPath.Trim();
            return fullyQualifiedPath;
        }


        public bool IsValidPath(string path, string relativePath = "", string fileExtension = "")
        {
            return ValidatePath(path, relativePath, fileExtension) == ExitCode.Success;
        }
    }
}