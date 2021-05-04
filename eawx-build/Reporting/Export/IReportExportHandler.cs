namespace EawXBuild.Reporting.Export
{
    public interface IReportExportHandler
    {
        void CreateExport(IReport report, ExportType exportType = ExportType.Full);
    }
}