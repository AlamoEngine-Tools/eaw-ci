using System;

namespace EawXBuild.Reporting
{
    public interface IErrorMessage : IMessage
    {
        Exception? AssociatedException { get; }
    }
}