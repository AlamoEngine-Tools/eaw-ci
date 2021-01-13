using EawXBuild;
using EawXBuild.Configuration.CLI;
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
            var app = new EawXBuildApplication(TestUtility.GetConfiguredServiceCollection().BuildServiceProvider(),
                new RunOptions
                {
                    BackendLua = false, BackendXml = false
                });
            var exitCode = app.Run();
            Assert.AreEqual(ExitCode.ExUsage, exitCode);
        }
        
        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        public void GivenAValidServiceProvider__RequiredServicesResolveBAckendCorrectly()
        {
            var app = new EawXBuildApplication(TestUtility.GetConfiguredServiceCollection().BuildServiceProvider(),
                new RunOptions());
            var xmlParser = app.GetXmlBuildConfigParserInternal(app.Services.GetService<IIOService>());
            Assert.IsInstanceOfType(xmlParser, typeof(XmlBuildConfigParser));
            var luaParser = app.GetLuaBuildConfigParserInternal();
            Assert.IsInstanceOfType(luaParser, typeof(LuaBuildConfigParser));
        }
    }
}