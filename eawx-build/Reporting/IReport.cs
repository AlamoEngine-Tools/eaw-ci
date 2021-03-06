using System;
using System.Collections.Generic;
using EawXBuild.Reporting.Export;
namespace EawXBuild.Reporting
{
    
    public interface IReport
    {
        public DateTime ReportStartTime { get; }
        public DateTime ReportEndTime { get; }
        public TimeSpan ReportDuration { get; }
        public bool IsFinalized { get; }
        public void Export(ExportType exportType = ExportType.Full);
        
        internal IReportExportHandler ExportHandler { get; }
        internal IReadOnlyList<IErrorMessage> ErrorMessages { get; }
        internal IReadOnlyList<IMessage> Messages { get; }
    }
}