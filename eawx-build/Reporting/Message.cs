using System;
using System.Diagnostics.CodeAnalysis;

namespace EawXBuild.Reporting
{
    [ExcludeFromCodeCoverage]
    public readonly struct Message : IMessage
    {
        public DateTime CreatedTimestamp { get; }
        public string MessageContent { get; }

        public Message([NotNull] string messageContent)
        {
            CreatedTimestamp = DateTime.Now;
            MessageContent = messageContent;
        }
    }
}