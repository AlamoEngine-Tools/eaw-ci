using System;
using System.Globalization;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using EawXBuild.Core;
using EawXBuildTest.Tasks;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Core {
    [TestClass]
    public class JobTest {
        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        public void GivenJobWithTwoTasks__WhenRunningJob__ShouldExecuteAllTasks() {
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
        public void GivenJobWithTaskThatThrowsException__WhenRunningJob__ShouldPrintExceptionMessageToStdOut() {
            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture);
            Console.SetOut(stringWriter);

            var sut = new Job("job");
            const string exceptionMessage = "exception message";
            sut.AddTask(new ExceptionThrowingTask(exceptionMessage));

            sut.Run();

            const string expectedOutput = "exception message\n";
            Assert.AreEqual(expectedOutput, stringBuilder.ToString());
        }

        [TestMethod]
        public void GivenJobWithTaskThatThrowsException_AndSecondTask__WhenRunningJob__ShouldNotRunSecondTask() {
            var taskSpy = new TaskSpy();

            var sut = new Job("job");
            sut.AddTask(new ExceptionThrowingTask());
            sut.AddTask(taskSpy);

            sut.Run();

            AssertTaskWasNotRun(taskSpy);
        }

        private static void AssertTaskWasRun(TaskSpy firstTask) {
            Assert.IsTrue(firstTask.WasRun, "Task should have been run, but wasn't.");
        }

        private static void AssertTaskWasNotRun(TaskSpy taskSpy) {
            Assert.IsFalse(taskSpy.WasRun, "Should not have run task, but did");
        }
    }
}