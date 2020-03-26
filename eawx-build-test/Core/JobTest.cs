using EawXBuild.Core;
using NUnit.Framework;

namespace EawXBuildTest.Core
{
    class JobTest
    {
        [Test]
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

        private static void AssertTaskWasRun(TaskSpy firstTask)
        {
            Assert.True(firstTask.WasRun, "Task should have been run, but wasn't.");
        }
    }
}