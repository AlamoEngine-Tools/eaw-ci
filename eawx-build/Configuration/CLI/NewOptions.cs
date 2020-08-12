using CommandLine;

namespace EawXBuild.Configuration.CLI
{
    [Verb("new", false, HelpText = "Create a new default eaw-ci project.")]
    public class NewOptions : IOptions
    {
        [Option('f', "file",Required = true, Default = ".eaw-ci.xml", HelpText = "The file name of the to-be-created eaw-ci project. You may provide a relative or fully qualified file path.")]
        public string File { get; set; }

        [Option('r', "parser", Required = false, Default = ConfigVersion.V1,
            HelpText = "The parser to use for the configuration file.", Hidden = true)]
        public ConfigVersion Version { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
    }
}