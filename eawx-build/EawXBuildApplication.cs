using System;
using EawXBuild.Configuration;
using EawXBuild.Configuration.CLI;
using EawXBuild.Configuration.v1;
using EawXBuild.Core;
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
            _logger?.LogTrace("Application initialised successfully.");
        }

        internal ExitCode Run()
        {
            switch (Opts)
            {
                case RunOptions opts:
                    _logger?.LogInformation("Running application in RUN mode.");
                    return ExitCode.Success;
                case ValidateOptions validateOptions:
                {
                    _logger?.LogInformation("Running application in VALIDATION mode.");
                    IIOService ioService = Services.GetService<IIOService>();
                    if (!ioService.IsValidPath(validateOptions.ConfigPath,
                        ioService.FileSystem.Directory.GetCurrentDirectory(), ".xml"))
                    {
                        return ioService.ValidatePath(validateOptions.ConfigPath,
                            ioService.FileSystem.Directory.GetCurrentDirectory(), ".xml");
                    }

                    string path = ioService.ResolvePath(validateOptions.ConfigPath,
                        ioService.FileSystem.Directory.GetCurrentDirectory(), ".xml");
                    IBuildConfigParser buildConfigParser = new BuildConfigParser(ioService.FileSystem,
                        Services.GetService<IBuildComponentFactory>(),
                        Services.GetRequiredService<ILoggerFactory>().CreateLogger<BuildConfigParser>());
                    return buildConfigParser.TestIsValidConfiguration(path) ? ExitCode.Success : ExitCode.ExConfig;
                }
            }

            return ExitCode.ExConfig;
        }
    }
}