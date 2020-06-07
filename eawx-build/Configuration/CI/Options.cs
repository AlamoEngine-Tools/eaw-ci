using CommandLine;

namespace EawXBuild.Configuration.CI
{
    internal class Options
    {
        [Option('c', "config", Required = true, HelpText = "The relative path to the configuration file.")]
        public string ConfigPath { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
    }
}
