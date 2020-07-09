using CommandLine;

namespace EawXBuild.Configuration.CLI
{
    [Verb("run", true, HelpText = "Run a CI project from an optionally specified configuration file.")]
    internal class RunOptions : IOptions
    {
        [Option('c', "config", Required = false, HelpText = "The relative or absolute path to the configuration file.",
            Default = ".eaw-ci.xml")]
        public string ConfigPath { get; set; }

        [Option('p', "project", Required = true, HelpText = "Name of the CI project to run.")]
        public string ProjectName { get; set; }

        [Option('j', "job", Required = false, HelpText = "Name of a job specified within a CI project to run.")]
        public string JobName { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
    }
}