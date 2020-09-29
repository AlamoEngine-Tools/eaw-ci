using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace EawXBuild.Core
{
    public class AsyncJob : Job
    {
        private readonly ConcurrentBag<Exception> _exceptions;
        private readonly System.Threading.Tasks.Task[] _tasks;
        private CancellationToken _cancel;

        public int WorkerCount { get; }

        public AggregateException Exception => _exceptions.Count > 0 ? new AggregateException(_exceptions) : null;

        public AsyncJob(string name, int workerCount) : base(name)
        {
            if (workerCount < 1)
                throw new ArgumentOutOfRangeException(nameof(workerCount));
            WorkerCount = workerCount;
            _exceptions = new ConcurrentBag<Exception>();
            _tasks = new System.Threading.Tasks.Task[workerCount];
        }

        public void Wait()
        {
            Wait(Timeout.InfiniteTimeSpan);
            var exception = Exception;
            if (exception != null)
                throw exception;
        }
        
        internal void Wait(TimeSpan timeout)
        {
            System.Threading.Tasks.Task.WaitAll(_tasks, timeout);
        }

        protected override void RunCore(CancellationToken token)
        {
            ThrowIfCancelled(token);
            Tasks.AddRange(TaskQueue);
            _cancel = token;
            for (var index = 0; index < WorkerCount; ++index)
                _tasks[index] = System.Threading.Tasks.Task.Run(InvokeThreaded);
        }

        private void InvokeThreaded()
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancel);
            var canceled = false;
            while (TaskQueue.TryDequeue(out var task))
            {
                try
                {
                    ThrowIfCancelled(_cancel);
                    task.Run(_cancel);
                }
                catch (Exception ex)
                {
                    _exceptions.Add(ex);
                    if (!canceled)
                    {
                        if (ex.IsExceptionType<OperationCanceledException>())
                            Logger.LogTrace($"Activity threw exception {ex.GetType()}: {ex.Message}" + System.Environment.NewLine + $"{ex.StackTrace}");
                        else
                            Logger.LogError(ex, $"Activity threw exception {ex.GetType()}: {ex.Message}");
                    }
                    var e = new TaskEventArgs(task)
                    {
                        Cancel = _cancel.IsCancellationRequested || IsCancelled || ex.IsExceptionType<OperationCanceledException>()
                    };
                    OnError(e);
                    if (e.Cancel)
                    {
                        canceled = true;
                        linkedTokenSource.Cancel();
                    }
                }
            }
        }
    }
}