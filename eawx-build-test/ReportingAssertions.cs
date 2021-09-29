using EawXBuild.Reporting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace EawXBuildTest
{
    public static class ReportingAssertions
    {
        public static void AssertMessageContentEquals(string expectedContent, IMessage message)
        {
            Assert.AreEqual(expectedContent, message.MessageContent);
        }
    }
}