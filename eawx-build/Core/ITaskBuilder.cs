namespace EawXBuild.Core {
    public interface ITaskBuilder {
        ITaskBuilder With(string name, object value);

        ITask Build();
    }
}