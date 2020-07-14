using Semver;

namespace EawXBuild.Configuration
{
    internal static class ConfigurationUtility
    {
        internal static bool IsVersionMatch(string versionString, ConfigVersion version)
        {
            if (!SemVersion.TryParse(versionString, out SemVersion semVer, true))
            {
                return false;
            }

            return version switch
            {
                ConfigVersion.V1 => semVer.Major == 1,
                _ => false
            };
        }
    }
}