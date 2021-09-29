using System.Threading.Tasks;
using EawXBuild.Core;
using EawXBuild.Exceptions;
using EawXBuildTest.Reporting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static EawXBuildTest.ReportingAssertions;

namespace EawXBuildTest.Core
{
    [TestClass]
    public class ProjectTest
    {
        private Project _sut;
        private ReportSpy _report;

        [TestInitialize]
        public void SetUp()
        {
            _report = new ReportSpy();
            _sut = new Project();
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        public async Task GivenProjectWithNamedJob__WhenCallingRunWithJobName__ShouldRunJob()
        {
            var jobSpy = MakeJobSpy("job");
            _sut.AddJob(jobSpy);

            await _sut.RunJobAsync("job", _report);

            AssertJobWasRun(jobSpy);
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        public async Task GivenProjectWithNamedJob__WhenCallingRunWithJobName__ShouldPassReportToJob()
        {
            var jobSpy = MakeJobSpy("job");
            _sut.AddJob(jobSpy);

            await _sut.RunJobAsync("job", _report);

            AssertReceivedReport(jobSpy);
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        public async Task
            GivenProjectWithTwoJobs__WhenCallingRunWithJobName__ShouldOnlyRunWithMatchingName()
        {
            var otherJob = MakeJobSpy("other");
            _sut.AddJob(otherJob);
            var expected = MakeJobSpy("job");
            _sut.AddJob(expected);

            await _sut.RunJobAsync("job", _report);

            AssertJobWasRun(expected);
            AssertJobWasNotRun(otherJob);
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
        public async Task GivenJobWithReport__WhenRunning__ShouldReportJobStartThenRunJobThenReportEnd()
        {
            const string name = "job";
            var jobSpy = new ReportingJob {Name = name};
            _sut.AddJob(jobSpy);

            await _sut.RunJobAsync(name, _report);

            var messages = _report.Messages;
            AssertMessageContentEquals($"Starting job \"{name}\"", messages[0]);
            AssertMessageContentEquals($"Finished job \"{name}\"", messages[2]);
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        public void GivenProjectWithMultipleJobs__WhenCallingRunAll__AllJobsRan()
        {
            var job1 = MakeJobSpy("job1");
            _sut.AddJob(job1);
            var job2 = MakeJobSpy("job2");
            _sut.AddJob(job2);

            Task.WaitAll(_sut.RunAllJobsAsync().ToArray());

            AssertJobWasRun(job1);
            AssertJobWasRun(job2);
        }


        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        [ExpectedException(typeof(JobNotFoundException))]
        public async Task GivenProjectWithNoJobs__WhenCallingRunJob__ShouldThrowJobNotFoundException()
        {
            await _sut.RunJobAsync("job", _report);
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        [ExpectedException(typeof(DuplicateJobNameException))]
        public void GivenProjectWithJob__WhenAddingJobWithSameName__ShouldThrowDuplicateJobNameException()
        {
            var jobSpy = MakeJobSpy("job");
            _sut.AddJob(jobSpy);

            _sut.AddJob(MakeJobSpy("job"));
        }

        private static JobSpy MakeJobSpy(string name)
        {
            var jobSpy = new JobSpy {Name = name};

            return jobSpy;
        }

        private static void AssertJobWasRun(JobSpy jobSpy)
        {
            Assert.IsNotNull(jobSpy != null, nameof(jobSpy) + " != null");
            Assert.IsTrue(jobSpy.WasRun, $"Job {jobSpy.Name} should have been run, but wasn't.");
        }

        private static void AssertJobWasNotRun(JobSpy otherJob)
        {
            Assert.IsNotNull(otherJob != null, nameof(otherJob) + " != null");
            Assert.IsFalse(otherJob.WasRun, $"Should not have run Job {otherJob.Name}, but did.");
        }

        private void AssertReceivedReport(JobSpy jobSpy)
        {
            Assert.AreEqual(_report, jobSpy.Report);
        }
    }
}