using EawXBuild;
using EawXBuild.Configuration.CLI;
using EawXBuild.Configuration.FrontendAgnostic;
using EawXBuild.Configuration.Lua.v1;
using EawXBuild.Configuration.Xml.v1;
using EawXBuild.Environment;
using EawXBuild.Services.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest
{
    [TestClass]
    public class EawXBuildApplicationTest
    {
        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        public void WhenCalledWithInvalidRunOptions__ThenCorrectError()
        {
            EawXBuildApplication app = new EawXBuildApplication(
                TestUtility.GetConfiguredServiceCollection().BuildServiceProvider(),
                new RunOptions
                {
                    BackendLua = false, BackendXml = false
                });
            ExitCode exitCode = app.Run();
            Assert.AreEqual(ExitCode.ExUsage, exitCode);
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        public void GivenAValidServiceProvider__RequiredServicesResolveBackendCorrectly()
        {
            EawXBuildApplication app = new EawXBuildApplication(
                TestUtility.GetConfiguredServiceCollection().BuildServiceProvider(),
                new RunOptions());
            IBuildConfigParser xmlParser =
                app.GetXmlBuildConfigParserInternal(app.Services.GetService<IIOHelperService>());
            Assert.IsInstanceOfType(xmlParser, typeof(XmlBuildConfigParser));
            IBuildConfigParser luaParser = app.GetLuaBuildConfigParserInternal();
            Assert.IsInstanceOfType(luaParser, typeof(LuaBuildConfigParser));
        }
    }
}