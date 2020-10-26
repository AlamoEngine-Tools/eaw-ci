using System;
using System.Collections.Generic;

namespace EawXBuild.Core {
    public class Job : IJob {
        private readonly List<ITask> _tasks = new List<ITask>();

        public Job(string name) {
            Name = name;
        }

        public string Name { get; }

        public void Run() {
            for (var index = 0; index < _tasks.Count; index++) {
                var task = _tasks[index];
                Console.WriteLine($"Launching task {task.Name ?? index.ToString()}");
                task.Run();
            }
        }

        public void AddTask(ITask task) {
            _tasks.Add(task);
        }
    }
}