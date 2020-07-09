using System.Collections.Generic;
using System.IO.Abstractions;
using CommandLine;
using EawXBuild.Configuration.CLI;
using EawXBuild.Environment;
using EawXBuild.Services.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.Extensions.Options;

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
                    config.AddDebug();
                    config.AddConsole();
                })
                .Configure<LoggerFilterOptions>(options =>
                {
                    options.AddFilter<DebugLoggerProvider>(null, LogLevel.Trace);
                    options.AddFilter<ConsoleLoggerProvider>(null, verbose ? LogLevel.Information : LogLevel.Warning);
                });
            ServiceProvider lsp = serviceCollection.BuildServiceProvider();
            serviceCollection.AddTransient<IIOService, IOService>(s =>
                new IOService(new FileSystem(), lsp.GetRequiredService<ILoggerFactory>().CreateLogger<IOService>()));
        }

        private static void HandleParseErrorsInternal(IEnumerable<Error> errs)
        {
            System.Environment.ExitCode = (int) ExitCode.ExUsage;
        }
    }
}
