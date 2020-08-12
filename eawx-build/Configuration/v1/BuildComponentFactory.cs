using System;
using EawXBuild.Core;
using Microsoft.Extensions.Logging;

namespace EawXBuild.Configuration.v1 {
    public class BuildComponentFactory : IBuildComponentFactory
    {

        private readonly ILogger _logger;

        public BuildComponentFactory(ILogger logger = null)
        {
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
                "RunProgram" => new RunProcessTaskBuilder(),
                "Clean" => new CleanTaskBuilder(),
                "Copy" => new CopyTaskBuilder(),
                _ => throw new InvalidOperationException($"Unknown Task type: {taskTypeName}")
            };
        }
    }
}