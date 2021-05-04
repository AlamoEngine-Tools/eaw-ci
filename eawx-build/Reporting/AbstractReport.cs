using System;
using System.Collections.Generic;
using System.Linq;
using EawXBuild.Exceptions;

namespace EawXBuild.Reporting
{
    public abstract class AbstractReport : IReport
    {
        private DateTime _reportEndTime;
        private TimeSpan _reportDuration = TimeSpan.Zero;
        private readonly List<IMessage> _messages = new List<IMessage>();

        public DateTime ReportStartTime { get; }
        public DateTime ReportEndTime => _reportEndTime;
        public TimeSpan ReportDuration => CalcReportDuration();

        public IReadOnlyList<IErrorMessage> ErrorMessages =>
            (from m in _messages
                where m.GetType().IsAssignableFrom(typeof(IErrorMessage))
                orderby m.CreatedTimestamp
                select m as IErrorMessage).ToList().AsReadOnly();

        public IReadOnlyList<IMessage> Messages =>
            (from m in _messages orderby m.CreatedTimestamp select m).ToList().AsReadOnly();

        public bool IsFinalized { get; private set; }

        protected AbstractReport()
        {
            ReportStartTime = DateTime.Now;
        }

        public virtual void FinalizeReport()
        {
            _reportEndTime = DateTime.Now;
            _reportDuration = CalcReportDuration(true);
            IsFinalized = true;
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
        } 

        private TimeSpan CalcReportDuration(bool isFinalizing = false)
        {
            if (isFinalizing)
            {
                return new TimeSpan(_reportEndTime.Ticks - ReportStartTime.Ticks);
            }

            return IsFinalized ? _reportDuration : new TimeSpan(DateTime.Now.Ticks - ReportStartTime.Ticks);
        }
    }
}