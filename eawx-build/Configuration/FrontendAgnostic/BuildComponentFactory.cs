using System;
using System.Threading.Tasks;
using EawXBuild.Core;
using EawXBuild.Native;
using EawXBuild.Tasks;
using Microsoft.Extensions.Logging;

namespace EawXBuild.Configuration.FrontendAgnostic {
    internal enum Tasks {
        Clean,
        Copy,
        CreateSteamWorkshopItem,
        RunProgram,
        SoftCopy
    }

    public class BuildComponentFactory : IBuildComponentFactory {
        private readonly ILogger _logger;
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
            var taskType = ParseTaskTypeName(taskTypeName);
            return taskType switch {
                Tasks.RunProgram => new RunProcessTaskBuilder(),
                Tasks.Clean => new CleanTaskBuilder(),
                Tasks.Copy => new CopyTaskBuilder(new CopyPolicy()),
                Tasks.SoftCopy => new CopyTaskBuilder(new LinkCopyPolicy(_fileLinkerFactory.MakeFileLinker())),
                Tasks.CreateSteamWorkshopItem => new CreateSteamWorkshopItemTaskBuilder(),
                _ => null
            };
        }

        private static Tasks ParseTaskTypeName(string taskTypeName) {
            try {
                return Enum.Parse<Tasks>(taskTypeName);
            }
            catch (ArgumentException) {
                throw new InvalidOperationException($"Unknown Task type: {taskTypeName}");
            }
        }
    }
}