using System;
using System.Globalization;
using System.IO;
using System.Text;
using EawXBuild.Core;
using EawXBuild.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Core {
    [TestClass]
    public class ProjectTest {
        private Project _sut;

        [TestInitialize]
        public void SetUp() {
            _sut = new Project();
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        public async System.Threading.Tasks.Task GivenProjectWithNamedJob__WhenCallingRunWithJobName__ShouldRunJob() {
            var jobSpy = MakeJobSpy("job");
            _sut.AddJob(jobSpy);

            await _sut.RunJobAsync("job");

            AssertJobWasRun(jobSpy);
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        public async System.Threading.Tasks.Task
            GivenProjectWithTwoJobs__WhenCallingRunWithJobName__ShouldOnlyRunWithMatchingName() {
            var otherJob = MakeJobSpy("other");
            _sut.AddJob(otherJob);
            var expected = MakeJobSpy("job");
            _sut.AddJob(expected);

            await _sut.RunJobAsync("job");

            AssertJobWasRun(expected);
            AssertJobWasNotRun(otherJob);
        }


        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        [ExpectedException(typeof(JobNotFoundException))]
        public void GivenProjectWithNoJobs__WhenCallingRunJob__ShouldThrowJobNotFoundException() {
            _sut.RunJobAsync("job");
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        [ExpectedException(typeof(DuplicateJobNameException))]
        public void GivenProjectWithJob__WhenAddingJobWithSameName__ShouldThrowDuplicateJobNameException() {
            var jobSpy = MakeJobSpy("job");

            Assert.IsNotNull(_sut != null, nameof(_sut) + " != null");
            _sut.AddJob(jobSpy);
            _sut.AddJob(MakeJobSpy("job"));
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        public async System.Threading.Tasks.Task GivenProjectWithJobThatThrowsException__WhenRunningJob__ShouldPrintException() {
            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture);
            Console.SetOut(stringWriter);


            const string exceptionMessage = "exception message";
            _sut.AddJob(new ExceptionThrowingJob(exceptionMessage) {Name = "job"});
            await _sut.RunJobAsync("job");

            const string expectedOutput = "exception message\n";
            Assert.AreEqual(expectedOutput, stringBuilder.ToString());
        }

        private static JobSpy MakeJobSpy(string name) {
            var jobSpy = new JobSpy {Name = name};

            return jobSpy;
        }

        private static void AssertJobWasRun(JobSpy jobSpy) {
            Assert.IsNotNull(jobSpy != null, nameof(jobSpy) + " != null");
            Assert.IsTrue(jobSpy.WasRun, $"Job {jobSpy.Name} should have been run, but wasn't.");
        }

        private static void AssertJobWasNotRun(JobSpy otherJob) {
            Assert.IsNotNull(otherJob != null, nameof(otherJob) + " != null");
            Assert.IsFalse(otherJob.WasRun, $"Should not have run Job {otherJob.Name}, but did.");
        }
    }
}