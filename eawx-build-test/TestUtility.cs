using System.IO.Abstractions.TestingHelpers;
using System.Runtime.InteropServices;
using EawXBuild.Configuration.FrontendAgnostic;
using EawXBuild.Configuration.Lua.v1;
using EawXBuild.Core;
using EawXBuild.Services.IO;
using EawXBuildTest.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;

namespace EawXBuildTest {
    public static class TestUtility {
        public const string TEST_TYPE_HOLY = "Holy Test";
        public const string TEST_TYPE_UTILITY = "Utility Test";

        public static bool IsWindows() {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        public static bool IsLinuxOrMacOS() {
            return IsLinux() || IsMacOS();
        }

        public static bool IsLinux() {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }

        public static bool IsMacOS() {
            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        }

        public static IServiceCollection GetConfiguredServiceCollection(bool verbose = true) {
            GetConfiguredMockFileSystem(out var mockFileSystem,
                out _);
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(config => {
                    config.AddDebug();
                    config.AddConsole();
                })
                .Configure<LoggerFilterOptions>(options => {
                    options.AddFilter<DebugLoggerProvider>(null, LogLevel.Trace);
                    options.AddFilter<ConsoleLoggerProvider>(null, verbose ? LogLevel.Trace : LogLevel.Warning);
                });
            var lsp = serviceCollection.BuildServiceProvider();
            serviceCollection.AddTransient<IIOService, IOService>(s =>
                new IOService(mockFileSystem, lsp.GetRequiredService<ILoggerFactory>()));
            serviceCollection.AddTransient<IBuildComponentFactory, BuildComponentFactory>(s =>
                new BuildComponentFactory(
                    lsp.GetRequiredService<ILoggerFactory>().CreateLogger<BuildComponentFactory>()));
            serviceCollection.AddTransient<ILuaParser, NLuaParser>(s => new NLuaParser());
            return serviceCollection;
        }

        public static void GetConfiguredMockFileSystem(out MockFileSystem mockFileSystem,
            out FileSystemAssertions fileSystemAssertions) {
            mockFileSystem = null;
            fileSystemAssertions = null;
            if (IsWindows()) {
                mockFileSystem = new MockFileSystem();
                mockFileSystem.AddDirectory("C:/data/test/path");
                mockFileSystem.AddDirectory("C:/data/path");
                mockFileSystem.AddFile("C:/data/test/path/test.dat", new MockFileData(string.Empty));
                mockFileSystem.AddFile("C:/data/path/test.xml", new MockFileData(string.Empty));
                fileSystemAssertions = new FileSystemAssertions(mockFileSystem);
                fileSystemAssertions.AssertDirectoryExists("C:/data/test/path");
                fileSystemAssertions.AssertDirectoryExists("C:/data/path");
                fileSystemAssertions.AssertFileExists("C:/data/test/path/test.dat");
                fileSystemAssertions.AssertFileExists("C:/data/path/test.xml");
            }

            if (IsLinuxOrMacOS()) {
                mockFileSystem = new MockFileSystem();
                mockFileSystem.AddDirectory("/mnt/c/data/test/path");
                mockFileSystem.AddDirectory("/mnt/c/data/path");
                mockFileSystem.AddFile("/mnt/c/data/test/path/test.dat", new MockFileData(string.Empty));
                mockFileSystem.AddFile("/mnt/c/data/path/test.xml", new MockFileData(string.Empty));
                fileSystemAssertions = new FileSystemAssertions(mockFileSystem);
                fileSystemAssertions.AssertDirectoryExists("/mnt/c/data/test/path");
                fileSystemAssertions.AssertDirectoryExists("/mnt/c/data/path");
                fileSystemAssertions.AssertFileExists("/mnt/c/data/test/path/test.dat");
                fileSystemAssertions.AssertFileExists("/mnt/c/data/path/test.xml");
            }
        }
    }
}