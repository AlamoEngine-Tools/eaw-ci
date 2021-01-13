using System.Linq;
using Semver;

namespace EawXBuild.Configuration {
    internal static class ConfigurationUtility {
        private static readonly int[] SUPPORTED_MAJOR_VERSIONS = {1};

        internal static bool IsVersionMatch(string versionString, ConfigVersion version) {
            if (IsVersionInvalid(versionString)) {
                return version == ConfigVersion.Invalid;
            }

            SemVersion.TryParse(versionString, out SemVersion semVer, true);
            return version switch {
                ConfigVersion.V1 => semVer.Major == 1,
                _ => false
            };
        }

        private static bool IsVersionInvalid(string versionString) {
            if (!SemVersion.TryParse(versionString, out SemVersion semVer, true)) {
                return true;
            }

            return !SUPPORTED_MAJOR_VERSIONS.Contains(semVer.Major);
        }

        internal static ConfigVersion GetConfigVersionInternal(string versionString) {
            if (IsVersionInvalid(versionString)) {
                return ConfigVersion.Invalid;
            }

            SemVersion.TryParse(versionString, out SemVersion semVer, true);
            return semVer.Major switch {
                1 => ConfigVersion.V1,
                _ => ConfigVersion.Invalid
            };
        }
    }
}