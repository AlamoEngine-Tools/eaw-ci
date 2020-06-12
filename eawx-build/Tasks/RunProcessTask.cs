using System.IO;
using System.IO.Abstractions;
using EawXBuild.Core;
using EawXBuild.Exceptions;
using EawXBuild.Services.Process;

namespace EawXBuild.Tasks {
    public class RunProcessTask : ITask {
        private readonly IProcessRunner _runner;
        private readonly IFileSystem _filesystem;

        public RunProcessTask(IProcessRunner runner, IFileSystem filesystem) {
            _runner = runner;
            _filesystem = filesystem;
        }

        public void Run() {
            if (!_filesystem.File.Exists(ExecutablePath))
                throw new FileNotFoundException();

            _runner.Start(ExecutablePath, Arguments);
            _runner.WaitForExit();
            if(_runner.ExitCode != 0) throw new ProcessFailedException();
        }

        public string ExecutablePath { get; set; }
        public string Arguments { get; set; }
    }
}