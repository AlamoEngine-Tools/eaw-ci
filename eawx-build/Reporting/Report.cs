using System;
using System.Collections.Generic;
using System.Linq;
using EawXBuild.Exceptions;
using EawXBuild.Reporting.Export;

namespace EawXBuild.Reporting
{
    public class Report : IReport
    {
        public event EventHandler<IMessage> MessageAddedEvent;
        public event EventHandler<IErrorMessage> ErrorMessageAddedEvent;
        public event EventHandler<bool> ReportFinalizedEvent;
        
        private DateTime _reportEndTime;
        private TimeSpan _reportDuration = TimeSpan.Zero;
        private readonly List<IMessage> _messages = new List<IMessage>();
        private readonly IReportExportHandler _exportHandler;

        public DateTime ReportStartTime { get; }
        public DateTime ReportEndTime => _reportEndTime;
        public TimeSpan ReportDuration => CalcReportDuration();

        IReadOnlyList<IErrorMessage> IReport.ErrorMessages =>
            (from m in _messages
                where m.GetType().IsAssignableFrom(typeof(IErrorMessage))
                orderby m.CreatedTimestamp
                select m as IErrorMessage).ToList().AsReadOnly();

        IReadOnlyList<IMessage> IReport.Messages =>
            (from m in _messages orderby m.CreatedTimestamp select m).ToList().AsReadOnly();

        public bool IsFinalized { get; private set; }

        IReportExportHandler IReport.ExportHandler => _exportHandler;

        protected internal Report() : this(null)
        {
        }

        protected Report(IReportExportHandler exportHandler)
        {
            _exportHandler = exportHandler ?? new NullReportExportHandler();
            ReportStartTime = DateTime.Now;
        }

        public virtual void FinalizeReport()
        {
            _reportEndTime = DateTime.Now;
            _reportDuration = CalcReportDuration(true);
            IsFinalized = true;
            OnReportFinalized();
        }

        protected virtual void OnReportFinalized()
        {
            ReportFinalizedEvent?.Invoke(this, true);
        }

        public virtual void AddMessage(IMessage m)
        {
            if (IsFinalized)
            {
                throw new ReportAlreadyFinalizedException($"The report has been finalized at {ReportEndTime:O} and cannot be expanded with new messages.");
            }
            if (m == null)
            {
                throw new ArgumentNullException(nameof(m));
            }
            _messages.Add(m);
            OnMessageAdded(m);
        }

        protected virtual void OnMessageAdded(IMessage m)
        {
            if (m.GetType().IsInstanceOfType(typeof(IErrorMessage)))
            {
                ErrorMessageAddedEvent?.Invoke(this, (IErrorMessage) m);
            }
            MessageAddedEvent?.Invoke(this, m);
        }

        private TimeSpan CalcReportDuration(bool isFinalizing = false)
        {
            if (isFinalizing)
            {
                return new TimeSpan(_reportEndTime.Ticks - ReportStartTime.Ticks);
            }

            return IsFinalized ? _reportDuration : new TimeSpan(DateTime.Now.Ticks - ReportStartTime.Ticks);
        }

        public virtual void Export(ExportType exportType = ExportType.Full)
        {
            _exportHandler.CreateExport(this, exportType);
        }
    }
}