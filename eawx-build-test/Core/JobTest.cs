using EawXBuild.Core;
using EawXBuildTest.Reporting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static EawXBuildTest.ReportingAssertions;

namespace EawXBuildTest.Core
{
    [TestClass]
    public class JobTest
    {
        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        public void GivenJobWithTwoTasks__WhenRunningJob__ShouldExecuteAllTasks()
        {
            var sut = new Job("job");
            var firstTask = new TaskSpy();
            var secondTask = new TaskSpy();
            sut.AddTask(firstTask);
            sut.AddTask(secondTask);

            sut.Run();

            AssertTaskWasRun(firstTask);
            AssertTaskWasRun(secondTask);
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        public void GivenJobWithReport__WhenRunning__ShouldPassReportToTasks()
        {
            var sut = new Job("job");
            var task = new TaskSpy();
            sut.AddTask(task);

            var reportDummy = new ReportDummy();

            sut.Run(reportDummy);

            Assert.AreSame(reportDummy, task.Report);
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        public void GivenJobWithTwoTasksAndReport__WhenRunning__ShouldReportTaskStartThenRunTaskThenReportTaskEnd()
        {
            var sut = new Job("job");
            var firstTask = new ReportingTask {Name = "first"};
            var secondTask = new ReportingTask {Name = "second"};
            sut.AddTask(firstTask);
            sut.AddTask(secondTask);

            var reportSpy = new ReportSpy();

            sut.Run(reportSpy);

            var messages = reportSpy.Messages;
            AssertMessageContentEquals("Starting task \"first\"", messages[0]);
            AssertMessageContentEquals("Finished task \"first\"", messages[2]);
            AssertMessageContentEquals("Starting task \"second\"", messages[3]);
            AssertMessageContentEquals("Finished task \"second\"", messages[5]);
        }

        private static void AssertTaskWasRun(TaskSpy firstTask)
        {
            Assert.IsTrue(firstTask.WasRun, "Task should have been run, but wasn't.");
        }


    }
}