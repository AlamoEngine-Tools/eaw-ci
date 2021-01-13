using System.Collections.Generic;
using EawXBuild.Core;

namespace EawXBuildTest.Core {
    public class BuildComponentFactoryStub : IBuildComponentFactory {
        public IProject Project { get; set; } = new ProjectDummy();

        public JobDummy Job { get; set; } = new JobDummy();

        public ITaskBuilder TaskBuilder { get; set; } = new TaskBuilderDummy();

        public virtual IProject MakeProject() {
            return Project;
        }

        public virtual IJob MakeJob(string name) {
            Job.Name = name;
            return Job;
        }

        public virtual ITaskBuilder Task(string taskTypeName) {
            return TaskBuilder;
        }
    }

    public class JobIteratingBuildComponentFactoryStub : IBuildComponentFactory {
        private IEnumerator<IJob> _enumerator;

        public IEnumerable<IJob> Jobs { get; set; }

        public IProject Project { get; set; }

        public IJob MakeJob(string name) {
            if (_enumerator == null) _enumerator = Jobs.GetEnumerator();
            _enumerator.MoveNext();
            return _enumerator.Current;
        }

        public IProject MakeProject() {
            return Project;
        }

        public ITaskBuilder Task(string taskTypeName) {
            return new TaskBuilderStub();
        }
    }

    public class ProjectIteratingBuildComponentFactoryStub : IBuildComponentFactory {
        private IEnumerator<IProject> _enumerator;

        public IEnumerable<IProject> Projects { get; set; }

        public JobDummy Job { get; set; } = new JobDummy();

        public ITaskBuilder TaskBuilder { get; set; } = new TaskBuilderDummy();

        public IJob MakeJob(string name) {
            return Job;
        }

        public IProject MakeProject() {
            if (_enumerator == null) _enumerator = Projects.GetEnumerator();
            _enumerator.MoveNext();
            return _enumerator.Current;
        }

        public ITaskBuilder Task(string taskTypeName) {
            return TaskBuilder;
        }
    }

    public class BuildComponentFactorySpy : BuildComponentFactoryStub {
        public string ActualTaskTypeName { get; private set; }

        public override ITaskBuilder Task(string taskTypeName) {
            ActualTaskTypeName = taskTypeName;
            return TaskBuilder;
        }
    }
}