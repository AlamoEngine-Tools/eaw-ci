using System;
using System.IO.Abstractions;
using EawXBuild.Core;
using EawXBuild.Tasks;

namespace EawXBuild.Configuration.v1 {
    public class CleanTaskBuilder : ITaskBuilder {
        private readonly CleanTask _cleanTask = new CleanTask(new FileSystem());

        public ITaskBuilder With(string name, object value) {
            if (!name.Equals("Path")) throw new InvalidOperationException($"Invalid configuration option: {name}");
            _cleanTask.Path = (string) value;
            return this;
        }

        public ITask Build() {
            return _cleanTask;
        }
    }
}