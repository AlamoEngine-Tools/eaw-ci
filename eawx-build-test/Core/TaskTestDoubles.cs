using System;
using System.Threading;
using EawXBuild.Core;

namespace EawXBuildTest.Core {
    public class TaskDummy : ITask {
        public virtual void Run(CancellationToken token) { }

        public Exception Error { get; }
    }

    public class TaskSpy : TaskDummy {
        public bool WasRun { get; private set; }

        public override void Run(CancellationToken token) {
            WasRun = true;
        }
    }
}