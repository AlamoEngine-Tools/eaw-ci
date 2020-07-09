namespace EawXBuild.Configuration.CLI
{
    internal interface IOptions
    {
        string ConfigPath { get; }
        public bool Verbose { get; }
    }
}