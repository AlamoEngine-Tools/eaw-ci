namespace EawXBuild.Core
{
    public interface IBuildComponentFactory
    {
        IProject MakeProject();

        IJob MakeJob(string name);

        ITaskBuilder Task(string taskTypeName);
    }
}