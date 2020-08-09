using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            return Opts switch
            {
                RunOptions runOptions => RunInternal(runOptions),
                ValidateOptions validateOptions => RunValidateInternal(validateOptions),
                _ => ExitCode.ExConfig
            };
        }

        private ExitCode RunInternal(RunOptions runOptions)
        {
            _logger?.LogInformation("Running application in RUN mode.");
            IIOService ioService = Services.GetService<IIOService>();
            if (!ioService.IsValidPath(runOptions.ConfigPath,
                ioService.FileSystem.Directory.GetCurrentDirectory(), ".xml"))
            {
                return ioService.ValidatePath(runOptions.ConfigPath,
                    ioService.FileSystem.Directory.GetCurrentDirectory(), ".xml");
            }
            string path = ioService.ResolvePath(runOptions.ConfigPath,
                ioService.FileSystem.Directory.GetCurrentDirectory(), ".xml");
            IBuildConfigParser buildConfigParser = new BuildConfigParser(ioService.FileSystem,
                Services.GetService<IBuildComponentFactory>(),
                Services.GetRequiredService<ILoggerFactory>().CreateLogger<BuildConfigParser>());
            IEnumerable<IProject> projects = buildConfigParser.Parse(path);
            IProject project =
                projects.FirstOrDefault(p => p.Name.Equals(runOptions.ProjectName, StringComparison.CurrentCulture));

            if (null == project)
            {
                _logger?.LogError($"No project found for name \"{runOptions.ProjectName}\".");
                return ExitCode.ExConfig;
            }

            List<Task> tasks = new List<Task>();
            if (string.IsNullOrEmpty(runOptions.JobName) || string.IsNullOrWhiteSpace(runOptions.JobName))
            {
                try
                {
                    tasks.AddRange(project.RunAllJobsAsync());
                }
                catch (Exception e)
                {
                    _logger?.LogError($"An error occured running all tasks for {project.Name}: ", e);
                }
            }
            else
            {
                try
                {
                    tasks.Add(project.RunJobAsync(runOptions.JobName));
                }
                catch (Exception e)
                {
                    _logger?.LogError($"An error occured running the job {project.Name}.{runOptions.JobName}: ", e);
                }
            }

            Task.WaitAll(tasks.ToArray());
            int errCount = 0;
            foreach (Task task in tasks.Where(task => null != task.Exception))
            {
                errCount += 1;
                _logger?.LogError($"An error occured running the job {project.Name}.{runOptions.JobName}: ", task.Exception);
            }

            if (errCount == 0)
            {
                _logger?.LogInformation($"{project.Name} ran {tasks.Count} tasks successfully.");
                return ExitCode.Success;
            }

            if (errCount == tasks.Count)
            {
                _logger?.LogInformation($"{project.Name} could not be run. All tasks failed.");
                return ExitCode.RunFailed;
            }

            _logger?.LogInformation($"{project.Name} could not complete: {errCount} of {tasks.Count} tasks failed.");
            return ExitCode.MultipleJobsFailed;
        }

        private ExitCode RunValidateInternal(ValidateOptions opt)
        {
            _logger?.LogInformation("Running application in VALIDATION mode.");
            IIOService ioService = Services.GetService<IIOService>();
            if (!ioService.IsValidPath(opt.ConfigPath,
                ioService.FileSystem.Directory.GetCurrentDirectory(), ".xml"))
            {
                return ioService.ValidatePath(opt.ConfigPath,
                    ioService.FileSystem.Directory.GetCurrentDirectory(), ".xml");
            }
            string path = ioService.ResolvePath(opt.ConfigPath,
                ioService.FileSystem.Directory.GetCurrentDirectory(), ".xml");
            IBuildConfigParser buildConfigParser = new BuildConfigParser(ioService.FileSystem,
                Services.GetService<IBuildComponentFactory>(),
                Services.GetRequiredService<ILoggerFactory>().CreateLogger<BuildConfigParser>());
            return buildConfigParser.TestIsValidConfiguration(path) ? ExitCode.Success : ExitCode.ExConfig;
        }
    }
}