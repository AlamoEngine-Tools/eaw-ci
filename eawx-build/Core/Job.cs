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
            try {
                foreach (var task in _tasks)
                    task.Run();
            } catch (Exception ex) {
                Console.Out.WriteLine(ex.Message);
            }
        }

        public void AddTask(ITask task) {
            _tasks.Add(task);
        }
    }
}