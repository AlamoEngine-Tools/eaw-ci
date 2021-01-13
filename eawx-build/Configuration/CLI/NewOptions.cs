using CommandLine;

namespace EawXBuild.Configuration.CLI {
    [Verb("new", false, HelpText = "Create a new default eaw-ci project.")]
    public class NewOptions : IOptions {
        public bool BackendLua { get; set; }
        public bool BackendXml { get; set; }
        public string ConfigPath { get; set; }
        public ConfigVersion Version { get; set; }
        public bool Verbose { get; set; }
    }
}