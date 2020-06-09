using System.Runtime.InteropServices;

namespace EawXBuildTest
{
    public static class TestUtility
    {
        public const string TEST_TYPE_HOLY = "Holy Test";
        public const string TEST_TYPE_UTILITY = "Utility Test";

        public static bool IsWindows()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        public static bool IsLinuxOrMacOS()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        }
    }
}
