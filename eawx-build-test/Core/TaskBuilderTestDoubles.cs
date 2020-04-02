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
    
    public class TaskBuilderMock : TaskBuilderStub
    {
        private readonly Dictionary<string, object> _expectedEntries;
        private readonly Dictionary<string, object> _actualEntries = new Dictionary<string, object>();

        public TaskBuilderMock(Dictionary<string, object> expectedEntries)
        {
            _expectedEntries = expectedEntries;
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