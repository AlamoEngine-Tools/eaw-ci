using System.Threading.Tasks;
using EawXBuild.Core;
using EawXBuild.Core.Exceptions;
using NUnit.Framework;

namespace EawXBuildTest.Core
{
    class ProjectTest
    {

        private Project sut;

        [SetUp]
        public void SetUp()
        {
            sut = new Project();
        }

        [Test]
        public async Task GivenProjectWithNamedJob__WhenCallingRunWithJobName__ShouldRunJob()
        {
            var jobSpy = MakeJobSpy("job");
            sut.AddJob(jobSpy);

            await sut.RunJobAsync("job");

            AssertJobWasRun(jobSpy);
        }

        [Test]
        public async Task GivenProjectWithTwoJobs__WhenCallingRunWithJobName__ShouldOnlyRunWithMatchingName()
        {
            var otherJob = MakeJobSpy("other");
            sut.AddJob(otherJob);
            var expected = MakeJobSpy("job");
            sut.AddJob(expected);

            await sut.RunJobAsync("job");

            AssertJobWasRun(expected);
            AssertJobWasNotRun(otherJob);
        }


        [Test]
        public void GivenProjectWithNoJobs__WhenCallingRunJob__ShouldThrowJobNotFoundException()
        {
            AsyncTestDelegate expectedFail = () => sut.RunJobAsync("job");

            Assert.ThrowsAsync<JobNotFoundException>(expectedFail, "Should have thrown JobNotFoundException, but did not.");
        }

        [Test]
        public void GivenProjectWithJob__WhenAddingJobWithSameName__ShouldThrowDuplicateJobNameException()
        {
            JobSpy jobSpy = MakeJobSpy("job");

            sut.AddJob(jobSpy);

            TestDelegate expectedFail = () => sut.AddJob(MakeJobSpy("job"));
            Assert.Throws<DuplicateJobNameException>(expectedFail, "Should have thrown DuplicateJobNameException, but did not.");
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
            Assert.IsTrue(jobSpy.WasRun, $"Job {jobSpy.Name} should have been run, but wasn't.");
        }
        private static void AssertJobWasNotRun(JobSpy otherJob)
        {
            Assert.False(otherJob.WasRun, $"Should not have run Job {otherJob.Name}, but did.");
        }
    }
}