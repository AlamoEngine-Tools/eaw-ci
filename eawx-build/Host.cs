using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using EawXBuild.Configuration.CLI;
using EawXBuild.Configuration.FrontendAgnostic;
using EawXBuild.Core;
using EawXBuild.Environment;
using EawXBuild.Services.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;
using Steamworks;
using Steamworks.Ugc;

namespace EawXBuild
{
    internal class Host
    {
        private static void Main(string[] args)
        {
            // Parser.Default.ParseArguments<RunOptions, ValidateOptions>(args)
            //     .WithParsed<RunOptions>(ExecInternal)
            //     .WithParsed<ValidateOptions>(ExecInternal)
            //     .WithNotParsed(HandleParseErrorsInternal);


            SteamClient.Init(32470);

            var dir = new DirectoryInfo("./test");
            if (!dir.Exists) dir.Create();
            var file = new FileInfo("./test/myfile.txt");
            file.Create().Close();
            
            Console.Out.WriteLine(SteamClient.Name);
            Console.Out.WriteLine(SteamClient.AppId);
            var submitAsync = Editor.NewCommunityFile
                .InLanguage("English")
                .WithPrivateVisibility()
                .ForAppId(32470)
                .WithTitle("Empire at War Expanded: The automatically released version")
                .WithContent(dir)
                .SubmitAsync();

            Task.WaitAll(submitAsync);

            var publishedItemTask = Item.GetAsync(submitAsync.Result.FileId);

            var item = publishedItemTask.Result;
            var updateTask = item?.Edit().WithDescription("My updated description").SubmitAsync();
            Task.WaitAll(updateTask ?? Task.CompletedTask);
            
            dir.Delete(true);
            SteamClient.Shutdown();
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