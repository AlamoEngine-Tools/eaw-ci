using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using EawXBuild.Exceptions;
using Microsoft.Extensions.Logging;

namespace EawXBuild.Core 
{
    public class Job : IJob
    {
        private readonly List<ITask> _tasks = new List<ITask>();
        protected readonly ILogger<Job> Logger = new LoggerFactory().CreateLogger<Job>();

        public event EventHandler<TaskEventArgs> Error;

        protected ConcurrentQueue<ITask> TaskQueue { get; }

        public bool IsCancelled { get; private set; }

        internal IList<ITask> Tasks => _tasks;

        public Job(string name) 
        {
            TaskQueue = new ConcurrentQueue<ITask>();
            Name = name;
        }

        public string Name { get; }

        public void Run(CancellationToken token) 
        {
            RunCore(token);
        }

        public void AddTask(ITask task) 
        {
            if (task is null)
                throw new ArgumentNullException(nameof(task));
            TaskQueue.Enqueue(task);
        }

        public IEnumerator<ITask> GetEnumerator()
        {
            return TaskQueue.GetEnumerator();
        }

        protected virtual void RunCore(CancellationToken token)
        {
            var alreadyCancelled = false;
            _tasks.AddRange(TaskQueue);
            while (TaskQueue.TryDequeue(out var task))
            {
                try
                {
                    ThrowIfCancelled(token);
                    task.Run(token);
                }
                catch (StopJobException)
                {
                    Logger.LogTrace("Stop subsequent tasks");
                    break;
                }
                catch (Exception e)
                {
                    if (!alreadyCancelled)
                    {
                        if (e.IsExceptionType<OperationCanceledException>())
                            Logger.LogTrace($"Task {task} cancelled");
                        else
                            Logger.LogError(e, $"Task {task} threw an exception: {e.GetType()}: {e.Message}");
                    }

                    var error = new TaskEventArgs(task)
                    {
                        Cancel = token.IsCancellationRequested || IsCancelled ||
                                 e.IsExceptionType<OperationCanceledException>()
                    };
                    if (error.Cancel)
                        alreadyCancelled = true;
                    OnError(error);
                }
            }
        }

        protected virtual void OnError(TaskEventArgs e)
        {
            Error?.Invoke(this, e);
            if (!e.Cancel)
                return;
            IsCancelled |= e.Cancel;
        }

        protected void ThrowIfCancelled(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            if (IsCancelled)
                throw new OperationCanceledException(token);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return TaskQueue.GetEnumerator();
        }
    }
}