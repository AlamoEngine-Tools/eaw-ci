using System;
using System.IO;
using System.Text;
using EawXBuild.Core;
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
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        public void GivenJobWithTwoTasks__WhenRunningJob__ShouldLogAllTaskNames() {
            var builder = new StringBuilder();
            Console.SetOut(new StringWriter(builder));

            var sut = new Job("job");
            var firstTask = new TaskSpy {Name = "FirstTask"};
            var secondTask = new TaskSpy {Name = "SecondTask"};
            sut.AddTask(firstTask);
            sut.AddTask(secondTask);

            sut.Run();

            var actual = builder.ToString();
            var expected = "Launching task FirstTask"
                           + Environment.NewLine
                           + "Launching task SecondTask"
                           + Environment.NewLine;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        public void GivenJobWithTwoUnnamedTasks__WhenRunningJob__ShouldLogTasksWithIndex() {
            var builder = new StringBuilder();
            Console.SetOut(new StringWriter(builder));

            var sut = new Job("job");
            var firstTask = new TaskSpy();
            var secondTask = new TaskSpy();
            sut.AddTask(firstTask);
            sut.AddTask(secondTask);

            sut.Run();

            var actual = builder.ToString();
            var expected = "Launching task 0"
                           + Environment.NewLine
                           + "Launching task 1"
                           + Environment.NewLine;

            Assert.AreEqual(expected, actual);
        }

        private static void AssertTaskWasRun(TaskSpy firstTask) {
            Assert.IsTrue(firstTask.WasRun, "Task should have been run, but wasn't.");
        }
    }
}