using System;
using System.IO.Abstractions;
using EawXBuild.Core;
using EawXBuild.Tasks;

namespace EawXBuild.Configuration.FrontendAgnostic
{
    public class CleanTaskBuilder : ITaskBuilder
    {
        private readonly CleanTask _cleanTask;

        public CleanTaskBuilder(IFileSystem? fileSystem = null)
        {
            _cleanTask = new CleanTask(fileSystem ?? new FileSystem());
        }

        public ITaskBuilder With(string name, object value)
        {
            switch (name)
            {
                case "Id":
                    _cleanTask.Id = (string) value;
                    break;
                case "Name":
                    _cleanTask.Name = (string) value;
                    break;
                case "Path":
                    _cleanTask.Path = (string) value;
                    break;
                default:
                    throw new InvalidOperationException($"Invalid configuration option: {name}");
            }

            return this;
        }

        public ITask Build()
        {
            return _cleanTask;
        }
    }
}