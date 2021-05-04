using System;
using System.Collections.Generic;

namespace EawXBuild.Reporting
{
    public interface IReport
    {
        DateTime ReportStartTime { get; }
        DateTime ReportEndTime { get; }
        TimeSpan ReportDuration { get; }
        IReadOnlyList<IErrorMessage> ErrorMessages { get; }
        IReadOnlyList<IMessage> Messages { get; }
        bool IsFinalized { get; }
    }
}