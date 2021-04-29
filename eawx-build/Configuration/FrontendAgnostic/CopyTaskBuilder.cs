using System;
using System.IO.Abstractions;
using EawXBuild.Core;
using EawXBuild.Tasks;

namespace EawXBuild.Configuration.FrontendAgnostic
{
    public class CopyTaskBuilder : ITaskBuilder
    {
        private readonly CopyTask _copyTask;

        public CopyTaskBuilder(ICopyPolicy copyPolicy, IFileSystem fileSystem = null)
        {
            _copyTask = new CopyTask(copyPolicy, fileSystem ?? new FileSystem());
        }

        public ITaskBuilder With(string name, object value)
        {
            switch (name)
            {
                case "Id":
                    _copyTask.Id = (string) value;
                    break;
                case "Name":
                    _copyTask.Name = (string) value;
                    break;
                case "CopyFromPath":
                    _copyTask.Source = (string) value;
                    break;
                case "CopyToPath":
                    _copyTask.Destination = (string) value;
                    break;
                case "CopySubfolders":
                    _copyTask.Recursive = (bool) value;
                    break;
                case "CopyFileByPattern":
                    _copyTask.FilePattern = (string) value;
                    break;
                case "AlwaysOverwrite":
                    _copyTask.AlwaysOverwrite = (bool) value;
                    break;
                default:
                    throw new InvalidOperationException($"Invalid configuration option: {name}");
            }

            return this;
        }

        public ITask Build()
        {
            return _copyTask;
        }
    }
}