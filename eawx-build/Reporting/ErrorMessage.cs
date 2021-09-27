using System;
using System.Diagnostics.CodeAnalysis;

namespace EawXBuild.Reporting
{
    [ExcludeFromCodeCoverage]
    public readonly struct ErrorMessage : IErrorMessage
    {
        public DateTime CreatedTimestamp { get; }
        public string MessageContent { get; }
        public Exception? AssociatedException { get; }

        public ErrorMessage([NotNull] string messageContent, Exception? associatedException = null)
        {
            CreatedTimestamp = DateTime.Now;
            MessageContent = messageContent;
            AssociatedException = associatedException;
        }
    }
}