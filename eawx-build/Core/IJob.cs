using System;
using System.Collections.Generic;
using System.Threading;

namespace EawXBuild.Core
{
    public interface IJob : IEnumerable<ITask>
    {
        string Name { get; }

        void AddTask(ITask task);
        
        void Run(CancellationToken token);
    }
}