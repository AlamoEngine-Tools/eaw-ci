using System;
using System.Threading;
using EawXBuild.Core;

namespace EawXBuild.Tasks
{
    public abstract class SynchronizedTask : TaskBase
    {
        public event EventHandler<EventArgs> Canceled;

        private readonly ManualResetEvent _handle;

        protected SynchronizedTask()
        {
            _handle = new ManualResetEvent(false);
        }

        public void Wait()
        {
            Wait(Timeout.InfiniteTimeSpan);
        }

        internal void Wait(TimeSpan timeout)
        {
            if (!_handle.WaitOne(timeout))
                throw new TimeoutException();
        }

        protected sealed override void RunCore(CancellationToken token)
        {
            try
            {
                RunSynchronized(token);
            }
            catch (Exception ex)
            {
                if (ex.IsExceptionType<OperationCanceledException>())
                    Canceled?.Invoke(this, new EventArgs());
                throw;
            }
            finally
            {
                _handle.Set();
            }
        }

        protected abstract void RunSynchronized(CancellationToken token);
    }
}