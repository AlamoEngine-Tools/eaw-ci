namespace EawXBuild.Configuration.CLI
{
    public interface IOptions
    {
        ConfigVersion Version { get; }
        bool Verbose { get; }
    }
}