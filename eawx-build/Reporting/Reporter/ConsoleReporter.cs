#nullable enable
using System;
namespace EawXBuild.Reporting.Reporter
{
    public class ConsoleReporter : IReporter
    {
        public void ReportError(IMessage msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg.MessageContent);
        }

        public void ReportMessage(IMessage msg)
        {
            Console.ResetColor();
            Console.WriteLine(msg.MessageContent);
        }
    }
}