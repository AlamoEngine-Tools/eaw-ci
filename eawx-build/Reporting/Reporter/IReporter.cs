namespace EawXBuild.Reporting.Reporter
{
    public interface IReporter
    {
        void ReportMessage(IMessage msg);

        void ReportError(IMessage msg);
    }
}