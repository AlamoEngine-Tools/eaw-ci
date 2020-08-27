using System;
using System.IO.Abstractions;
using EawXBuild.Core;
using EawXBuild.Native;
using EawXBuild.Tasks;
using Microsoft.Extensions.Logging;

namespace EawXBuild.Configuration.v1 {
    public class BuildComponentFactory : IBuildComponentFactory {
        private readonly ILogger _logger;
        private readonly FileSystem _fileSystem = new FileSystem();
        private readonly FileLinkerFactory _fileLinkerFactory = new FileLinkerFactory();

        public BuildComponentFactory(ILogger logger = null) {
            _logger = logger;
        }

        public IProject MakeProject() {
            return new Project();
        }

        public IJob MakeJob(string name) {
            return new Job(name);
        }

        public ITaskBuilder Task(string taskTypeName) {
            return taskTypeName switch {
                "RunProgram" => new RunProcessTaskBuilder(_fileSystem),
                "Clean" => new CleanTaskBuilder(_fileSystem),
                "Copy" => new CopyTaskBuilder(_fileSystem, new CopyPolicy()),
                "SoftCopy" => new CopyTaskBuilder(_fileSystem, new LinkCopyPolicy(_fileLinkerFactory.MakeFileLinker())),
                _ => throw new InvalidOperationException($"Unknown Task type: {taskTypeName}")
            };
        }
    }
}