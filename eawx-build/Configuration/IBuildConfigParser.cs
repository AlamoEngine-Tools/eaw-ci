using System.Collections.Generic;
using EawXBuild.Core;

namespace EawXBuild.Configuration
{
    public interface IBuildConfigParser
    {
        IEnumerable<IProject> Parse(string filePath);
        bool TestIsValidConfiguration(string filePath);
        ConfigVersion Version { get; }
        string DefaultXml { get; }
    }
}