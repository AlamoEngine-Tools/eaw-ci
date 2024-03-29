﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Linq;
using CommandLine;
using EawXBuild.Configuration.CLI;
using EawXBuild.Configuration.FrontendAgnostic;
using EawXBuild.Configuration.Lua.v1;
using EawXBuild.Core;
using EawXBuild.Environment;
using EawXBuild.Reporting.Reporter;
using EawXBuild.Services.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;
namespace EawXBuild
{
    [ExcludeFromCodeCoverage]
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
            var serviceCollection = ConfigureServices(opts.Verbose);
            var application = new EawXBuildApplication(serviceCollection.BuildServiceProvider(), opts);
            System.Environment.ExitCode = (int)application.Run();
        }

        private static ServiceCollection ConfigureServices(bool verbose)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureLogging(serviceCollection, verbose);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            serviceCollection.AddTransient<ILuaParser, NLuaParser>();
            serviceCollection.AddTransient<IReporter, ConsoleReporter>();
            serviceCollection.AddTransient<IIOHelperService>(CreateIOHelperService);
            serviceCollection.AddTransient<IBuildComponentFactory>(CreateBuildComponentFactory);

            return serviceCollection;
        }

        private static IBuildComponentFactory CreateBuildComponentFactory(IServiceProvider s)
        {
            return new BuildComponentFactory(GetLoggerFactory(s).CreateLogger<BuildComponentFactory>());
        }

        private static IIOHelperService CreateIOHelperService(IServiceProvider s)
        {
            return new IOHelperService(new FileSystem(), GetLoggerFactory(s));
        }

        private static ILoggerFactory GetLoggerFactory(IServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<ILoggerFactory>();
        }

        private static void ConfigureLogging(IServiceCollection serviceCollection, bool verbose)
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
        }

        private static void HandleParseErrorsInternal(IEnumerable<Error> errs)
        {
            IEnumerable<Error> errors = errs as Error[] ?? errs.ToArray();
            if (errors.OfType<HelpVerbRequestedError>().Any() || errors.OfType<HelpRequestedError>().Any())
            {
                System.Environment.ExitCode = (int)ExitCode.Success;
                return;
            }

            System.Environment.ExitCode = (int)ExitCode.ExUsage;
        }
    }
}