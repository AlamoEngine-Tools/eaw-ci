namespace EawXBuild.Core
{
    public interface IJob
    {
        string Name { get; }

        void Run();
    }
}