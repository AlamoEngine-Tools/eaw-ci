using System.Collections.Generic;
using System.Linq;
using EawXBuild.Configuration.Lua.v1;
using EawXBuildTest.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Configuration.Lua.v1 {
    [TestClass]
    public class LuaCopyTaskTest {
        private TaskBuilderMock _taskBuilderMock;
        private const string Source = "original.txt";
        private const string Target = "copy.txt";


        [TestMethod]
        public void GivenLuaCopyTaskWithSourceAndDestination__ShouldBuildTaskWithCorrectSettings() {
            InitTaskBuilderMock();

            var sut = new LuaCopyTask(_taskBuilderMock, "original.txt", "copy.txt");

            _taskBuilderMock.Verify();
        }

        [TestMethod]
        public void GivenLuaCopyTask__WhenCallingOverwrite__ShouldBuildTaskWithOverwrite() {
            InitTaskBuilderMock(new Dictionary<string, object> {{"AlwaysOverwrite", true}});
            
            var sut = new LuaCopyTask(_taskBuilderMock, Source, Target);
            sut.overwrite(true);

            _taskBuilderMock.Verify();
        }

        [TestMethod]
        public void GivenLuaCopyTask__WhenCallingOverwrite__ShouldReturnItself() {
            InitTaskBuilderMock(new Dictionary<string, object> {{"AlwaysOverwrite", true}});

            var sut = new LuaCopyTask(_taskBuilderMock, Source, Target);
            var actual = sut.overwrite(true);

            Assert.AreSame(sut, actual);
        }

        [TestMethod]
        public void GivenLuaCopyTask__WhenCallingPattern__ShouldBuildTaskWithPattern() {
            InitTaskBuilderMock(new Dictionary<string, object> {{"CopyFileByPattern", "*.xml"}});
            
            var sut = new LuaCopyTask(_taskBuilderMock, Source, Target);
            sut.pattern("*.xml");

            _taskBuilderMock.Verify();
        }
        
        [TestMethod]
        public void GivenLuaCopyTask__WhenCallingPattern__ShouldReturnItself() {
            InitTaskBuilderMock(new Dictionary<string, object> {{"CopyFileByPattern", "*.xml"}});

            var sut = new LuaCopyTask(_taskBuilderMock, Source, Target);
            var actual = sut.pattern("*.xml");

            Assert.AreSame(sut, actual);
        }
        
        [TestMethod]
        public void GivenLuaCopyTask__WhenCallingRecursive__ShouldBuildTaskWithRecursive() {
            InitTaskBuilderMock(new Dictionary<string, object> {{"CopySubfolders", true}});
            
            var sut = new LuaCopyTask(_taskBuilderMock, Source, Target);
            sut.recursive(true);

            _taskBuilderMock.Verify();
        }
        
        [TestMethod]
        public void GivenLuaCopyTask__WhenCallingRecursive__ShouldReturnItself() {
            InitTaskBuilderMock(new Dictionary<string, object> {{"CopySubfolders", true}});

            var sut = new LuaCopyTask(_taskBuilderMock, Source, Target);
            var actual = sut.recursive(true);

            Assert.AreSame(sut, actual);
        }

        private void InitTaskBuilderMock(Dictionary<string, object> expectedConfig = null) {
            var expectedEntries = new Dictionary<string, object> {
                {"CopyFromPath", Source},
                {"CopyToPath", Target}
            };

            expectedConfig?.ToList().ForEach(pair => expectedEntries.Add(pair.Key, pair.Value));
            _taskBuilderMock = new TaskBuilderMock(expectedEntries);
        }
    }
}