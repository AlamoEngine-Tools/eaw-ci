namespace EawXBuild.Reporting.Export
{
    public class NullReportExportHandler : IReportExportHandler
    {
        public void CreateExport(IReport report, ExportType exportType = ExportType.Full)
        {
            // NOP
        }
    }
}