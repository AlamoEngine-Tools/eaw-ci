using System;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using EawXBuild;
using EawXBuild.Configuration.CLI;
using EawXBuild.Configuration.FrontendAgnostic;
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
    public class EawXBuildXmlApplicationTest
    {
        private const string DefaultXml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<eaw-ci:BuildConfiguration ConfigVersion=""1.0.0"" xmlns:eaw-ci=""eaw-ci"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""eaw-ci eaw-ci.xsd "">
    <Projects>
        <Project Id=""pid0"" Name=""My-Project"">
            <Jobs>
                <Job Id=""pid0.jid0"" Name=""My-Job"">
                    <Tasks>
                        {0}
                    </Tasks>
                </Job>
            </Jobs>
        </Project>
    </Projects>
</eaw-ci:BuildConfiguration>";

        private const string CopyTask = @"
<Task Id=""idvalue1"" Name=""CopyTask"" xsi:type=""eaw-ci:Copy"">
  <CopyFromPath>eaw-ci.xml</CopyFromPath>
  <CopyToPath>newfile.xml</CopyToPath>
  <CopySubfolders>true</CopySubfolders>
  <CopyFileByPattern>*</CopyFileByPattern>
  <AlwaysOverwrite>false</AlwaysOverwrite>
</Task>
";

        private const string EchoTask = @"
<Task Id=""idvalue2"" Name=""RunProcessTask"" xsi:type=""eaw-ci:RunProgram"">
  <ExecutablePath>echo</ExecutablePath>
  <Arguments>Hello World</Arguments>
</Task>";

        private const string XmlConfigFilePath = "eaw-ci.xml";


        private readonly IFileSystem _fileSystem = new FileSystem();
        private ServiceCollection _services;
        private IFileInfo _xmlConfigFile;


        [TestInitialize]
        public void SetUp()
        {
            _services = ConfigureServices();
        }

        [TestCleanup]
        public void TearDown()
        {
            _xmlConfigFile.Delete();

            if (_fileSystem.File.Exists("newfile.xml"))
                _fileSystem.File.Delete("newfile.xml");
        }

        private void CreateXmlConfigWithTask(string task)
        {
            using StreamWriter stream = _fileSystem.File.CreateText(XmlConfigFilePath);
            stream.Write(DefaultXml, task);
            _xmlConfigFile = _fileSystem.FileInfo.FromFileName(XmlConfigFilePath);
        }

        [TestMethod]
        public void WhenRunningWith_OneProject_OneJob_And_CopyTask__ShouldCopyFileToTarget()
        {
            RunOptions options = new RunOptions
            {
                BackendXml = true,
                ConfigPath = "eaw-ci.xml",
                ProjectName = "pid0",
                JobName = "My-Job"
            };

            CreateXmlConfigWithTask(CopyTask);

            EawXBuildApplication sut = new EawXBuildApplication(_services.BuildServiceProvider(), options);

            sut.Run();

            bool actual = _fileSystem.File.Exists("newfile.xml");
            Assert.IsTrue(actual);
        }

        [PlatformSpecificTestMethod("Linux", "OSX")]
        public void GivenUnixLikeSystem__WhenRunningWith_OneProject_OneJob_And_RunProgramTask__ShouldRunProgram()
        {
            RunOptions options = new RunOptions
            {
                BackendXml = true,
                ConfigPath = "eaw-ci.xml",
                ProjectName = "pid0",
                JobName = "My-Job"
            };

            CreateXmlConfigWithTask(EchoTask);
            StringBuilder stringBuilder = new StringBuilder();
            Console.SetOut(new StringWriter(stringBuilder));

            EawXBuildApplication sut = new EawXBuildApplication(_services.BuildServiceProvider(), options);

            sut.Run();

            string actual = stringBuilder.ToString().Trim();
            Assert.AreEqual("Hello World", actual);
        }

        private static ServiceCollection ConfigureServices(LogLevel logLevel = LogLevel.None)
        {
            ServiceCollection services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());
            services.Configure<LoggerFilterOptions>(options =>
                options.AddFilter<ConsoleLoggerProvider>(null, logLevel));
            services.AddTransient<IBuildComponentFactory, BuildComponentFactory>();
            services.AddTransient<IIOHelperService, IOHelperService>(serviceProvider =>
                new IOHelperService(new FileSystem(), serviceProvider.GetRequiredService<ILoggerFactory>()));
            services.AddTransient<IReporter, DummyReporter>();
            return services;
        }
    }
}