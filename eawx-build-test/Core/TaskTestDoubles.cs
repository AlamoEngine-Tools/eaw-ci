using System;
using EawXBuild.Core;

namespace EawXBuildTest.Core
{
    public class TaskDummy : ITask
    {
        public virtual void Run()
        {
        }
    }

    public class TaskSpy : TaskDummy
    {
        public bool WasRun { get; private set; }

        public override void Run()
        {
            WasRun = true;
        }
    }
    
    public class ExceptionThrowingTask : ITask {
        private readonly string _message;

        public ExceptionThrowingTask(string message = null) {
            _message = message;
        }
        
        public void Run() {
            throw new Exception(_message);
        }
    }
}