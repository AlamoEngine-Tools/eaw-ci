using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Xml.Schema;
using EawXBuild.Configuration.Xml.v1;
using EawXBuild.Core;
using EawXBuildTest.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Configuration.Xml.v1 {
    [TestClass]
    public class BuildConfigParserTest {
        private const string Path = "MyXml.xml";

        private MockFileSystem _fileSystem;
        private MockFileData _mockFileData;

        [TestInitialize]
        public void SetUp() {
            _mockFileData = new MockFileData(string.Empty);
            _fileSystem = new MockFileSystem();
            _fileSystem.AddFile(Path, _mockFileData);
        }

        [TestMethod]
        public void
            GivenXmlWithWrongConfigVersion__WhenCallingParse__ShouldThrowException() {
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
            var sut = new XmlBuildConfigParser(_fileSystem, factoryStub);
            try {
                sut.Parse(Path);
            }
            catch (Exception e) {
                Assert.AreEqual(typeof(InvalidOperationException), e.GetType());
                Assert.AreEqual(typeof(XmlSchemaValidationException), e.InnerException.GetType());
                Assert.AreEqual(exceptionMessage, e.InnerException.Message);
            }
        }

        [TestMethod]
        public void GivenXmlWithSingleProject__WhenCallingParse__ShouldReturnArrayWithMatchingProject() {
            const string xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                                <eaw-ci:BuildConfiguration
                                  ConfigVersion=""1.0.0"" 
                                  xmlns:eaw-ci=""eaw-ci""
                                  xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
                                  <Projects>
                                    <Project Id=""TestProject"">
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
            var sut = new XmlBuildConfigParser(_fileSystem, factoryStub);

            var projects = sut.Parse(Path);

            var actual = projects.ToList()[0] as ProjectStub;
            var actualName = actual?.Name;
            AssertProjectNameEquals(projectName, actualName);
        }

        [TestMethod]
        public void GivenXmlWithSingleProjectAndJob__WhenCallingParse__ProjectShouldHaveMatchingJob() {
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
            var sut = new XmlBuildConfigParser(_fileSystem, factoryStub);

            var projects = sut.Parse(Path);

            var actualProject = (ProjectStub) projects.ToList()[0];
            var actualJob = actualProject.Jobs[0];
            AssertJobNameEquals(jobName, actualJob);
        }

        [TestMethod]
        public void GivenProjectWithJobAndCopyTask__WhenCallingParse__ShouldRequestTaskBuilderForCorrectType() {
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
                                              <AlwaysOverwrite>true</AlwaysOverwrite>
                                            </Task>
                                          </Tasks>
                                        </Job>
                                      </Jobs>
                                    </Project>
                                  </Projects>
                                </eaw-ci:BuildConfiguration>";

            _mockFileData.TextContents = xml;

            var factorySpy = new BuildComponentFactorySpy();
            var sut = new XmlBuildConfigParser(_fileSystem, factorySpy);

            sut.Parse(Path);

            var expected = "Copy";
            Assert.AreEqual(expected, factorySpy.ActualTaskTypeName,
                $"Should have requested TaskBuilder for {expected}, but got {factorySpy.ActualTaskTypeName}");
        }

        [TestMethod]
        public void GivenProjectWithJobAndCopyTask__WhenCallingParse__ShouldConfigureTaskWithBuilder() {
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
                                            <Task Id=""TestTask"" Name=""TestTask"" xsi:type=""eaw-ci:Copy"">
                                              <CopyFromPath>path/to/source</CopyFromPath>
                                              <CopyToPath>path/to/dest</CopyToPath>
                                              <CopySubfolders>true</CopySubfolders>
                                              <CopyFileByPattern>*</CopyFileByPattern>
                                              <AlwaysOverwrite>false</AlwaysOverwrite>
                                            </Task>
                                          </Tasks>
                                        </Job>
                                      </Jobs>
                                    </Project>
                                  </Projects>
                                </eaw-ci:BuildConfiguration>";

            _mockFileData.TextContents = xml;

            var taskBuilderMock = new TaskBuilderMock(new Dictionary<string, object> {
                {"Id", "TestTask"},
                {"Name", "TestTask"},
                {"CopyFromPath", "path/to/source"},
                {"CopyToPath", "path/to/dest"},
                {"CopySubfolders", true},
                {"CopyFileByPattern", "*"},
                {"AlwaysOverwrite", false}
            });

            var factoryStub = new BuildComponentFactoryStub {TaskBuilder = taskBuilderMock};
            var sut = new XmlBuildConfigParser(_fileSystem, factoryStub);

            sut.Parse(Path);

            taskBuilderMock.Verify();
        }

        [TestMethod]
        public void GivenProjectWithJobAndCopyTask__WhenCallingParse__ShouldAddTaskToJob() {
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
                                              <AlwaysOverwrite>false</AlwaysOverwrite>
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
            var sut = new XmlBuildConfigParser(_fileSystem, factoryStub);

            sut.Parse(Path);

            AssertJobHasExpectedTask(jobStub, taskBuilderStub.Task);
        }

        [TestMethod]
        public void GivenGlobalCopyTaskAndReferenceInProjectJob__WhenCallingParse__ShouldAddTaskToJob() {
            const string xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                                <eaw-ci:BuildConfiguration
                                  ConfigVersion=""1.0.0"" 
                                  xmlns:eaw-ci=""eaw-ci""
                                  xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
                                  <GlobalTasks>
                                    <TaskDefinition Id=""TestTask"" xsi:type=""eaw-ci:Copy"">
                                      <CopyFromPath>path/to/source</CopyFromPath>
                                      <CopyToPath>path/to/dest</CopyToPath>
                                      <CopySubfolders>true</CopySubfolders>
                                      <CopyFileByPattern>*</CopyFileByPattern>
                                      <AlwaysOverwrite>false</AlwaysOverwrite>
                                    </TaskDefinition>
                                  </GlobalTasks>

                                  <Projects>
                                    <Project Id=""idvalue1"">
                                      <Jobs>
                                        <Job Id=""idvalue2"" Name=""TestJob"">
                                          <Tasks>
                                            <TaskReference ReferenceId=""TestTask"" />
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
            var sut = new XmlBuildConfigParser(_fileSystem, factoryStub);

            sut.Parse(Path);

            AssertJobHasExpectedTask(jobStub, taskBuilderStub.Task);
        }

        [TestMethod]
        public void
            GivenTwoGlobalTasksAndReferenceToSecondInProjectJob__WhenCallingParse__ShouldBuildSecondGlobalTask() {
            const string xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                                <eaw-ci:BuildConfiguration
                                  ConfigVersion=""1.0.0"" 
                                  xmlns:eaw-ci=""eaw-ci""
                                  xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
                                  <GlobalTasks>
                                    <TaskDefinition Id=""TestTask"" xsi:type=""eaw-ci:Copy"">
                                      <CopyFromPath>invalid</CopyFromPath>
                                      <CopyToPath>invalid</CopyToPath>
                                      <CopySubfolders>false</CopySubfolders>
                                      <CopyFileByPattern>*</CopyFileByPattern>
                                      <AlwaysOverwrite>false</AlwaysOverwrite>
                                    </TaskDefinition>

                                    <TaskDefinition Id=""ExpectedTask"" Name=""ExpectedTask"" xsi:type=""eaw-ci:Copy"">
                                      <CopyFromPath>path/to/source</CopyFromPath>
                                      <CopyToPath>path/to/dest</CopyToPath>
                                      <CopySubfolders>true</CopySubfolders>
                                      <CopyFileByPattern>*</CopyFileByPattern>
                                      <AlwaysOverwrite>false</AlwaysOverwrite>
                                    </TaskDefinition>
                                  </GlobalTasks>

                                  <Projects>
                                    <Project Id=""idvalue1"">
                                      <Jobs>
                                        <Job Id=""idvalue2"" Name=""TestJob"">
                                          <Tasks>
                                            <TaskReference ReferenceId=""ExpectedTask"" />
                                          </Tasks>
                                        </Job>
                                      </Jobs>
                                    </Project>
                                  </Projects>
                                </eaw-ci:BuildConfiguration>";

            _mockFileData.TextContents = xml;

            var jobStub = new JobStub();
            var taskBuilderMock = new TaskBuilderMock(new Dictionary<string, object> {
                {"Id", "ExpectedTask"},
                {"Name", "ExpectedTask"},
                {"CopyFromPath", "path/to/source"},
                {"CopyToPath", "path/to/dest"},
                {"CopySubfolders", true},
                {"CopyFileByPattern", "*"},
                {"AlwaysOverwrite", false}
            });

            var factoryStub = new BuildComponentFactoryStub {Job = jobStub, TaskBuilder = taskBuilderMock};
            var sut = new XmlBuildConfigParser(_fileSystem, factoryStub);

            sut.Parse(Path);

            taskBuilderMock.Verify();
        }

        [TestMethod]
        public void GivenProjectWithJobAndTwoTasks__WhenCallingParse__ShouldAddBothTasksToJob() {
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
                                            <Task Id=""idvalue2"" Name=""FirstTask"" xsi:type=""eaw-ci:Copy"">
                                              <CopyFromPath>path/to/first/source</CopyFromPath>
                                              <CopyToPath>path/to/first/dest</CopyToPath>
                                              <CopySubfolders>true</CopySubfolders>
                                              <CopyFileByPattern>*</CopyFileByPattern>
                                              <AlwaysOverwrite>false</AlwaysOverwrite>
                                            </Task>

                                            <Task Id=""idvalue3"" Name=""SecondTask"" xsi:type=""eaw-ci:Copy"">
                                              <CopyFromPath>path/to/second/source</CopyFromPath>
                                              <CopyToPath>path/to/second/dest</CopyToPath>
                                              <CopySubfolders>true</CopySubfolders>
                                              <CopyFileByPattern>*</CopyFileByPattern>
                                              <AlwaysOverwrite>false</AlwaysOverwrite>
                                            </Task>
                                          </Tasks>
                                        </Job>
                                      </Jobs>
                                    </Project>
                                  </Projects>
                                </eaw-ci:BuildConfiguration>";

            _mockFileData.TextContents = xml;

            var jobStub = new JobStub();
            var firstTask = new TaskDummy();
            var secondTask = new TaskDummy();

            var taskBuilderStub = new IteratingTaskBuilderStub();
            taskBuilderStub.Tasks.Add(firstTask);
            taskBuilderStub.Tasks.Add(secondTask);

            var factoryStub = new BuildComponentFactoryStub {Job = jobStub, TaskBuilder = taskBuilderStub};
            var sut = new XmlBuildConfigParser(_fileSystem, factoryStub);

            sut.Parse(Path);

            AssertJobHasExpectedTask(jobStub, firstTask);
            AssertJobHasExpectedTask(jobStub, secondTask);
        }


        [TestMethod]
        public void GivenProjectWithTwoJobs__WhenCallingParse__ShouldAddBothJobsToProject() {
            const string xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                                <eaw-ci:BuildConfiguration
                                  ConfigVersion=""1.0.0"" 
                                  xmlns:eaw-ci=""eaw-ci""
                                  xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
                                  <Projects>
                                    <Project Id=""idvalue0"">
                                      <Jobs>
                                        <Job Id=""FirstJob"" Name=""FirstJob"">
                                          <Tasks></Tasks>
                                        </Job>

                                        <Job Id=""SecondJob"" Name=""SecondJob"">
                                          <Tasks></Tasks>
                                        </Job>
                                      </Jobs>
                                    </Project>
                                  </Projects>
                                </eaw-ci:BuildConfiguration>";

            _mockFileData.TextContents = xml;

            var expectedJobs = new List<IJob> {new JobStub(), new JobStub()};
            var projectStub = new ProjectStub();
            var factoryStub = new JobIteratingBuildComponentFactoryStub {
                Project = projectStub,
                Jobs = expectedJobs
            };

            var sut = new XmlBuildConfigParser(_fileSystem, factoryStub);

            sut.Parse(Path);

            CollectionAssert.AreEqual(expectedJobs, projectStub.Jobs);
        }

        [TestMethod]
        public void GivenBuildConfigWithTwoProjects__WhenCallingParse__ShouldReturnBothProjects() {
            const string xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                                <eaw-ci:BuildConfiguration
                                  ConfigVersion=""1.0.0"" 
                                  xmlns:eaw-ci=""eaw-ci""
                                  xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
                                  <Projects>
                                    <Project Id=""FirstProject"">
                                        <Jobs>
                                            <Job Id=""Dummy"">
                                                <Tasks></Tasks>
                                            </Job>
                                        </Jobs>
                                    </Project>
                                    <Project Id=""SecondProject"">
                                        <Jobs>
                                            <Job Id=""Dummy2"">
                                                <Tasks></Tasks>
                                            </Job>
                                        </Jobs>
                                    </Project>
                                  </Projects>
                                </eaw-ci:BuildConfiguration>";

            _mockFileData.TextContents = xml;

            var expectedProjects = new List<IProject> {new ProjectDummy(), new ProjectDummy()};
            var factoryStub = new ProjectIteratingBuildComponentFactoryStub() {
                Projects = expectedProjects
            };

            var sut = new XmlBuildConfigParser(_fileSystem, factoryStub);

            var projects = sut.Parse(Path);

            CollectionAssert.AreEqual(expectedProjects, projects.ToList());
        }

        [TestMethod]
        [DataRow(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                                <eaw-ci:BuildConfiguration
                                  ConfigVersion=""1.0.0"" 
                                  xmlns:eaw-ci=""eaw-ci""
                                  xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
                                  <Projects>
                                    <Project Id=""idvalue0"">
                                      <Jobs>
                                        <Job Id=""FirstJob"" Name=""FirstJob"">
                                          <Tasks></Tasks>
                                        </Job>

                                        <Job Id=""SecondJob"" Name=""SecondJob"">
                                          <Tasks></Tasks>
                                        </Job>
                                      </Jobs>
                                    </Project>
                                  </Projects>
                                </eaw-ci:BuildConfiguration>", true)]
        [DataRow(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                                <eaw-ci:BuildConfiguration
                                  ConfigVersion=""3.0.0"" 
                                  xmlns:eaw-ci=""eaw-ci""
                                  xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
                                  <Projects>
                                    <Project Id=""idvalue0"">
                                      <Jobs>
                                        <Job Id=""FirstJob"" Name=""FirstJob"">
                                          <Tasks></Tasks>
                                        </Job>

                                        <Job Id=""SecondJob"" Name=""SecondJob"">
                                          <Tasks></Tasks>
                                        </Job>
                                      </Jobs>
                                    </Project>
                                  </Projects>
                                </eaw-ci:BuildConfiguration>", false)]
        public void GivenConfig__TestIsValidConfiguration__IsExpected(string xmlContent, bool expected) {
            _mockFileData.TextContents = xmlContent;
            var sut = new XmlBuildConfigParser(_fileSystem, null);
            var actual = sut.TestIsValidConfiguration(Path);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenNullConfig__TestIsValidConfiguration__ReturnsFalse() {
            var sut = new XmlBuildConfigParser(_fileSystem, null);
            var actual = sut.TestIsValidConfiguration(null);
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void GivenEmptyConfig__TestIsValidConfiguration__ReturnsFalse() {
            const string xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>";
            _mockFileData.TextContents = xml;
            var sut = new XmlBuildConfigParser(_fileSystem, null);
            var actual = sut.TestIsValidConfiguration(Path);
            Assert.IsFalse(actual);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void
            GivenProjectWithJobAndTaskReferenceToNonExistingGlobalTask__WhenCallingParse__ShouldThrowInvalidOperationException() {
            const string xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                                <eaw-ci:BuildConfiguration
                                  ConfigVersion=""1.0.0"" 
                                  xmlns:eaw-ci=""eaw-ci""
                                  xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
                                  <GlobalTasks>
                                  </GlobalTasks>

                                  <Projects>
                                    <Project Id=""idvalue1"">
                                      <Jobs>
                                        <Job Id=""idvalue2"" Name=""TestJob"">
                                          <Tasks>
                                            <TaskReference ReferenceId=""TestTask"" />
                                          </Tasks>
                                        </Job>
                                      </Jobs>
                                    </Project>
                                  </Projects>
                                </eaw-ci:BuildConfiguration>";

            _mockFileData.TextContents = xml;

            var factoryStub = new BuildComponentFactoryStub();
            var sut = new XmlBuildConfigParser(_fileSystem, factoryStub);

            sut.Parse(Path);
        }


        private static void AssertJobHasExpectedTask(JobStub jobStub, ITask element) {
            CollectionAssert.Contains(jobStub.Tasks, element,
                "Should Job should have expected task, but does not.");
        }

        private static void AssertProjectNameEquals(string projectName, string actualName) {
            Assert.AreEqual(projectName, actualName,
                $"Should return Project with name {projectName}, but was {actualName}");
        }

        private static void AssertJobNameEquals(string jobName, IJob actualJob) {
            Assert.AreEqual(jobName, actualJob.Name, $"Job name should be {jobName}, but was {actualJob.Name}");
        }
    }
}