using CommandLine;

namespace EawXBuild.Configuration.CLI
{
    [Verb("validate", false, HelpText = "Validates a given configuration file.")]
    public class ValidateOptions : IOptions
    {
        [Option('c', "config", Required = true, HelpText = "The relative or absolute path to the configuration file.")]
        public string ConfigPath { get; set; }

        [Option('r', "parser", Required = false, Default = ConfigVersion.V1,
            HelpText = "The parser to use for the configuration file.", Hidden = true)]
        public ConfigVersion Version { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
    }
}