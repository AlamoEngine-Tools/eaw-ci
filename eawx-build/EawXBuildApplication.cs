using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using EawXBuild.Configuration.CLI;
using EawXBuild.Configuration.FrontendAgnostic;
using EawXBuild.Configuration.Lua.v1;
using EawXBuild.Configuration.Xml.v1;
using EawXBuild.Core;
using EawXBuild.Environment;
using EawXBuild.Reporting;
using EawXBuild.Reporting.Reporter;
using EawXBuild.Services.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
namespace EawXBuild
{
    internal class EawXBuildApplication
    {
        [NotNull] private readonly ILogger<EawXBuildApplication> _logger;

        public EawXBuildApplication(IServiceProvider services, IOptions opts)
        {
            Services = services;
            Opts = opts;
            var loggerFactory = Services.GetRequiredService<ILoggerFactory>() ?? new NullLoggerFactory();
            _logger = loggerFactory.CreateLogger<EawXBuildApplication>();
            _logger.LogTrace("Application initialised successfully");
        }

        public IServiceProvider Services { get; }

        private IOptions Opts { get; }

        internal ExitCode Run()
        {
            return Opts switch
            {
                RunOptions runOptions => RunInternal(runOptions),
                ValidateOptions validateOptions => RunValidateInternal(validateOptions),
                _ => ExitCode.ExUsage
            };
        }

        private ExitCode RunInternal(RunOptions runOptions)
        {
            _logger.LogInformation("Running application in RUN mode");
            var ioService = Services.GetRequiredService<IIOHelperService>();
            IBuildConfigParser? buildConfigParser = null;
            if (runOptions.BackendLua)
                buildConfigParser = GetLuaBuildConfigParserInternal();
            else if (runOptions.BackendXml)
                buildConfigParser = GetXmlBuildConfigParserInternal(ioService);

            if (buildConfigParser == null)
                return ExitCode.ExUsage;

            return !TryExtractAndValidatePathInternal(ioService, runOptions.ConfigPath,
                buildConfigParser.ConfiguredFileExtension,
                out var path,
                out var exitCode)
                ? exitCode
                : ExecRunInternal(runOptions, buildConfigParser, path);
        }

        private static bool TryExtractAndValidatePathInternal(IIOHelperService iioHelperService, string pathIn,
            string fileExtension,
            out string? pathOut, out ExitCode exitCode)
        {
            var currentDirectory = iioHelperService.FileSystem.Directory.GetCurrentDirectory();
            if (!iioHelperService.IsValidPath(pathIn, currentDirectory, fileExtension))
            {
                pathOut = null;
                exitCode = iioHelperService.ValidatePath(pathIn, currentDirectory, fileExtension);
                return false;
            }

            pathOut = iioHelperService.ResolvePath(pathIn, currentDirectory, fileExtension);
            exitCode = ExitCode.Success;
            return true;
        }

        private ExitCode ExecRunInternal(RunOptions runOptions, IBuildConfigParser buildConfigParser, string? path)
        {
            var projects = buildConfigParser.Parse(path);
            var project = projects.FirstOrDefault(p =>
                p.Name.Equals(runOptions.ProjectName, StringComparison.CurrentCulture));

            if (project == null)
            {
                _logger.LogError("No project found for name \"{0}\"", runOptions.ProjectName);
                return ExitCode.ExUsage;
            }

            var tasks = new List<Task>();
            var report = new Report();
            var reporter = Services.GetService<IReporter>();

            report.MessageAddedEvent += (obj, msg) => reporter?.ReportMessage(msg);
            report.ErrorMessageAddedEvent += (obj, msg) => reporter?.ReportError(msg);

            try
            {
                tasks = RunMatchingTasks(runOptions, project, report);
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException e)
            {
                var baseException = e.GetBaseException().Message;
                report.AddMessage(new ErrorMessage($"An error occured: {baseException}"));
            }

            var errCount = 0;
            foreach (var task in tasks.Where(task => null != task.Exception))
            {
                errCount += 1;
                _logger.LogError(task.Exception, "An error occured running the job {0}.{1}", project.Name,
                    runOptions.JobName);
            }

            if (errCount == 0)
            {
                _logger.LogInformation($"{project.Name} ran {tasks.Count} tasks successfully.");
                return ExitCode.Success;
            }

            if (errCount == tasks.Count)
            {
                _logger.LogInformation($"{project.Name} could not be run. All tasks failed.");
                return ExitCode.RunFailed;
            }

            _logger.LogInformation(
                $"{project.Name} could not complete: {errCount} of {tasks.Count} tasks failed.");
            return ExitCode.MultipleJobsFailed;
        }

        private List<Task> RunMatchingTasks(RunOptions runOptions, IProject project, Report report)
        {
            bool noJobName = string.IsNullOrWhiteSpace(runOptions.JobName);
            if (noJobName) return project.RunAllJobsAsync();
            return new List<Task> { project.RunJobAsync(runOptions.JobName, report) };
        }

        internal IBuildConfigParser GetXmlBuildConfigParserInternal(IIOHelperService iioHelperService)
        {
            _logger.LogInformation("Running application with XML backend.");
            return new XmlBuildConfigParser(iioHelperService.FileSystem,
                Services.GetRequiredService<IBuildComponentFactory>(),
                Services.GetRequiredService<ILoggerFactory>());
        }

        internal IBuildConfigParser GetLuaBuildConfigParserInternal()
        {
            _logger.LogInformation("Running application LUA backend.");
            return new LuaBuildConfigParser(
                Services.GetRequiredService<ILuaParser>(), Services.GetRequiredService<IBuildComponentFactory>());
        }

        private ExitCode RunValidateInternal(IOptions validateOptions)
        {
            _logger.LogInformation("Running application in VALIDATION mode.");
            var ioService = Services.GetRequiredService<IIOHelperService>();
            string? fileExtension = null;
            IBuildConfigParser? buildConfigParser = null;

            if (validateOptions.BackendLua)
            {
                fileExtension = ".lua";
                buildConfigParser = GetLuaBuildConfigParserInternal();
            }
            else
            {
                if (validateOptions.BackendXml)
                {
                    fileExtension = ".xml";
                    buildConfigParser = GetXmlBuildConfigParserInternal(ioService);
                }
            }

            if (string.IsNullOrWhiteSpace(fileExtension) || null == buildConfigParser) return ExitCode.ExUsage;

            if (!TryExtractAndValidatePathInternal(ioService, validateOptions.ConfigPath, fileExtension,
                out var path,
                out var exitCode))
                return exitCode;

            return buildConfigParser.TestIsValidConfiguration(path) ? ExitCode.Success : ExitCode.ExConfig;
        }
    }
}