using System.Collections.Generic;

namespace EawXBuild.Core
{
    public class Job : IJob
    {
        private readonly string _name;
        private readonly List<ITask> tasks = new List<ITask>();

        public Job(string name)
        {
            _name = name;
        }

        public string GetName()
        {
            return _name;
        }

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