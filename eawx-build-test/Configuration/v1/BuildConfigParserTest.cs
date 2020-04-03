using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Xml.Schema;
using EawXBuild.Configuration.v1;
using EawXBuild.Core;
using EawXBuildTest.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Configuration.v1
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
        public void
            GivenXmlWithWrongConfigVersion__WhenCallingParse__ShouldThrowException()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                                <eaw-ci:BuildConfiguration
                                  ConfigVersion=""1.0.1"" 
                                  xmlns:eaw-ci=""eaw-ci""
                                  xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
                                  <Projects>
                                    <Project Id=""idvalue0"" Name=""TestProject"">
                                      <Jobs>
                                        <Job Id=""idvalue1"">
                                          <Tasks>
                                            <Task Id=""idvalue2"" xsi:type=""eaw-ci:AbstractTaskType""/>
                                          </Tasks>
                                        </Job>
                                      </Jobs>
                                    </Project>
                                  </Projects>
                                </eaw-ci:BuildConfiguration>";

            _mockFileData.TextContents = xml;
            const string exceptionMessage =
                "The value of the 'ConfigVersion' attribute does not equal its fixed value.";
            var factoryStub = new BuildComponentFactoryStub {Project = new ProjectStub()};
            var sut = new BuildConfigParser(_fileSystem, factoryStub);
            try
            {
                sut.Parse(Path);
            }
            catch (Exception e)
            {
                Assert.AreEqual(typeof(InvalidOperationException), e.GetType());
                Assert.AreEqual(typeof(XmlSchemaValidationException), e.InnerException.GetType());
                Assert.AreEqual(exceptionMessage, e.InnerException.Message);
            }
        }

        [TestMethod]
        public void GivenXmlWithSingleProject__WhenCallingParse__ShouldReturnArrayWithMatchingProject()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                                <eaw-ci:BuildConfiguration
                                  ConfigVersion=""1.0.0"" 
                                  xmlns:eaw-ci=""eaw-ci""
                                  xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
                                  <Projects>
                                    <Project Id=""idvalue0"" Name=""TestProject"">
                                      <Jobs>
                                        <Job Id=""idvalue1"">
                                          <Tasks>
                                          </Tasks>
                                        </Job>
                                      </Jobs>
                                    </Project>
                                  </Projects>
                                </eaw-ci:BuildConfiguration>";

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
                                <eaw-ci:BuildConfiguration
                                  ConfigVersion=""1.0.0"" 
                                  xmlns:eaw-ci=""eaw-ci""
                                  xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
                                  <Projects>
                                    <Project Id=""idvalue0"">
                                      <Jobs>
                                        <Job Id=""idvalue1"" Name=""TestJob"">
                                          <Tasks>
                                          </Tasks>
                                        </Job>
                                      </Jobs>
                                    </Project>
                                  </Projects>
                                </eaw-ci:BuildConfiguration>";

            _mockFileData.TextContents = xml;

            const string jobName = "TestJob";

            var factoryStub = new BuildComponentFactoryStub {Project = new ProjectStub(), Job = new JobStub()};
            var sut = new BuildConfigParser(_fileSystem, factoryStub);

            var projects = sut.Parse(Path);

            var actualProject = projects[0] as ProjectStub;
            var actualJob = actualProject.Jobs[0];
            AssertJobNameEquals(jobName, actualJob);
        }

        [TestMethod]
        public void GivenProjectWithJobAndCopyTask__WhenCallingParse__ShouldRequestTaskBuilderForCorrectType()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                                <eaw-ci:BuildConfiguration
                                  ConfigVersion=""1.0.0"" 
                                  xmlns:eaw-ci=""eaw-ci""
                                  xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
                                  <Projects>
                                    <Project Id=""idvalue0"">
                                      <Jobs>
                                        <Job Id=""idvalue1"" Name=""TestJob"">
                                          <Tasks>
                                            <Task Id=""idvalue2"" Name=""TestTask"" xsi:type=""eaw-ci:Copy"">
                                              <CopyFromPath>path/to/source</CopyFromPath>
                                              <CopyToPath>path/to/dest</CopyToPath>
                                              <CopySubfolders>true</CopySubfolders>
                                              <CopyFileByPattern>*</CopyFileByPattern>
                                            </Task>
                                          </Tasks>
                                        </Job>
                                      </Jobs>
                                    </Project>
                                  </Projects>
                                </eaw-ci:BuildConfiguration>";

            _mockFileData.TextContents = xml;

            var factorySpy = new BuildComponentFactorySpy();
            var sut = new BuildConfigParser(_fileSystem, factorySpy);

            sut.Parse(Path);

            var expected = "Copy";
            Assert.AreEqual(expected, factorySpy.ActualTaskTypeName,
                $"Should have requested TaskBuilder for {expected}, but got {factorySpy.ActualTaskTypeName}");
        }

        [TestMethod]
        public void GivenProjectWithJobAndCopyTask__WhenCallingParse__ShouldConfigureTaskWithBuilder()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                                <eaw-ci:BuildConfiguration
                                  ConfigVersion=""1.0.0"" 
                                  xmlns:eaw-ci=""eaw-ci""
                                  xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
                                  <Projects>
                                    <Project Id=""idvalue0"">
                                      <Jobs>
                                        <Job Id=""idvalue1"" Name=""TestJob"">
                                          <Tasks>
                                            <Task Id=""idvalue2"" Name=""TestTask"" xsi:type=""eaw-ci:Copy"">
                                              <CopyFromPath>path/to/source</CopyFromPath>
                                              <CopyToPath>path/to/dest</CopyToPath>
                                              <CopySubfolders>true</CopySubfolders>
                                              <CopyFileByPattern>*</CopyFileByPattern>
                                            </Task>
                                          </Tasks>
                                        </Job>
                                      </Jobs>
                                    </Project>
                                  </Projects>
                                </eaw-ci:BuildConfiguration>";

            _mockFileData.TextContents = xml;

            var taskBuilderMock = new TaskBuilderMock(new Dictionary<string, object>
            {
                {"Name", "TestTask"},
                {"CopyFromPath", "path/to/source"},
                {"CopyToPath", "path/to/dest"},
                {"CopySubfolders", true}
            });

            var factoryStub = new BuildComponentFactoryStub {TaskBuilder = taskBuilderMock};
            var sut = new BuildConfigParser(_fileSystem, factoryStub);

            sut.Parse(Path);

            taskBuilderMock.Verify();
        }

        [TestMethod]
        public void GivenProjectWithJobAndCopyTask__WhenCallingParse__ShouldAddTaskToJob()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                                <eaw-ci:BuildConfiguration
                                  ConfigVersion=""1.0.0"" 
                                  xmlns:eaw-ci=""eaw-ci""
                                  xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
                                  <Projects>
                                    <Project Id=""idvalue0"">
                                      <Jobs>
                                        <Job Id=""idvalue1"" Name=""TestJob"">
                                          <Tasks>
                                            <Task Id=""idvalue2"" Name=""TestTask"" xsi:type=""eaw-ci:Copy"">
                                              <CopyFromPath>path/to/source</CopyFromPath>
                                              <CopyToPath>path/to/dest</CopyToPath>
                                              <CopySubfolders>true</CopySubfolders>
                                              <CopyFileByPattern>*</CopyFileByPattern>
                                            </Task>
                                          </Tasks>
                                        </Job>
                                      </Jobs>
                                    </Project>
                                  </Projects>
                                </eaw-ci:BuildConfiguration>";

            _mockFileData.TextContents = xml;

            var jobStub = new JobStub();
            var taskBuilderStub = new TaskBuilderStub();
            var factoryStub = new BuildComponentFactoryStub {Job = jobStub, TaskBuilder = taskBuilderStub};
            var sut = new BuildConfigParser(_fileSystem, factoryStub);

            sut.Parse(Path);

            CollectionAssert.Contains(jobStub.Tasks, taskBuilderStub.Task, "Should Job should have expected task, but does not.");
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
