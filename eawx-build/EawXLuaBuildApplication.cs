using System.IO.Abstractions;
using EawXBuild.Configuration.CLI;
using EawXBuild.Core;
using EawXBuild.Environment;
using EawXBuild.Tasks;
using NLua;

namespace EawXBuild {
    public class EawXLuaBuildApplication : IEawXBuildApplication {
        private readonly IOptions _options;
        private readonly IFileSystem _fileSystem;


        public EawXLuaBuildApplication(IOptions options, IFileSystem fileSystem) {
            _options = options;
            _fileSystem = fileSystem;
        }

        public ExitCode Run() {
            var task = new CopyTask(new CopyPolicy(), _fileSystem) {Source = "original.txt", Destination = "copy.txt"};
            task.Run();
            return ExitCode.Success;
        }
    }
}