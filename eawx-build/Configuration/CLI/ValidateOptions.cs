using CommandLine;

namespace EawXBuild.Configuration.CLI
{
    [Verb("validate", false, HelpText = "Validates a given configuration file.")]
    public class ValidateOptions : IOptions
    {
        public bool BackendLua { get; set; }
        public bool BackendXml { get; set; }
        public string ConfigPath { get; set; }
        public ConfigVersion Version { get; set; }
        public bool Verbose { get; set; }
    }
}