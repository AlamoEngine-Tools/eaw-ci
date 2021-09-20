namespace EawXBuild.Reporting.Reporter
{
    class DummyReporter : IReporter
    {
        public void ReportError(IMessage msg)
        {
        }

        public void ReportMessage(IMessage msg)
        {
        }
    }
}