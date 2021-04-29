using System.Collections.Generic;
using System.Linq;
using EawXBuild.Configuration.Lua.v1;
using EawXBuildTest.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Configuration.Lua.v1
{
    [TestClass]
    public class LuaCopyTaskTest
    {
        private const string Source = "original.txt";
        private const string Target = "copy.txt";
        private TaskBuilderMock _taskBuilderMock;


        [TestMethod]
        public void GivenLuaCopyTaskWithSourceAndDestination__ShouldBuildTaskWithCorrectSettings()
        {
            InitTaskBuilderMock();

            LuaCopyTask sut = new LuaCopyTask(_taskBuilderMock, "original.txt", "copy.txt");

            _taskBuilderMock.Verify();
        }

        [TestMethod]
        public void GivenLuaCopyTask__WhenCallingOverwrite__ShouldBuildTaskWithOverwrite()
        {
            InitTaskBuilderMock(new Dictionary<string, object> {{"AlwaysOverwrite", true}});

            LuaCopyTask sut = new LuaCopyTask(_taskBuilderMock, Source, Target);
            sut.overwrite(true);

            _taskBuilderMock.Verify();
        }

        [TestMethod]
        public void GivenLuaCopyTask__WhenCallingOverwrite__ShouldReturnItself()
        {
            InitTaskBuilderMock(new Dictionary<string, object> {{"AlwaysOverwrite", true}});

            LuaCopyTask sut = new LuaCopyTask(_taskBuilderMock, Source, Target);
            LuaCopyTask actual = sut.overwrite(true);

            Assert.AreSame(sut, actual);
        }

        [TestMethod]
        public void GivenLuaCopyTask__WhenCallingPattern__ShouldBuildTaskWithPattern()
        {
            InitTaskBuilderMock(new Dictionary<string, object> {{"CopyFileByPattern", "*.xml"}});

            LuaCopyTask sut = new LuaCopyTask(_taskBuilderMock, Source, Target);
            sut.pattern("*.xml");

            _taskBuilderMock.Verify();
        }

        [TestMethod]
        public void GivenLuaCopyTask__WhenCallingPattern__ShouldReturnItself()
        {
            InitTaskBuilderMock(new Dictionary<string, object> {{"CopyFileByPattern", "*.xml"}});

            LuaCopyTask sut = new LuaCopyTask(_taskBuilderMock, Source, Target);
            LuaCopyTask actual = sut.pattern("*.xml");

            Assert.AreSame(sut, actual);
        }

        [TestMethod]
        public void GivenLuaCopyTask__WhenCallingRecursive__ShouldBuildTaskWithRecursive()
        {
            InitTaskBuilderMock(new Dictionary<string, object> {{"CopySubfolders", true}});

            LuaCopyTask sut = new LuaCopyTask(_taskBuilderMock, Source, Target);
            sut.recursive(true);

            _taskBuilderMock.Verify();
        }

        [TestMethod]
        public void GivenLuaCopyTask__WhenCallingRecursive__ShouldReturnItself()
        {
            InitTaskBuilderMock(new Dictionary<string, object> {{"CopySubfolders", true}});

            LuaCopyTask sut = new LuaCopyTask(_taskBuilderMock, Source, Target);
            LuaCopyTask actual = sut.recursive(true);

            Assert.AreSame(sut, actual);
        }

        private void InitTaskBuilderMock(Dictionary<string, object> expectedConfig = null)
        {
            Dictionary<string, object> expectedEntries = new Dictionary<string, object>
            {
                {"CopyFromPath", Source},
                {"CopyToPath", Target}
            };

            expectedConfig?.ToList().ForEach(pair => expectedEntries.Add(pair.Key, pair.Value));
            _taskBuilderMock = new TaskBuilderMock(expectedEntries);
        }
    }
}