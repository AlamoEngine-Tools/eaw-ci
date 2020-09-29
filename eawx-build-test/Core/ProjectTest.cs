using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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

            await _sut.RunJobAsync("job", default);

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

            await _sut.RunJobAsync("job", default);

            AssertJobWasRun(expected);
            AssertJobWasNotRun(otherJob);
        }
        
        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        public void GivenProjectWithMultipleJobs__WhenCallingRunAll__AllJobsRan() {
            var job1 = MakeJobSpy("job1");
            _sut.AddJob(job1);
            var job2 = MakeJobSpy("job2");
            _sut.AddJob(job2);

            Task.WaitAll(_sut.RunAllJobsAsync(default).ToArray());

            AssertJobWasRun(job1);
            AssertJobWasRun(job2);
        }


        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        [ExpectedException(typeof(JobNotFoundException))]
        public void GivenProjectWithNoJobs__WhenCallingRunJob__ShouldThrowJobNotFoundException() {
            _sut.RunJobAsync("job", default);
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