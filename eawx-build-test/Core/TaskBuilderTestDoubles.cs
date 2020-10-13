using System.Collections.Generic;
using EawXBuild.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Core
{
    public class TaskBuilderDummy : ITaskBuilder
    {
        public virtual ITaskBuilder With(string name, object value)
        {
            return this;
        }

        public virtual ITask Build()
        {
            return null;
        }
    }

    public class TaskBuilderStub : TaskBuilderDummy
    {
        public ITask Task { get; set; } = new TaskDummy();

        public override ITask Build()
        {
            return Task;
        }
    }

    public class IteratingTaskBuilderStub : TaskBuilderDummy
    {
        private IEnumerator<ITask> _enumerator;
        private List<ITask> _tasks = new List<ITask>();

        public List<ITask> Tasks
        {
            get => _tasks;
            set
            {
                _tasks = value;
                _enumerator = _tasks.GetEnumerator();
            }
        }

        public override ITask Build()
        {
            if(_enumerator == null)
                _enumerator = Tasks.GetEnumerator();

            _enumerator.MoveNext();
            return _enumerator.Current;
        }
    }

    public class TaskBuilderMock : TaskBuilderStub
    {
        private readonly Dictionary<string, object> _expectedEntries;
        private readonly Dictionary<string, object> _actualEntries = new Dictionary<string, object>();

        public TaskBuilderMock(Dictionary<string, object> expectedEntries)
        {
            _expectedEntries = expectedEntries;
        }

        public void AddExpectedEntry(string key, object value) {
            _expectedEntries.Add(key, value);
        }

        public override ITaskBuilder With(string name, object value)
        {
            _actualEntries.Add(name, value);
            return this;
        }

        public override ITask Build()
        {
            return new TaskDummy();
        }

        public void Verify()
        {
            CollectionAssert.AreEquivalent(_expectedEntries, _actualEntries,
                "Actual TaskBuilder configuration entries do not match expected ones");
        }
    }
}