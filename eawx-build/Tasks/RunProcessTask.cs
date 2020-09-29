using System.Diagnostics;
using System.IO.Abstractions;
using System.Threading;
using EawXBuild.Core;
using EawXBuild.Exceptions;
using EawXBuild.Services.Process;

namespace EawXBuild.Tasks {
    public class RunProcessTask : TaskBase {
        private readonly IProcessRunner _runner;
        private readonly IFileSystem _filesystem;

        public RunProcessTask(IProcessRunner runner, IFileSystem filesystem = null) {
            _runner = runner;
            _filesystem = filesystem ?? new FileSystem();
        }

        public string ExecutablePath { get; set; }
        public string Arguments { get; set; }
        public string WorkingDirectory { get; set; } = System.Environment.CurrentDirectory;
        public bool AllowedToFail { get; set; }

        protected override void RunCore(CancellationToken token)
        {
            if (_filesystem.Path.IsPathRooted(ExecutablePath)) throw new NoRelativePathException(ExecutablePath);
            _runner.Start(new ProcessStartInfo
            {
                FileName = ExecutablePath,
                Arguments = Arguments,
                WorkingDirectory = WorkingDirectory
            });

            _runner.WaitForExit();
            if (!AllowedToFail && _runner.ExitCode != 0) throw new ProcessFailedException();
        }
    }
}