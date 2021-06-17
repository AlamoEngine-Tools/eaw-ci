using System;

namespace EawXBuild.Reporting
{
    public interface IMessage
    {
        DateTime CreatedTimestamp { get; }
        string MessageContent { get; }
    }
}