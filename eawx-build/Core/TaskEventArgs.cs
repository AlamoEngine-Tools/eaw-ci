using System;

namespace EawXBuild.Core
{
    public class TaskEventArgs : EventArgs
    {
        private bool _cancel;

        public ITask Task { get; }

        public bool Cancel
        {
            get => _cancel;
            set => _cancel |= value;
        }

        public TaskEventArgs(ITask task)
        {
            Task = task;
        }
    }
}