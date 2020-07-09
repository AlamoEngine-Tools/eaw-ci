using System;
using EawXBuild.Configuration.CLI;
using EawXBuild.Environment;
using EawXBuild.Services.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EawXBuild
{
    internal class EawXBuildApplication
    {
        public IServiceProvider Services { get; }
        private readonly ILogger<EawXBuildApplication> _logger;

        private IOptions Opts { get; }

        public EawXBuildApplication(IServiceProvider services, IOptions opts)
        {
            Services = services;
            Opts = opts;
            _logger = Services.GetRequiredService<ILoggerFactory>().CreateLogger<EawXBuildApplication>();
            _logger?.LogInformation("Application initialised successfully.");
        }

        internal ExitCode Run()
        {
            IIOService svc = Services.GetService<IIOService>();
            if (!svc.IsValidPath(Opts.ConfigPath, svc.FileSystem.Directory.GetCurrentDirectory(), ".xml"))
            {
                return svc.ValidatePath(Opts.ConfigPath, svc.FileSystem.Directory.GetCurrentDirectory(), ".xml");
            }
            // Do actual work.
            return ExitCode.Success;

        }
    }
}
