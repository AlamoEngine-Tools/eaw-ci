using System.Collections.Generic;
using EawXBuild.Core;

namespace EawXBuild.Configuration
{
    internal interface IConfigParser
    {
        IEnumerable<IProject> Parse(string filePath);
        ConfigVersion Version { get; }
    }
}