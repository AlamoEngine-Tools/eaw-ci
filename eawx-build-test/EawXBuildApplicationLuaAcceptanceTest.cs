using System;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using EawXBuild;
using EawXBuild.Configuration.CLI;
using EawXBuild.Configuration.FrontendAgnostic;
using EawXBuild.Configuration.Lua.v1;
using EawXBuild.Core;
using EawXBuild.Reporting.Reporter;
using EawXBuild.Services.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest
{
    [TestClass]
    public class EawXBuildApplicationLuaAcceptanceTest
    {
        private const string LuaConfigFilePath = "eaw-ci.lua";
        private readonly IFileSystem _fileSystem = new FileSystem();
        private IFileInfo _luaConfigFile;
        private ServiceCollection _services;


        [TestInitialize]
        public void SetUp()
        {
            _services = ConfigureServices();
        }

        [TestCleanup]
        public void TearDown()
        {
            _luaConfigFile.Delete();

            if (_fileSystem.File.Exists("newfile.lua"))
                _fileSystem.File.Delete("newfile.lua");
        }

        [TestMethod]
        public void GivenConfig_With_OneProject_OneJob_And_CopyTask__WhenRunning__ShouldCopyToTargetLocation()
        {
            const string config = @"
            local proj = project('pid0')
            local job = proj:job('My-Job')
            job:tasks {
                {
                    name = 'Copy eaw-ci',
                    action = copy('eaw-ci.lua', 'newfile.lua')
                }
            }
            ";

            CreateConfigFile(config);

            var options = new RunOptions { BackendLua = true, ConfigPath = "eaw-ci.lua", ProjectName = "pid0", JobName = "My-Job" };

            var sut = new EawXBuildApplication(_services.BuildServiceProvider(), options);

            sut.Run();

            var actual = _fileSystem.File.Exists("newfile.lua");
            Assert.IsTrue(actual);
        }

        [PlatformSpecificTestMethod("Linux", "OSX")]
        public void
            GivenUnixLikeSystem_And_Config_With_OneProject_OneJob_And_RunProcessTask__WhenRunning__ShouldRunProcess()
        {
            const string config = @"
            local proj = project('pid0')
            local job = proj:job('My-Job')
            job:tasks {
                {
                    name = 'Run Echo Unix',
                    action = run_process('echo')
                                :arguments('Hello World')
                }
            }
            ";

            CreateConfigFile(config);

            var stringBuilder = new StringBuilder();
            Console.SetOut(new StringWriter(stringBuilder));

            var options = new RunOptions { BackendLua = true, ConfigPath = "eaw-ci.lua", ProjectName = "pid0", JobName = "My-Job" };

            var sut = new EawXBuildApplication(_services.BuildServiceProvider(), options);

            sut.Run();

            var actual = stringBuilder.ToString().Trim();
            Assert.AreEqual("Hello World", actual);
        }

        [PlatformSpecificTestMethod("Windows")]
        public void
            GivenWindowsSystem_And_Config_With_OneProject_OneJob_And_RunProcessTask__WhenRunning__ShouldRunProcess()
        {
            const string config = @"
            local proj = project('pid0')
            local job = proj:job('My-Job')
            job:tasks {
                {
                    name = 'Run Echo Windows',
                    action = run_process('cmd')
                                :arguments('/c echo Hello World')
                }
            }
            ";

            CreateConfigFile(config);

            var stringBuilder = new StringBuilder();
            Console.SetOut(new StringWriter(stringBuilder));

            var options = new RunOptions { BackendLua = true, ConfigPath = "eaw-ci.lua", ProjectName = "pid0", JobName = "My-Job" };

            var sut = new EawXBuildApplication(_services.BuildServiceProvider(), options);

            sut.Run();

            var actual = stringBuilder.ToString().Trim();
            Assert.AreEqual("Hello World", actual);
        }

        private void CreateConfigFile(string content)
        {
            using var stream = _fileSystem.File.CreateText(LuaConfigFilePath);
            stream.Write(content);
            _luaConfigFile = _fileSystem.FileInfo.FromFileName(LuaConfigFilePath);
        }

        private static ServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();
            AddLoggingService(services);
            AddIOService(services);
            services.AddTransient<IBuildComponentFactory, BuildComponentFactory>();
            services.AddTransient<ILuaParser, NLuaParser>();
            services.AddTransient<IReporter, DummyReporter>();
            return services;
        }

        private static void AddLoggingService(IServiceCollection services)
        {
            services.AddLogging(builder => builder.AddConsole());
            services.Configure<LoggerFilterOptions>(options =>
                options.AddFilter<ConsoleLoggerProvider>(null, LogLevel.None));
        }

        private static void AddIOService(ServiceCollection services)
        {
            services.AddTransient<IIOHelperService, IOHelperService>(serviceProvider =>
                new IOHelperService(new FileSystem(), serviceProvider.GetRequiredService<ILoggerFactory>()));
        }
    }
}