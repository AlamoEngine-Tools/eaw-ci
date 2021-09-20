namespace EawXBuild.Reporting.Export
{
    /// <summary>
    /// Default <see cref="IReportExportHandler"/> that is used by an <see cref="Report"/> if no explicit
    /// <see cref="IReportExportHandler"/> is provided.
    /// </summary>
    public sealed class NullReportExportHandler : AbstractReportExportHandler
    {
        protected override void CreateErrorExport(IReport report)
        {
            // NOP
        }

        protected override void CreateMessageExport(IReport report)
        {
            // NOP
        }

        protected override void CreateFullExport(IReport report)
        {
            // NOP
        }
    }
}