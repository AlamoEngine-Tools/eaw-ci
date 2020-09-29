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

            sut.Run(default);

            AssertTaskWasRun(firstTask);
            AssertTaskWasRun(secondTask);
        }

        private static void AssertTaskWasRun(TaskSpy firstTask) {
            Assert.IsTrue(firstTask.WasRun, "Task should have been run, but wasn't.");
        }
    }
}