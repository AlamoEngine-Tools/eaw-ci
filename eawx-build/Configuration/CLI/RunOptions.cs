using CommandLine;

namespace EawXBuild.Configuration.CLI {
    [Verb("run", true, HelpText = "Run a CI project from an optionally specified configuration file.")]
    internal class RunOptions : IOptions {
        public bool BackendLua { get; set; }

        public bool BackendXml { get; set; }
        public string ConfigPath { get; set; }

        public ConfigVersion Version { get; set; }

        [Option('p', "project", Required = true, HelpText = "Name of the CI project to run.")]
        public string ProjectName { get; set; }

        [Option('j', "job", Required = false, HelpText = "Name of a job specified within a CI project to run.")]
        public string JobName { get; set; }

        public bool Verbose { get; set; }
    }
}