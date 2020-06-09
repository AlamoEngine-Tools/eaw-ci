using System.Collections.Generic;
using EawXBuild.Core;

namespace EawXBuildTest.Core
{
    public class BuildComponentFactoryStub : IBuildComponentFactory
    {
        public IProject Project { get; set; } = new ProjectDummy();

        public JobDummy Job { get; set; } = new JobDummy();

        public ITaskBuilder TaskBuilder { get; set; } = new TaskBuilderDummy();

        public virtual IProject MakeProject()
        {
            return Project;
        }

        public virtual IJob MakeJob(string name)
        {
            Job.Name = name;
            return Job;
        }

        public virtual ITaskBuilder Task(string taskTypeName)
        {
            return TaskBuilder;
        }
    }

    public class JobIteratingBuildComponentFactoryStub : IBuildComponentFactory
    {
        private IEnumerator<IJob> _enumerator;

        public IEnumerable<IJob> Jobs { get; set; }

        public IProject Project { get; set; }

        public IJob MakeJob(string name)
        {
            if (_enumerator == null) _enumerator = Jobs.GetEnumerator();
            _enumerator.MoveNext();
            return _enumerator.Current;
        }

        public IProject MakeProject()
        {
            return Project;
        }

        public ITaskBuilder Task(string taskTypeName)
        {
            return new TaskBuilderStub();
        }
    }

    public class TaskBuilderIteratingComponentFactoryStub : IBuildComponentFactory
    {
        private IEnumerator<ITaskBuilder> _enumerator;

        public IProject Project { get; set; } = new ProjectDummy();

        public JobDummy Job { get; set; } = new JobDummy();

        public List<ITaskBuilder> TaskBuilders { get; } = new List<ITaskBuilder>();

        public virtual IProject MakeProject()
        {
            return Project;
        }

        public virtual IJob MakeJob(string name)
        {
            Job.Name = name;
            return Job;
        }

        public virtual ITaskBuilder Task(string taskTypeName)
        {
            if (_enumerator == null) _enumerator = TaskBuilders.GetEnumerator();
            _enumerator.MoveNext();
            return _enumerator.Current;
        }
    }

    public class BuildComponentFactorySpy : BuildComponentFactoryStub
    {
        public string ActualTaskTypeName { get; private set; }

        public override ITaskBuilder Task(string taskTypeName)
        {
            ActualTaskTypeName = taskTypeName;
            return TaskBuilder;
        }
    }
}