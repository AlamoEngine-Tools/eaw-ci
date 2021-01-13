using CommandLine;

namespace EawXBuild.Configuration.CLI {
    public interface IOptions {
        [Option('l', "lua", Default = false, Group = "backend-lua",
            HelpText = "Run the CI Tool with a lua-based configuration.")]
        public bool BackendLua { get; set; }

        [Option('x', "xml", Default = true, Group = "backend-xml",
            HelpText = "Run the CI Tool with a xml-based configuration. This is the default option.")]
        public bool BackendXml { get; set; }

        [Option('c', "config", Required = true, HelpText = "The relative or absolute path to the configuration file.")]
        public string ConfigPath { get; set; }

        [Option('r', "parser", Required = false, Default = ConfigVersion.V1,
            HelpText = "The parser to use for the configuration file.", Hidden = true)]
        ConfigVersion Version { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        bool Verbose { get; set; }
    }
}