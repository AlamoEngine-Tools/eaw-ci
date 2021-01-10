using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using CommandLine;
using EawXBuild.Configuration.CLI;
using EawXBuild.Configuration.FrontendAgnostic;
using EawXBuild.Configuration.Lua.v1;
using EawXBuild.Core;
using EawXBuild.Environment;
using EawXBuild.Services.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;

namespace EawXBuild
{
    internal class Host
    {
        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<RunOptions, ValidateOptions>(args)
                .WithParsed<RunOptions>(ExecInternal)
                .WithParsed<ValidateOptions>(ExecInternal)
                .WithNotParsed(HandleParseErrorsInternal);
        }

        private static void ExecInternal(IOptions opts)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection, opts.Verbose);
            EawXBuildApplication application = new EawXBuildApplication(serviceCollection.BuildServiceProvider(), opts);
            System.Environment.ExitCode = (int) application.Run();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection, bool verbose)
        {
            serviceCollection.AddLogging(config =>
                {
#if DEBUG
                    config.AddDebug();
#endif
                    config.AddConsole();
                })
                .Configure<LoggerFilterOptions>(options =>
                {
#if DEBUG
                    options.AddFilter<DebugLoggerProvider>(null, LogLevel.Trace);
#endif
                    options.AddFilter<ConsoleLoggerProvider>(null, verbose ? LogLevel.Trace : LogLevel.Warning);
                });
            ServiceProvider lsp = serviceCollection.BuildServiceProvider();
            serviceCollection.AddTransient<IIOService, IOService>(s =>
                new IOService(new FileSystem(), lsp.GetRequiredService<ILoggerFactory>().CreateLogger<IOService>()));
            serviceCollection.AddTransient<IBuildComponentFactory, BuildComponentFactory>(s =>
                new BuildComponentFactory(
                    lsp.GetRequiredService<ILoggerFactory>().CreateLogger<BuildComponentFactory>()));
            serviceCollection.AddTransient<ILuaParser, NLuaParser>(s => new NLuaParser());
        }

        private static void HandleParseErrorsInternal(IEnumerable<Error> errs)
        {
            IEnumerable<Error> errors = errs as Error[] ?? errs.ToArray();
            if (errors.OfType<HelpVerbRequestedError>().Any() || errors.OfType<HelpRequestedError>().Any())
            {
                System.Environment.ExitCode = (int) ExitCode.Success;
                return;
            }

            System.Environment.ExitCode = (int) ExitCode.ExUsage;
        }
    }
}