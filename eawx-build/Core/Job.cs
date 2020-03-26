using System.Collections.Generic;

namespace EawXBuild.Core
{
    public class Job : IJob
    {
        private readonly List<ITask> tasks = new List<ITask>();

        public Job(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public void Run()
        {
            foreach (var task in tasks)
                task.Run();
        }

        public void AddTask(ITask task)
        {
            tasks.Add(task);
        }
    }
}