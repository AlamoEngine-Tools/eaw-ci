using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using EawXBuild.Environment;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

[assembly: InternalsVisibleTo("eawx-build-test")]

namespace EawXBuild.Services.IO
{
    internal class IOHelperService : IIOHelperService
    {
        private readonly ILogger<IOHelperService> _logger;
        [NotNull] public IFileSystem FileSystem { get; }

        public IOHelperService(IFileSystem fileSystem, ILoggerFactory loggerFactory = null)
        {
            ILoggerFactory lf = loggerFactory ?? new NullLoggerFactory();
            _logger = lf.CreateLogger<IOHelperService>();
            FileSystem = fileSystem ?? new FileSystem();
            _logger.LogTrace("{0} initialised successfully.", nameof(IOHelperService));
        }

        public ExitCode ValidatePath([NotNull] string path, [NotNull] string relativePath = "",
            [NotNull] string fileExtension = "")
        {
            Debug.Assert(path != null, nameof(path) + " != null");
            if (path.IndexOfAny(FileSystem.Path.GetInvalidPathChars()) == -1)
            {
                try
                {
                    ResolvePath(path, relativePath, fileExtension);
                    return ExitCode.Success;
                }
                catch (ArgumentNullException e)
                {
                    _logger.LogError(e, "An error occurred");
                    return ExitCode.ExUsage;
                }
                catch (System.Security.SecurityException e)
                {
                    _logger.LogError(e, "An error occurred");
                    return ExitCode.ExNoperm;
                }
                catch (ArgumentException e)
                {
                    _logger.LogError(e, "An error occurred");
                    return ExitCode.ExUsage;
                }
                catch (UnauthorizedAccessException e)
                {
                    _logger.LogError(e, "An error occurred");
                    return ExitCode.ExNoperm;
                }
                catch (PathTooLongException e)
                {
                    _logger.LogError(e, "An error occurred");
                    return ExitCode.ExOserr;
                }
                catch (NotSupportedException e)
                {
                    _logger.LogError(e, "An error occurred");
                    return ExitCode.ExOserr;
                }
                catch (FileNotFoundException e)
                {
                    _logger.LogError(e, "An error occurred");
                    return ExitCode.ExIoerr;
                }
                catch (IOException e)
                {
                    _logger.LogError(e, "An error occurred");
                    return ExitCode.ExIoerr;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An error occurred");
                    return ExitCode.GenericError;
                }
            }

            _logger.LogError("The provided path \"{0}\" contains unsupported characters.", path);
            return ExitCode.ExOserr;
        }

        public string ResolvePath(string path, string relativePath = "", string fileExtension = "")
        {
            var fullyQualifiedPath = GetFullyQualifiedPath(path, relativePath);
            var fileInfo = FileSystem.FileInfo.FromFileName(fullyQualifiedPath);
            if (!fileInfo.Exists) throw new FileNotFoundException($"The file {path} does not exist");
            if (string.IsNullOrEmpty(fileExtension)) return fullyQualifiedPath;
            if (!FullPathEndsWithExtension(fullyQualifiedPath, fileExtension))
                throw new IOException("An error occurred!");

            fullyQualifiedPath = fullyQualifiedPath.Trim();
            return fullyQualifiedPath;
        }

        private string GetFullyQualifiedPath(string path, string relativePath)
        {
            if (FileSystem.Path.IsPathRooted(path))
                return FileSystem.Path.GetFullPath(path);

            var fullyQualifiedPath = string.IsNullOrEmpty(relativePath)
                ? FileSystem.Path.GetFullPath(relativePath)
                : FileSystem.Path.GetFullPath(FileSystem.Path.Combine(relativePath, path));

            return fullyQualifiedPath;
        }

        private bool FullPathEndsWithExtension(string fullyQualifiedPath, string fileExtension)
        {
            return FileSystem.Path.GetExtension(fullyQualifiedPath)
                .Equals(fileExtension, StringComparison.InvariantCultureIgnoreCase);
        }


        public bool IsValidPath(string path, string relativePath = "", string fileExtension = "")
        {
            return ValidatePath(path, relativePath, fileExtension) == ExitCode.Success;
        }
    }
}