using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using EawXBuild.Core;

namespace EawXBuildTest.Core {
    public class JobDummy : IJob {
        public virtual string Name { get; set; }

        public virtual void AddTask(ITask task) {
        }

        public virtual void Run(CancellationToken token) {
        }

        public IEnumerator<ITask> GetEnumerator()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class JobStub : JobDummy {
        public List<ITask> Tasks { get; } = new List<ITask>();

        public override string Name { get; set; }

        public override void AddTask(ITask task) {
            Tasks.Add(task);
        }
    }

    public class JobSpy : JobStub {
        public bool WasRun { get; private set; }

        public override void Run(CancellationToken token) {
            WasRun = true;
        }
    }

    public class ExceptionThrowingJob : JobDummy {
        private readonly string _exceptionMessage;

        public ExceptionThrowingJob(string exceptionMessage) {
            _exceptionMessage = exceptionMessage;
        }

        public override void Run(CancellationToken token) {
            throw new Exception(_exceptionMessage);
        }
    }
}