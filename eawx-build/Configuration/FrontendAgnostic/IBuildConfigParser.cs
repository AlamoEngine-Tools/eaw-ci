using System.Collections.Generic;
using EawXBuild.Core;

namespace EawXBuild.Configuration.FrontendAgnostic
{
    public interface IBuildConfigParser
    {
        ConfigVersion Version { get; }
        string ConfiguredFileExtension { get; }

        public string DefaultConfigFile { get; }
        IEnumerable<IProject> Parse(string filePath);
        bool TestIsValidConfiguration(string filePath);
    }
}