using System;
using System.IO.Abstractions;
using EawXBuild.Core;
using EawXBuild.Services.Process;
using EawXBuild.Tasks;

namespace EawXBuild.Configuration.v1 {
    public class RunProcessTaskBuilder : ITaskBuilder {
        private readonly RunProcessTask _runProcessTask;

        public RunProcessTaskBuilder(IFileSystem fileSystem = null) {
            _runProcessTask = new RunProcessTask(new ProcessRunner(), fileSystem ?? new FileSystem());
        }

        public ITaskBuilder With(string name, object value) {
            switch (name) {
                case "ExecutablePath":
                    _runProcessTask.ExecutablePath = (string) value;
                    break;
                case "Arguments":
                    _runProcessTask.Arguments = (string) value;
                    break;
                default:
                    throw new InvalidOperationException($"Invalid configuration option: {name}");
            }

            return this;
        }

        public ITask Build() {
            return _runProcessTask;
        }
    }
}