using System;
using System.Collections.Generic;
using EawXBuild.Exceptions;
using EawXBuild.Reporting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Reporting
{
    [TestClass]
    public class ReportTest
    {
        private class TestReport : AbstractReport
        {
        }

        [TestMethod]
        public void GivenOpenReport__AddMessage__IsPossible()
        {
            List<IMessage> messages = CreateTestMessageList();
            TestReport report = CreateTestReportWithMessages(messages);
            foreach (IMessage message in messages)
            {
                report.AddMessage(message);
            }
            Assert.IsTrue(messages.Count < report.Messages.Count, "The report should always contain more messages than the base creation.");
        }

        [TestMethod]
        [ExpectedException(typeof(ReportAlreadyFinalizedException))]
        public void GivenFinalizedReport__AddMessage__ThrowsException()
        {
            List<IMessage> messages = CreateTestMessageList();
            TestReport report = CreateTestReportWithMessages(messages);
            Assert.AreEqual(messages.Count, report.Messages.Count);
            report.FinalizeReport();
            Assert.IsTrue(report.IsFinalized);
            report.AddMessage(new Message("Message"));
        }

        private static List<IMessage> CreateTestMessageList()
        {
            List<IMessage> messages = new List<IMessage>
            {
                new Message("Test message 1"), new ErrorMessage("Test message 2"), new Message("Test message 3"),
                new ErrorMessage("Test message 4", new ArgumentException("that was a wrong argument!"))
            };
            return messages;
        }

        private static TestReport CreateTestReportWithMessages(IEnumerable<IMessage> messages)
        {
            TestReport report = new TestReport();
            foreach (IMessage message in messages)
            {
                report.AddMessage(message);
            }

            return report;
        }
    }
}