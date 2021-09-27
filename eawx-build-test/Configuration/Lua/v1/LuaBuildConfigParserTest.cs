using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using EawXBuild.Configuration.Lua.v1;
using EawXBuild.Core;
using EawXBuild.Steam;
using EawXBuildTest.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Configuration.Lua.v1
{
    [TestClass]
    public class LuaBuildConfigParserTest
    {
        private const string Path = "luaConfig.lua";

        private static readonly string[] ExpectedTags = { "EAW", "FOC" };

        private MockFileSystem _fileSystem;
        private LuaMockFileSystemParser _luaMockFileParser;
        private MockFileData _mockFileData;

        [TestInitialize]
        public void SetUp()
        {
            _mockFileData = new MockFileData(string.Empty);
            _fileSystem = new MockFileSystem();
            _fileSystem.AddFile(Path, _mockFileData);
            _luaMockFileParser = new LuaMockFileSystemParser(_fileSystem);
        }

        [TestMethod]
        public void GivenConfigWithProject__WhenParsing__ShouldReturnProject()
        {
            const string lua = "project('test')";
            _mockFileData.TextContents = lua;

            BuildComponentFactoryStub factoryStub = new BuildComponentFactoryStub();
            IEnumerable<IProject> projects = MakeSutAndParse(factoryStub);

            IProject actual = projects.First();
            Assert.AreEqual("test", actual.Name);
        }

        [TestMethod]
        public void GivenConfigWithProjectAndDifferentName__WhenParsing__ShouldReturnProject()
        {
            const string lua = "project('another-project')";
            _mockFileData.TextContents = lua;

            BuildComponentFactoryStub factoryStub = new BuildComponentFactoryStub();
            IEnumerable<IProject> projects = MakeSutAndParse(factoryStub);

            IProject actual = projects.First();
            Assert.AreEqual("another-project", actual.Name);
        }

        [TestMethod]
        public void GivenConfigWithProjectAndJob__WhenParsing__ProjectShouldHaveJob()
        {
            const string lua = @"
                local p = project('test')
                p:job('test-job')  
            ";
            _mockFileData.TextContents = lua;

            JobStub jobStub = new JobStub();
            BuildComponentFactoryStub factoryStub = MakeBuildComponentFactoryStub(jobStub);
            IEnumerable<IProject> projects = MakeSutAndParse(factoryStub);

            ProjectStub actual = projects.First() as ProjectStub;
            IJob actualJob = actual?.Jobs.First();
            Assert.AreSame(jobStub, actualJob);
            Assert.AreEqual("test-job", actualJob?.Name);
        }

        [TestMethod]
        public void GivenConfigWithProjectWithJobAndCopyTask__WhenParsing__JobShouldHaveTask()
        {
            const string lua = @"
                local p = project('test')
                local j = p:job('test-job')
                j:tasks {
                    {
                        name = 'CopyTask',
                        action = copy('a', 'b')
                    }
                }
            ";
            _mockFileData.TextContents = lua;

            JobStub jobStub = new JobStub();
            TaskDummy taskDummy = new TaskDummy();
            BuildComponentFactoryStub factoryStub = MakeBuildComponentFactoryStub(jobStub, taskDummy);

            MakeSutAndParse(factoryStub);

            ITask actualTask = jobStub.Tasks.First();
            Assert.AreEqual(taskDummy, actualTask);
        }

        [TestMethod]
        public void GivenConfigWithTaskWithName__WhenParsing__TaskObjectShouldHaveName()
        {
            const string lua = @"
                local p = project('test')
                local j = p:job('test-job')
                j:tasks {
                    {
                        name = 'CopyTask',
                        action = copy('a', 'b')
                    }
                }  
            ";

            _mockFileData.TextContents = lua;

            JobStub jobStub = new JobStub();
            TaskDummy taskDummy = new TaskDummy();
            BuildComponentFactoryStub factoryStub = MakeBuildComponentFactoryStub(jobStub, taskDummy);

            MakeSutAndParse(factoryStub);

            ITask actualTask = jobStub.Tasks.First();
            Assert.AreEqual(taskDummy.Name, "CopyTask");
        }

        [TestMethod]
        public void GivenConfigWithCopyTaskWithSettings__WhenParsing__JobShouldHaveTask()
        {
            const string lua = @"
                local p = project('test')
                local j = p:job('test-job')
                j:tasks {
                    {
                        name = 'CopyTask',
                        action = copy('a', 'b'):
                                overwrite(true):
                                pattern('*.xml'):
                                recursive(true)
                    }
                }  
            ";
            _mockFileData.TextContents = lua;

            JobStub jobStub = new JobStub();
            TaskDummy taskDummy = new TaskDummy();
            BuildComponentFactoryStub factoryStub = MakeBuildComponentFactoryStub(jobStub, taskDummy);

            MakeSutAndParse(factoryStub);

            ITask actualTask = jobStub.Tasks.First();
            Assert.AreEqual(taskDummy, actualTask);
        }

        [TestMethod]
        public void GivenConfigWithCopyTaskWithSettings__WhenParsing__TaskShouldBeConfiguredCorrectly()
        {
            const string lua = @"
                local p = project('test')
                local j = p:job('test-job')
                j:tasks{
                    {
                        name = 'CopyTask',
                        action = copy('a', 'b'):
                                    overwrite(true):
                                    pattern('*.xml'):
                                    recursive(true)
                    }
                }
            ";
            _mockFileData.TextContents = lua;

            TaskBuilderMock taskBuilderMock = new TaskBuilderMock(new Dictionary<string, object>
            {
                {"CopyFromPath", "a"},
                {"CopyToPath", "b"},
                {"AlwaysOverwrite", true},
                {"CopyFileByPattern", "*.xml"},
                {"CopySubfolders", true}
            });

            BuildComponentFactoryStub factoryStub = new BuildComponentFactoryStub { TaskBuilder = taskBuilderMock };
            MakeSutAndParse(factoryStub);

            taskBuilderMock.Verify();
        }

        [TestMethod]
        public void GivenConfigWithProjectWithJobAndLinkTask__WhenParsing__JobShouldHaveTask()
        {
            const string lua = @"
                local p = project('test')
                local j = p:job('test-job')
                j:tasks {
                    {
                        name = 'LinkTask',
                        action = link('a', 'b')
                    }
                }
            ";
            _mockFileData.TextContents = lua;

            JobStub jobStub = new JobStub();
            TaskDummy taskDummy = new TaskDummy();
            BuildComponentFactoryStub factoryStub = MakeBuildComponentFactoryStub(jobStub, taskDummy);

            MakeSutAndParse(factoryStub);

            ITask actualTask = jobStub.Tasks.First();
            Assert.AreEqual(taskDummy, actualTask);
        }

        [TestMethod]
        public void GivenConfigWithProjectWithJobAndCleanTask__WhenParsing__JobShouldHaveTask()
        {
            const string lua = @"
                local p = project('test')
                local j = p:job('test-job')
                j:tasks {
                    {
                        name = 'CleanTask',
                        action = clean('a'),
                    }
                }
            ";
            _mockFileData.TextContents = lua;

            JobStub jobStub = new JobStub();
            TaskDummy taskDummy = new TaskDummy();
            BuildComponentFactoryStub factoryStub = MakeBuildComponentFactoryStub(jobStub, taskDummy);

            MakeSutAndParse(factoryStub);

            ITask actualTask = jobStub.Tasks.First();
            Assert.AreEqual(taskDummy, actualTask);
        }

        [TestMethod]
        public void GivenConfigWithCleanTask__WhenParsing__TaskShouldBeConfiguredCorrectly()
        {
            const string lua = @"
                local p = project('test')
                local j = p:job('test-job')
                j:tasks {
                    {
                        name = 'CleanTask',
                        action = clean('path')
                    }
                }
            ";
            _mockFileData.TextContents = lua;

            TaskBuilderMock taskBuilderMock = new TaskBuilderMock(new Dictionary<string, object>
            {
                {"Path", "path"}
            });

            BuildComponentFactoryStub factoryStub = new BuildComponentFactoryStub { TaskBuilder = taskBuilderMock };
            MakeSutAndParse(factoryStub);

            taskBuilderMock.Verify();
        }

        [TestMethod]
        public void GivenConfigWithProjectWithJobAndRunProcessTask__WhenParsing__JobShouldHaveTask()
        {
            const string lua = @"
                local p = project('test')
                local j = p:job('test-job')
                j:tasks {
                    {
                        name = 'RunEcho',
                        action = run_process('echo')
                    }
                }
            ";
            _mockFileData.TextContents = lua;

            JobStub jobStub = new JobStub();
            TaskDummy taskDummy = new TaskDummy();
            BuildComponentFactoryStub factoryStub = MakeBuildComponentFactoryStub(jobStub, taskDummy);

            MakeSutAndParse(factoryStub);

            ITask actualTask = jobStub.Tasks.First();
            Assert.AreEqual(taskDummy, actualTask);
        }

        [TestMethod]
        public void GivenConfigWithProjectRunProcessTaskWithSettings__WhenParsing__TaskShouldBeConfiguredCorrectly()
        {
            const string lua = @"
                local p = project('test')
                local j = p:job('test-job')
                j:tasks {
                    {
                        name = 'RunEcho',
                        action = run_process('echo'):
                                    arguments('Hello World'):
                                    working_directory('sub/dir'):
                                    allowed_to_fail(true)
                    }
                }  
            ";
            _mockFileData.TextContents = lua;

            TaskBuilderMock taskBuilderMock = new TaskBuilderMock(new Dictionary<string, object>
            {
                {"ExecutablePath", "echo"},
                {"Arguments", "Hello World"},
                {"WorkingDirectory", "sub/dir"},
                {"AllowedToFail", true}
            });

            BuildComponentFactoryStub factoryStub = new BuildComponentFactoryStub { TaskBuilder = taskBuilderMock };
            MakeSutAndParse(factoryStub);

            taskBuilderMock.Verify();
        }

        [TestMethod]
        public void GivenConfigWithProjectWithJobAndCreateSteamWorkshopItemTask__WhenParsing__JobShouldHaveTask()
        {
            const string lua = @"
                local p = project('test')
                local j = p:job('test-job')
                j:tasks {
                    {
                        name = 'Create Workshop Item',
                        action = create_steam_workshop_item {
                            app_id = 32470,
                            title = 'my-test-item',
                            description_file = 'path/to/description',
                            item_folder = 'path/to/folder',
                            visibility = visibility.private,
                            tags = {'EAW', 'FOC'},
                            language = 'English'
                        }
                    }
                }
            ";
            _mockFileData.TextContents = lua;

            JobStub jobStub = new JobStub();
            TaskDummy taskDummy = new TaskDummy();
            BuildComponentFactoryStub factoryStub = MakeBuildComponentFactoryStub(jobStub, taskDummy);

            MakeSutAndParse(factoryStub);

            ITask actualTask = jobStub.Tasks.First();
            Assert.AreEqual(taskDummy, actualTask);
        }

        [TestMethod]
        public void
            GivenConfigWithCreateSteamWorkshopItemTask_WithoutTags__WhenParsing__TaskShouldBeConfiguredWithGivenSettings()
        {
            const string lua = @"
                local p = project('test')
                local j = p:job('test-job')
                j:tasks {
                    {
                        name = 'Create Workshop Item',
                        action = create_steam_workshop_item {
                            app_id = 32470,
                            title = 'my-test-item',
                            description_file = 'path/to/description',
                            item_folder = 'path/to/folder',
                            visibility = visibility.private,
                            language = 'English'
                        }
                    }
                }
            ";
            _mockFileData.TextContents = lua;

            TaskBuilderMock taskBuilderMock = new TaskBuilderMock(new Dictionary<string, object>
            {
                {"AppId", (uint) 32470},
                {"Title", "my-test-item"},
                {"DescriptionFilePath", "path/to/description"},
                {"ItemFolderPath", "path/to/folder"},
                {"Visibility", WorkshopItemVisibility.Private},
                {"Language", "English"}
            });

            BuildComponentFactoryStub factoryStub = new BuildComponentFactoryStub { TaskBuilder = taskBuilderMock };
            MakeSutAndParse(factoryStub);

            taskBuilderMock.Verify();
        }

        /// <summary>
        ///     For this test we're not using the TaskBuilderMock, because it uses CollectionAssert under the hood, which doesn't
        ///     do deep comparisons.
        ///     Instead we're querying the "Tags" key manually
        /// </summary>
        [TestMethod]
        public void
            GivenConfigWithCreateSteamWorkshopItemTask_WithTags__WhenParsing__TaskShouldBeConfiguredWithGivenTags()
        {
            const string lua = @"
                local p = project('test')
                local j = p:job('test-job')
                j:tasks {
                    {
                        name = 'Create Workshop Item',
                        action = create_steam_workshop_item {
                            app_id = 32470,
                            title = 'my-test-item',
                            description_file = 'path/to/description',
                            item_folder = 'path/to/folder',
                            visibility = visibility.private,
                            language = 'English',
                            tags = {'EAW', 'FOC'}
                        }
                    }
                }
            ";
            _mockFileData.TextContents = lua;

            TaskBuilderSpy taskBuilderSpy = new TaskBuilderSpy();

            BuildComponentFactoryStub factoryStub = new BuildComponentFactoryStub { TaskBuilder = taskBuilderSpy };
            MakeSutAndParse(factoryStub);

            object actual = taskBuilderSpy["Tags"];
            Assert.IsInstanceOfType(actual, typeof(IEnumerable<string>));
            CollectionAssert.AreEquivalent(ExpectedTags, ((IEnumerable<string>)actual).ToArray());
        }

        [TestMethod]
        public void GivenConfigWithProjectWithJobAndUpdateSteamWorkshopItemTask__WhenParsing__JobShouldHaveTask()
        {
            const string lua = @"
                local p = project('test')
                local j = p:job('test-job')
                j:tasks {
                    {
                        name = 'Update Item',
                        action = update_steam_workshop_item {
                            app_id = 32470,
                            item_id = 1234,
                            title = 'my-test-item',
                            description_file = 'path/to/description',
                            item_folder = 'path/to/folder',
                            visibility = visibility.private,
                            language = 'English'
                        }
                    }
                }  
            ";
            _mockFileData.TextContents = lua;

            JobStub jobStub = new JobStub();
            TaskDummy taskDummy = new TaskDummy();
            BuildComponentFactoryStub factoryStub = MakeBuildComponentFactoryStub(jobStub, taskDummy);

            MakeSutAndParse(factoryStub);

            ITask actualTask = jobStub.Tasks.First();
            Assert.AreEqual(taskDummy, actualTask);
        }

        [TestMethod]
        public void
            GivenConfigWithUpdateSteamWorkshopItemTask_WithoutTags__WhenParsing__TaskShouldBeConfiguredWithGivenSettings()
        {
            const string lua = @"
                local p = project('test')
                local j = p:job('test-job')
                j:tasks {
                    {
                        name = 'Update Item',
                        action = update_steam_workshop_item {
                            app_id = 32470,
                            item_id = 1234,
                            title = 'my-test-item',
                            description_file = 'path/to/description',
                            item_folder = 'path/to/folder',
                            visibility = visibility.private,
                            language = 'English'
                        }
                    }
                }  
            ";
            _mockFileData.TextContents = lua;

            TaskBuilderMock taskBuilderMock = new TaskBuilderMock(new Dictionary<string, object>
            {
                {"AppId", (uint) 32470},
                {"ItemId", (ulong) 1234},
                {"Title", "my-test-item"},
                {"DescriptionFilePath", "path/to/description"},
                {"ItemFolderPath", "path/to/folder"},
                {"Visibility", WorkshopItemVisibility.Private},
                {"Language", "English"}
            });

            BuildComponentFactoryStub factoryStub = new BuildComponentFactoryStub { TaskBuilder = taskBuilderMock };
            MakeSutAndParse(factoryStub);

            taskBuilderMock.Verify();
        }

        /// <summary>
        ///     For this test we're not using the TaskBuilderMock, because it uses CollectionAssert under the hood, which doesn't
        ///     do deep comparisons.
        ///     Instead we're querying the "Tags" key manually
        /// </summary>
        [TestMethod]
        public void
            GivenConfigWithUpdateSteamWorkshopItemTask_WithTags__WhenParsing__TaskShouldBeConfiguredWithGivenTags()
        {
            const string lua = @"
                local p = project('test')
                local j = p:job('test-job')
                j:tasks {
                    {
                        name = 'Update Item',
                        action = update_steam_workshop_item {
                            app_id = 32470,
                            item_id = 1234,
                            title = 'my-test-item',
                            description_file = 'path/to/description',
                            item_folder = 'path/to/folder',
                            visibility = visibility.private,
                            language = 'English',
                            tags = {'EAW', 'FOC'}
                        }
                    }
                }
            ";
            _mockFileData.TextContents = lua;

            TaskBuilderSpy taskBuilderMock = new TaskBuilderSpy();

            BuildComponentFactoryStub factoryStub = new BuildComponentFactoryStub { TaskBuilder = taskBuilderMock };
            MakeSutAndParse(factoryStub);

            object actual = taskBuilderMock["Tags"];
            Assert.IsInstanceOfType(actual, typeof(IEnumerable<string>));
            CollectionAssert.AreEquivalent(ExpectedTags, ((IEnumerable<string>)actual).ToArray());
        }

        [TestMethod]
        public void GivenJobWithMultipleTasks__WhenCallingParse__JobShouldHaveAllTasks()
        {
            const string lua = @"
                local p = project('test')
                local j = p:job('test-job')
                j:tasks { 
                    {
                        name = 'Run Echo',
                        action = run_process('echo')
                    },
                    {
                        name = 'Copy things',
                        action = copy('a', 'b') 
                    }
                }  
            ";
            _mockFileData.TextContents = lua;

            JobStub jobStub = new JobStub();
            ITask[] expectedTasks = { new TaskDummy(), new TaskDummy() };
            BuildComponentFactoryStub factoryStub = new BuildComponentFactoryStub
            {
                Job = jobStub,
                TaskBuilder = new IteratingTaskBuilderStub
                {
                    Tasks = expectedTasks.ToList()
                }
            };

            MakeSutAndParse(factoryStub);

            List<ITask> actualTasks = jobStub.Tasks;
            CollectionAssert.AreEqual(expectedTasks, actualTasks);
        }

        private static BuildComponentFactoryStub MakeBuildComponentFactoryStub(JobDummy job, TaskDummy task = null)
        {
            BuildComponentFactoryStub factoryStub = new BuildComponentFactoryStub
            {
                Project = new ProjectStub(),
                Job = job,
                TaskBuilder = new TaskBuilderStub
                {
                    Task = task ?? new TaskDummy()
                }
            };
            return factoryStub;
        }

        private IEnumerable<IProject> MakeSutAndParse(IBuildComponentFactory factoryStub)
        {
            LuaBuildConfigParser sut = new LuaBuildConfigParser(_luaMockFileParser, factoryStub);
            IEnumerable<IProject> projects = sut.Parse(Path);
            return projects;
        }
    }
}