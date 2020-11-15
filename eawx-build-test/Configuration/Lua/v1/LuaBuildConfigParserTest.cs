using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using EawXBuild.Configuration.Lua.v1;
using EawXBuild.Core;
using EawXBuild.Steam;
using EawXBuildTest.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Configuration.Lua.v1 {
    [TestClass]
    public class LuaBuildConfigParserTest {
        private const string Path = "luaConfig.lua";

        private MockFileSystem _fileSystem;
        private MockFileData _mockFileData;
        private LuaMockFileSystemParser _luaMockFileParser;

        [TestInitialize]
        public void SetUp() {
            _mockFileData = new MockFileData(string.Empty);
            _fileSystem = new MockFileSystem();
            _fileSystem.AddFile(Path, _mockFileData);
            _luaMockFileParser = new LuaMockFileSystemParser(_fileSystem);
        }

        [TestMethod]
        public void GivenConfigWithProject__WhenParsing__ShouldReturnProject() {
            const string lua = "project('test')";
            _mockFileData.TextContents = lua;

            var factoryStub = new BuildComponentFactoryStub();
            var projects = MakeSutAndParse(factoryStub);

            var actual = projects.First();
            Assert.AreEqual("test", actual.Name);
        }

        [TestMethod]
        public void GivenConfigWithProjectAndDifferentName__WhenParsing__ShouldReturnProject() {
            const string lua = "project('another-project')";
            _mockFileData.TextContents = lua;

            var factoryStub = new BuildComponentFactoryStub();
            var projects = MakeSutAndParse(factoryStub);

            var actual = projects.First();
            Assert.AreEqual("another-project", actual.Name);
        }

        [TestMethod]
        public void GivenConfigWithProjectAndJob__WhenParsing__ProjectShouldHaveJob() {
            const string lua = @"
                local p = project('test')
                p:add_job('test-job')  
            ";
            _mockFileData.TextContents = lua;

            var jobStub = new JobStub();
            var factoryStub = MakeBuildComponentFactoryStub(jobStub);
            var projects = MakeSutAndParse(factoryStub);

            var actual = projects.First() as ProjectStub;
            var actualJob = actual?.Jobs.First();
            Assert.AreSame(jobStub, actualJob);
            Assert.AreEqual("test-job", actualJob?.Name);
        }

        [TestMethod]
        public void GivenConfigWithProjectWithJobAndCopyTask__WhenParsing__JobShouldHaveTask() {
            const string lua = @"
                local p = project('test')
                local j = p:add_job('test-job')
                j:add_task(copy('a', 'b'))  
            ";
            _mockFileData.TextContents = lua;

            var jobStub = new JobStub();
            var taskDummy = new TaskDummy();
            var factoryStub = MakeBuildComponentFactoryStub(jobStub, taskDummy);

            MakeSutAndParse(factoryStub);

            var actualTask = jobStub.Tasks.First();
            Assert.AreEqual(taskDummy, actualTask);
        }

        [TestMethod]
        public void GivenConfigWithCopyTaskWithSettings__WhenParsing__JobShouldHaveTask() {
            const string lua = @"
                local p = project('test')
                local j = p:add_job('test-job')
                j:add_task(copy('a', 'b'):
                           overwrite(true):
                           pattern('*.xml'):
                           recursive(true))  
            ";
            _mockFileData.TextContents = lua;

            var jobStub = new JobStub();
            var taskDummy = new TaskDummy();
            var factoryStub = MakeBuildComponentFactoryStub(jobStub, taskDummy);

            MakeSutAndParse(factoryStub);

            var actualTask = jobStub.Tasks.First();
            Assert.AreEqual(taskDummy, actualTask);
        }

        [TestMethod]
        public void GivenConfigWithCopyTaskWithSettings__WhenParsing__TaskShouldBeConfiguredCorrectly() {
            const string lua = @"
                local p = project('test')
                local j = p:add_job('test-job')
                j:add_task(copy('a', 'b'):
                           overwrite(true):
                           pattern('*.xml'):
                           recursive(true))  
            ";
            _mockFileData.TextContents = lua;

            var taskBuilderMock = new TaskBuilderMock(new Dictionary<string, object> {
                {"CopyFromPath", "a"},
                {"CopyToPath", "b"},
                {"AlwaysOverwrite", true},
                {"CopyFileByPattern", "*.xml"},
                {"CopySubfolders", true}
            });

            var factoryStub = new BuildComponentFactoryStub {TaskBuilder = taskBuilderMock};
            MakeSutAndParse(factoryStub);

            taskBuilderMock.Verify();
        }

        [TestMethod]
        public void GivenConfigWithProjectWithJobAndLinkTask__WhenParsing__JobShouldHaveTask() {
            const string lua = @"
                local p = project('test')
                local j = p:add_job('test-job')
                j:add_task(link('a', 'b'))  
            ";
            _mockFileData.TextContents = lua;

            var jobStub = new JobStub();
            var taskDummy = new TaskDummy();
            var factoryStub = MakeBuildComponentFactoryStub(jobStub, taskDummy);

            MakeSutAndParse(factoryStub);

            var actualTask = jobStub.Tasks.First();
            Assert.AreEqual(taskDummy, actualTask);
        }

        [TestMethod]
        public void GivenConfigWithProjectWithJobAndCleanTask__WhenParsing__JobShouldHaveTask() {
            const string lua = @"
                local p = project('test')
                local j = p:add_job('test-job')
                j:add_task(clean('a'))  
            ";
            _mockFileData.TextContents = lua;

            var jobStub = new JobStub();
            var taskDummy = new TaskDummy();
            var factoryStub = MakeBuildComponentFactoryStub(jobStub, taskDummy);

            MakeSutAndParse(factoryStub);

            var actualTask = jobStub.Tasks.First();
            Assert.AreEqual(taskDummy, actualTask);
        }

        [TestMethod]
        public void GivenConfigWithCleanTask__WhenParsing__TaskShouldBeConfiguredCorrectly() {
            const string lua = @"
                local p = project('test')
                local j = p:add_job('test-job')
                j:add_task(clean('path'))  
            ";
            _mockFileData.TextContents = lua;

            var taskBuilderMock = new TaskBuilderMock(new Dictionary<string, object> {
                {"Path", "path"},
            });

            var factoryStub = new BuildComponentFactoryStub {TaskBuilder = taskBuilderMock};
            MakeSutAndParse(factoryStub);

            taskBuilderMock.Verify();
        }

        [TestMethod]
        public void GivenConfigWithProjectWithJobAndRunProcessTask__WhenParsing__JobShouldHaveTask() {
            const string lua = @"
                local p = project('test')
                local j = p:add_job('test-job')
                j:add_task(run_process('echo'))  
            ";
            _mockFileData.TextContents = lua;

            var jobStub = new JobStub();
            var taskDummy = new TaskDummy();
            var factoryStub = MakeBuildComponentFactoryStub(jobStub, taskDummy);

            MakeSutAndParse(factoryStub);

            var actualTask = jobStub.Tasks.First();
            Assert.AreEqual(taskDummy, actualTask);
        }

        [TestMethod]
        public void GivenConfigWithProjectRunProcessTaskWithSettings__WhenParsing__TaskShouldBeConfiguredCorrectly() {
            const string lua = @"
                local p = project('test')
                local j = p:add_job('test-job')
                j:add_task(run_process('echo'):
                           arguments('Hello World'):
                           working_directory('sub/dir'):
                           allowed_to_fail(true)
                )  
            ";
            _mockFileData.TextContents = lua;

            var taskBuilderMock = new TaskBuilderMock(new Dictionary<string, object> {
                {"ExecutablePath", "echo"},
                {"Arguments", "Hello World"},
                {"WorkingDirectory", "sub/dir"},
                {"AllowedToFail", true}
            });

            var factoryStub = new BuildComponentFactoryStub {TaskBuilder = taskBuilderMock};
            MakeSutAndParse(factoryStub);

            taskBuilderMock.Verify();
        }

        [TestMethod]
        public void GivenConfigWithProjectWithJobAndCreateSteamWorkshopItemTask__WhenParsing__JobShouldHaveTask() {
            const string lua = @"
                local p = project('test')
                local j = p:add_job('test-job')
                j:add_task(create_steam_workshop_item {
                    app_id = 32470,
                    title = 'my-test-item',
                    description_file = 'path/to/description',
                    item_folder = 'path/to/folder',
                    visibility = visibility.private,
                    language = 'English'
                })  
            ";
            _mockFileData.TextContents = lua;

            var jobStub = new JobStub();
            var taskDummy = new TaskDummy();
            var factoryStub = MakeBuildComponentFactoryStub(jobStub, taskDummy);

            MakeSutAndParse(factoryStub);

            var actualTask = jobStub.Tasks.First();
            Assert.AreEqual(taskDummy, actualTask);
        }

        [TestMethod]
        public void GivenConfigWithCreateSteamWorkshopItemTask__WhenParsing__TaskShouldBeConfiguredWithGivenSettings() {
            const string lua = @"
                local p = project('test')
                local j = p:add_job('test-job')
                j:add_task(create_steam_workshop_item {
                    app_id = 32470,
                    title = 'my-test-item',
                    description_file = 'path/to/description',
                    item_folder = 'path/to/folder',
                    visibility = visibility.private,
                    language = 'English'
                })
            ";
            _mockFileData.TextContents = lua;

            var taskBuilderMock = new TaskBuilderMock(new Dictionary<string, object> {
                {"AppId", (uint) 32470},
                {"Title", "my-test-item"},
                {"DescriptionFilePath", "path/to/description"},
                {"ItemFolderPath", "path/to/folder"},
                {"Visibility", WorkshopItemVisibility.Private},
                {"Language", "English"},
            });

            var factoryStub = new BuildComponentFactoryStub {TaskBuilder = taskBuilderMock};
            MakeSutAndParse(factoryStub);

            taskBuilderMock.Verify();
        }

        [TestMethod]
        public void GivenConfigWithProjectWithJobAndUpdateSteamWorkshopItemTask__WhenParsing__JobShouldHaveTask() {
            const string lua = @"
                local p = project('test')
                local j = p:add_job('test-job')
                j:add_task(update_steam_workshop_item {
                    app_id = 32470,
                    title = 'my-test-item',
                    description_file = 'path/to/description',
                    item_folder = 'path/to/folder',
                    visibility = visibility.private,
                    language = 'English'
                })  
            ";
            _mockFileData.TextContents = lua;

            var jobStub = new JobStub();
            var taskDummy = new TaskDummy();
            var factoryStub = MakeBuildComponentFactoryStub(jobStub, taskDummy);

            MakeSutAndParse(factoryStub);

            var actualTask = jobStub.Tasks.First();
            Assert.AreEqual(taskDummy, actualTask);
        }

        [TestMethod]
        public void GivenConfigWithUpdateSteamWorkshopItemTask__WhenParsing__TaskShouldBeConfiguredWithGivenSettings() {
            const string lua = @"
                local p = project('test')
                local j = p:add_job('test-job')
                j:add_task(update_steam_workshop_item {
                    app_id = 32470,
                    item_id = 1234,
                    title = 'my-test-item',
                    description_file = 'path/to/description',
                    item_folder = 'path/to/folder',
                    visibility = visibility.private,
                    language = 'English'
                })
            ";
            _mockFileData.TextContents = lua;

            var taskBuilderMock = new TaskBuilderMock(new Dictionary<string, object> {
                {"AppId", (uint) 32470},
                {"ItemId", (ulong) 1234},
                {"Title", "my-test-item"},
                {"DescriptionFilePath", "path/to/description"},
                {"ItemFolderPath", "path/to/folder"},
                {"Visibility", WorkshopItemVisibility.Private},
                {"Language", "English"},
            });

            var factoryStub = new BuildComponentFactoryStub {TaskBuilder = taskBuilderMock};
            MakeSutAndParse(factoryStub);

            taskBuilderMock.Verify();
        }

        [TestMethod]
        public void GivenJobWithMultipleTasks__WhenCallingParse__JobShouldHaveAllTasks() {
            const string lua = @"
                local p = project('test')
                local j = p:add_job('test-job')
                j:add_tasks { run_process('echo'), copy('a', 'b') }  
            ";
            _mockFileData.TextContents = lua;

            var jobStub = new JobStub();
            ITask[] expectedTasks = {new TaskDummy(), new TaskDummy()};
            var factoryStub = new BuildComponentFactoryStub {
                Job = jobStub,
                TaskBuilder = new IteratingTaskBuilderStub {
                    Tasks = expectedTasks.ToList()
                }
            };

            MakeSutAndParse(factoryStub);

            var actualTasks = jobStub.Tasks;
            CollectionAssert.AreEqual(expectedTasks, actualTasks);
        }

        private static BuildComponentFactoryStub MakeBuildComponentFactoryStub(JobDummy job, TaskDummy task = null) {
            var factoryStub = new BuildComponentFactoryStub {
                Project = new ProjectStub(),
                Job = job,
                TaskBuilder = new TaskBuilderStub {
                    Task = task ?? new TaskDummy()
                }
            };
            return factoryStub;
        }

        private IEnumerable<IProject> MakeSutAndParse(IBuildComponentFactory factoryStub) {
            var sut = new LuaBuildConfigParser(_luaMockFileParser, factoryStub);
            var projects = sut.Parse(Path);
            return projects;
        }
    }
}