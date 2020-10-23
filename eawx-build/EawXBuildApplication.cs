using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EawXBuild.Configuration.CLI;
using EawXBuild.Configuration.FrontendAgnostic;
using EawXBuild.Configuration.Lua.v1;
using EawXBuild.Configuration.Xml.v1;
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
            if (runOptions.BackendXml)
            {
                return RunBackendXmlInternal(runOptions);
            }

            if (runOptions.BackendLua)
            {
                return RunBackendLuaInternal(runOptions);
            }

            return ExitCode.ExUsage;
        }

        private static bool TryExtractAndValidatePathInternal(IIOService ioService, string pathIn, string fileExtension,
            out string pathOut, out ExitCode exitCode)
        {
            if (!ioService.IsValidPath(pathIn,
                ioService.FileSystem.Directory.GetCurrentDirectory(), fileExtension))
            {
                pathOut = null;
                exitCode = ioService.ValidatePath(pathIn,
                    ioService.FileSystem.Directory.GetCurrentDirectory(), fileExtension);
                return false;
            }

            pathOut = ioService.ResolvePath(pathIn,
                ioService.FileSystem.Directory.GetCurrentDirectory(), fileExtension);
            exitCode = ExitCode.Success;
            return true;
        }

        private ExitCode RunBackendLuaInternal(RunOptions runOptions)
        {
            IIOService ioService = Services.GetService<IIOService>();
            if (!TryExtractAndValidatePathInternal(ioService, runOptions.ConfigPath, ".lua", out string path,
                out ExitCode exitCode))
            {
                return exitCode;
            }

            IBuildConfigParser buildConfigParser = GetLuaBuildConfigParserInternal(ioService);
            return ExecRunInternal(runOptions, buildConfigParser, path);
        }

        private ExitCode RunBackendXmlInternal(RunOptions runOptions)
        {
            IIOService ioService = Services.GetService<IIOService>();
            if (!TryExtractAndValidatePathInternal(ioService, runOptions.ConfigPath, ".xml", out string path,
                out ExitCode exitCode))
            {
                return exitCode;
            }

            IBuildConfigParser buildConfigParser = GetXmlBuildConfigParserInternal(ioService);
            return ExecRunInternal(runOptions, buildConfigParser, path);
        }

        private ExitCode ExecRunInternal(RunOptions runOptions, IBuildConfigParser buildConfigParser, string path)
        {
            IEnumerable<IProject> projects = buildConfigParser.Parse(path);
            IProject project =
                projects.FirstOrDefault(p =>
                    p.Name.Equals(runOptions.ProjectName, StringComparison.CurrentCulture));

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
                _logger?.LogError($"An error occured running the job {project.Name}.{runOptions.JobName}: ",
                    task.Exception);
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

            _logger?.LogInformation(
                $"{project.Name} could not complete: {errCount} of {tasks.Count} tasks failed.");
            return ExitCode.MultipleJobsFailed;
        }

        private XmlBuildConfigParser GetXmlBuildConfigParserInternal(IIOService ioService)
        {
            return new XmlBuildConfigParser(ioService.FileSystem,
                Services.GetService<IBuildComponentFactory>(),
                Services.GetRequiredService<ILoggerFactory>().CreateLogger<XmlBuildConfigParser>());
        }

        private LuaBuildConfigParser GetLuaBuildConfigParserInternal(IIOService ioService)
        {
            return new LuaBuildConfigParser(
                Services.GetService<ILuaParser>(), Services.GetService<IBuildComponentFactory>());
        }

        private ExitCode RunValidateInternal(IOptions validateOptions)
        {
            _logger?.LogInformation("Running application in VALIDATION mode.");
            IIOService ioService = Services.GetService<IIOService>();
            string fileExtension;
            IBuildConfigParser buildConfigParser;
            if (validateOptions.BackendXml)
            {
                fileExtension = ".xml";
                buildConfigParser = GetXmlBuildConfigParserInternal(ioService);
            }
            else
            {
                fileExtension = ".lua";
                buildConfigParser = GetLuaBuildConfigParserInternal(ioService);
            }

            if (!TryExtractAndValidatePathInternal(ioService, validateOptions.ConfigPath, fileExtension, out string path,
                out ExitCode exitCode))
            {
                return exitCode;
            }
            return buildConfigParser.TestIsValidConfiguration(path) ? ExitCode.Success : ExitCode.ExConfig;
        }
    }
}