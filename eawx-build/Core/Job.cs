using System.Collections.Generic;

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

        public void Run()
        {
            foreach (var task in _tasks)
                task.Run();
        }

        public void AddTask(ITask task)
        {
            _tasks.Add(task);
        }
    }
}