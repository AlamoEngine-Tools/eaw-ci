using System;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using EawXBuild;
using EawXBuild.Configuration.CLI;
using EawXBuild.Configuration.FrontendAgnostic;
using EawXBuild.Configuration.Lua.v1;
using EawXBuild.Core;
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
            _services = ConfigureServices(_fileSystem);
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
            local job = proj:add_job('My-Job')
            job:add_task(copy('eaw-ci.lua', 'newfile.lua'))
            ";

            CreateConfigFile(config);

            RunOptions options = new RunOptions
            {
                BackendLua = true,
                ConfigPath = "eaw-ci.lua",
                ProjectName = "pid0",
                JobName = "My-Job"
            };

            EawXBuildApplication sut = new EawXBuildApplication(_services.BuildServiceProvider(), options);

            sut.Run();

            bool actual = _fileSystem.File.Exists("newfile.lua");
            Assert.IsTrue(actual);
        }

        [PlatformSpecificTestMethod("Linux", "OSX")]
        public void
            GivenUnixLikeSystem_And_Config_With_OneProject_OneJob_And_RunProcessTask__WhenRunning__ShouldRunProcess()
        {
            const string config = @"
            local proj = project('pid0')
            local job = proj:add_job('My-Job')
            job:add_task(
                run_process('echo')
                :arguments('Hello World')
            )
            ";

            CreateConfigFile(config);

            StringBuilder stringBuilder = new StringBuilder();
            Console.SetOut(new StringWriter(stringBuilder));

            RunOptions options = new RunOptions
            {
                BackendLua = true,
                ConfigPath = "eaw-ci.lua",
                ProjectName = "pid0",
                JobName = "My-Job"
            };

            EawXBuildApplication sut = new EawXBuildApplication(_services.BuildServiceProvider(), options);

            sut.Run();

            string actual = stringBuilder.ToString().Trim();
            Assert.AreEqual("Hello World", actual);
        }

        private void CreateConfigFile(string content)
        {
            using StreamWriter stream = _fileSystem.File.CreateText(LuaConfigFilePath);
            stream.Write(content);
            _luaConfigFile = _fileSystem.FileInfo.FromFileName(LuaConfigFilePath);
        }

        private static ServiceCollection ConfigureServices(IFileSystem fileSystem, LogLevel logLevel = LogLevel.None)
        {
            ServiceCollection services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());
            services.Configure<LoggerFilterOptions>(options =>
                options.AddFilter<ConsoleLoggerProvider>(null, logLevel));
            services.AddTransient<IBuildComponentFactory, BuildComponentFactory>();
            services.AddTransient<IIOHelperService, IOHelperService>(serviceProvider =>
                new IOHelperService(new FileSystem(), serviceProvider.GetRequiredService<ILoggerFactory>()));
            services.AddTransient<ILuaParser, NLuaParser>();
            return services;
        }
    }
}