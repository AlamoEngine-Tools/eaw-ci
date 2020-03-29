using System.IO.Abstractions.TestingHelpers;
using EawXBuild.Configuration.v1;
using EawXBuild.Core;
using EawXBuildTest.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Configuration
{
    [TestClass]
    public class BuildConfigParserTest
    {
        private const string Path = "MyXml.xml";

        private MockFileSystem _fileSystem;
        private MockFileData _mockFileData;

        [TestInitialize]
        public void SetUp()
        {
            _mockFileData = new MockFileData(string.Empty);
            _fileSystem = new MockFileSystem();
            _fileSystem.AddFile(Path, _mockFileData);
        }

        [TestMethod]
        public void GivenXmlWithSingleProject__WhenCallingParse__ShouldReturnArrayWithMatchingProject()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <BuildConfiguration Version=""1.0"" xmlns=""eaw-ci"">
                <Projects>
                    <Project Name=""TestProject"">
                    </Project>
                </Projects>
            </BuildConfiguration>
            ";
            
            _mockFileData.TextContents = xml;

            const string projectName = "TestProject";

            var factoryStub = new BuildComponentFactoryStub {Project = new ProjectStub()};
            var sut = new BuildConfigParser(_fileSystem, factoryStub);

            var projects = sut.Parse(Path);

            var actual = projects[0] as ProjectStub;
            var actualName = actual?.Name;
            AssertProjectNameEquals(projectName, actualName);
        }

        [TestMethod]
        public void GivenXmlWithSingleProjectAndJob__WhenCallingParse__ProjectShouldHaveMatchingJob()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <BuildConfiguration Version=""1.0"" xmlns=""eaw-ci"">
                <Projects>
                    <Project Name=""TestProject"">
                        <Jobs>
                            <Job Name=""TestJob"">
                            </Job>
                        </Jobs>
                    </Project>
                </Projects>
            </BuildConfiguration>
            ";

            _mockFileData.TextContents = xml;

            const string jobName = "TestJob";

            var factoryStub = new BuildComponentFactoryStub {Project = new ProjectStub(), Job = new JobStub()};
            var sut = new BuildConfigParser(_fileSystem, factoryStub);

            var projects = sut.Parse(Path);

            var actualProject = projects[0] as ProjectStub;
            var actualJob = actualProject.Jobs[0];
            AssertJobNameEquals(jobName, actualJob);
        }

        private static void AssertProjectNameEquals(string projectName, string actualName)
        {
            Assert.AreEqual(projectName, actualName,
                $"Should return Project with name {projectName}, but was {actualName}");
        }

        private static void AssertJobNameEquals(string jobName, IJob actualJob)
        {
            Assert.AreEqual(jobName, actualJob.Name, $"Job name should be {jobName}, but was {actualJob.Name}");
        }
    }
}