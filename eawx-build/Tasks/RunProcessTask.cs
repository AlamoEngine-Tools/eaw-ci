using System.Diagnostics;
using System.IO.Abstractions;
using EawXBuild.Core;
using EawXBuild.Exceptions;
using EawXBuild.Reporting;
using EawXBuild.Services.Process;

namespace EawXBuild.Tasks
{
    public class RunProcessTask : ITask
    {
        private readonly IFileSystem _filesystem;
        private readonly IProcessRunner _runner;

        public RunProcessTask(IProcessRunner runner, IFileSystem? filesystem = null)
        {
            _runner = runner;
            _filesystem = filesystem ?? new FileSystem();
        }

        public string ExecutablePath { get; set; } = "";
        public string Arguments { get; set; } = "";
        public string WorkingDirectory { get; set; } = System.Environment.CurrentDirectory;
        public bool AllowedToFail { get; set; }

        public string Id { get; set; } = "";
        public string Name { get; set; } = "";

        public void Run(Report? report = null)
        {
            if (_filesystem.Path.IsPathRooted(ExecutablePath)) throw new NoRelativePathException(ExecutablePath);
            report?.AddMessage(new Message($"Running process {ExecutablePath}"));
            _runner.Start(new ProcessStartInfo
            {
                FileName = ExecutablePath,
                Arguments = Arguments,
                WorkingDirectory = WorkingDirectory
            });

            _runner.WaitForExit();
            if (!AllowedToFail && _runner.ExitCode != 0) throw new ProcessFailedException(ExecutablePath);
        }
    }
}