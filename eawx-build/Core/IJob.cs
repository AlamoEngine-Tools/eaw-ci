namespace EawXBuild.Core {
    public interface IJob {
        string Name { get; }

        void AddTask(ITask task);

        void Run();
    }
}