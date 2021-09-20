using System.Collections.Generic;
using EawXBuild.Reporting;

namespace EawXBuild.Core
{
    public class Job : IJob
    {
        private readonly List<ITask> _tasks = new List<ITask>();

        public Job(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public void Run(Report report = null)
        {
            foreach (var task in _tasks)
            {
                Report(report, $"Starting task \"{task.Name}\"");
                task.Run(report);
                Report(report, $"Finished task \"{task.Name}\"");
            }
        }
        private static void Report(Report report, string messageContent)
        {
            report?.AddMessage(new Message(messageContent));
        }

        public void AddTask(ITask task)
        {
            _tasks.Add(task);
        }
    }
}