using System.Collections.Generic;
using EawXBuild.Reporting;
using EawXBuild.Reporting.Export;
namespace EawXBuildTest.Reporting
{
    public class ReportDummy : Report
    {
        public override void Export(ExportType exportType = ExportType.Full)
        {
        }

        public override void FinalizeReport()
        {
        }
        protected override void OnReportFinalized()
        {
        }
        public override void AddMessage(IMessage m)
        {
        }
    }

    public class ReportSpy : ReportDummy
    {
        private readonly List<IMessage> _messages = new List<IMessage>();

        public IReadOnlyList<IMessage> Messages => _messages;

        public override void AddMessage(IMessage m)
        {
            _messages.Add(m);
        }
    }
}