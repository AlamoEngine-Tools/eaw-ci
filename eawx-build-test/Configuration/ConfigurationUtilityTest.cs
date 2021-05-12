using EawXBuild.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Configuration
{
    [TestClass]
    public class ConfigurationUtilityTest
    {
        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
        [DataRow("0.0.0", ConfigVersion.Invalid, true)]
        [DataRow("0.0.0", ConfigVersion.V1, false)]
        [DataRow("1.20.30-rc1", ConfigVersion.Invalid, false)]
        [DataRow("1.20.30-rc1", ConfigVersion.V1, true)]
        [DataRow("Abracadabra", ConfigVersion.Invalid, true)]
        [DataRow("Abracadabra", ConfigVersion.V1, false)]
        public void Test__IsVersionMatch__ReturnsExpected(string semVer, ConfigVersion version, bool expected)
        {
            bool actual = ConfigurationUtility.IsVersionMatch(semVer, version);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
        [DataRow("0.0.0", ConfigVersion.Invalid)]
        [DataRow("1.20.30-rc1", ConfigVersion.V1)]
        [DataRow("1.0.0", ConfigVersion.V1)]
        [DataRow("1", ConfigVersion.Invalid)]
        [DataRow("Abracadabra", ConfigVersion.Invalid)]
        public void Test__GetConfigVersionInternal__ReturnsExpected(string semVer, ConfigVersion expected)
        {
            ConfigVersion actual = ConfigurationUtility.GetConfigVersionInternal(semVer);
            Assert.AreEqual(expected, actual);
        }
    }
}