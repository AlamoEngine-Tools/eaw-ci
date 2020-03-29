namespace EawXBuild.Core
{
    public interface IBuildComponentFactory
    {
        IProject MakeProject();

        IJob MakeJob(string name);
    }
}