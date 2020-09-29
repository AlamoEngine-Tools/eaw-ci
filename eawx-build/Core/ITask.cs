using System;
using System.Threading;

namespace EawXBuild.Core
{
    public interface ITask
    {
        Exception Error { get; }

        void Run(CancellationToken token);
    }
}