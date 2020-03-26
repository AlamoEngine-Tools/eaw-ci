using EawXBuild.Core.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Core
{
    [TestClass]
    public class ProjectTest
    {

        private EawXBuild.Core.Project _sut;

        [TestInitialize]
        public void SetUp()
        {
            _sut = new EawXBuild.Core.Project();
        }

        [TestMethod]
        public async System.Threading.Tasks.Task GivenProjectWithNamedJob__WhenCallingRunWithJobName__ShouldRunJob()
        {
            JobSpy jobSpy = MakeJobSpy("job");
            _sut.AddJob(jobSpy);

            await _sut.RunJobAsync("job");

            AssertJobWasRun(jobSpy);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task GivenProjectWithTwoJobs__WhenCallingRunWithJobName__ShouldOnlyRunWithMatchingName()
        {
            JobSpy otherJob = MakeJobSpy("other");
            _sut.AddJob(otherJob);
            JobSpy expected = MakeJobSpy("job");
            _sut.AddJob(expected);

            await _sut.RunJobAsync("job");

            AssertJobWasRun(expected);
            AssertJobWasNotRun(otherJob);
        }


        [TestMethod]
        [ExpectedException(typeof(JobNotFoundException))]
        public void GivenProjectWithNoJobs__WhenCallingRunJob__ShouldThrowJobNotFoundException()
        {
            _sut.RunJobAsync("job");
        }

        [TestMethod]
        [ExpectedException(typeof(DuplicateJobNameException))]
        public void GivenProjectWithJob__WhenAddingJobWithSameName__ShouldThrowDuplicateJobNameException()
        {
            JobSpy jobSpy = MakeJobSpy("job");

            Assert.IsNotNull(_sut != null, nameof(_sut) + " != null");
            _sut.AddJob(jobSpy);
            _sut.AddJob(MakeJobSpy("job"));
        }

        private static JobSpy MakeJobSpy(string name)
        {
            JobSpy jobSpy = new JobSpy
            {
                Name = name
            };

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
    }
}
