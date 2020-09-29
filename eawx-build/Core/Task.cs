using System;
using System.Linq;
using System.Threading;
using EawXBuild.Exceptions;
using Microsoft.Extensions.Logging;

namespace EawXBuild.Core
{
    public abstract class TaskBase : ITask
    {
        private readonly ILogger<TaskBase> _logger = new LoggerFactory().CreateLogger<TaskBase>();

        public Exception Error { get; private set; }

        protected internal ILogger Logger { get; private set; }

        public void Run(CancellationToken token)
        {
            Logger = _logger;
            Logger.LogTrace($"BEGIN: {this}");
            try
            {
                RunCore(token);
                Logger.LogTrace($"END: {this}");
            }
            catch (OperationCanceledException ex)
            {
                Error = ex.InnerException;
                throw;
            }
            catch (StopJobException)
            {
                throw;
            }
            catch (AggregateException ex)
            {
                if (!ex.IsExceptionType<OperationCanceledException>())
                    LogFaultException(ex);
                else
                    Error = ex.InnerExceptions.FirstOrDefault(p => Extensions.IsExceptionType<OperationCanceledException>(p))?.InnerException;
                throw;
            }
            catch (Exception e)
            {
                LogFaultException(e);
                throw;
            }
        }

        public override string ToString()
        {
            return GetType().Name;
        }

        protected abstract void RunCore(CancellationToken token);

        private void LogFaultException(Exception ex)
        {
            Error = ex;
            _logger?.LogError(ex, ex.InnerException?.Message ?? ex.Message);
        }
    }
}