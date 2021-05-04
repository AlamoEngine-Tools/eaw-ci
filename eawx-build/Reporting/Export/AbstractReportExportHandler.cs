using System;

namespace EawXBuild.Reporting.Export
{
    public abstract class AbstractReportExportHandler : IReportExportHandler
    {
        public void CreateExport(IReport report, ExportType exportType = ExportType.Full)
        {
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report));
            }

            if (!report.IsFinalized)
            {
                throw new ArgumentException(
                    $"The provided {nameof(IReport)} {nameof(report)} is not finalized and cannot be exported.");
            }

            switch (exportType)
            {
                case ExportType.Full:
                    CreateFullExport(report);
                    break;
                case ExportType.MessagesOnly:
                    CreateMessageExport(report);
                    break;
                case ExportType.ErrorsOnly:
                    CreateErrorExport(report);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(exportType), exportType, null);
            }
        }

        protected abstract void CreateErrorExport(IReport report);

        protected abstract void CreateMessageExport(IReport report);

        protected abstract void CreateFullExport(IReport report);
    }
}