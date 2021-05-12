using System.Collections.Generic;
using EawXBuild.Configuration.Lua.v1;
using EawXBuildTest.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Configuration.Lua.v1
{
    [TestClass]
    public class LuaRunProcessTaskTest
    {
        private const string Path = "echo";
        private TaskBuilderMock _taskBuilderMock;

        [TestInitialize]
        public void SetUp()
        {
            _taskBuilderMock = new TaskBuilderMock(new Dictionary<string, object>
            {
                {"ExecutablePath", Path}
            });
        }

        [TestMethod]
        public void GivenLuaRunProcessTask__ShouldCallTaskBuilderWithExecutablePath()
        {
            LuaRunProcessTask sut = new LuaRunProcessTask(_taskBuilderMock, Path);

            _taskBuilderMock.Verify();
        }

        [TestMethod]
        public void GivenLuaRunProcessTask__WhenCallingArguments__ShouldCallTaskBuilderWithArguments()
        {
            const string args = "hello world";
            _taskBuilderMock.AddExpectedEntry("Arguments", args);

            LuaRunProcessTask sut = new LuaRunProcessTask(_taskBuilderMock, Path);
            sut.arguments(args);

            _taskBuilderMock.Verify();
        }

        [TestMethod]
        public void GivenLuaRunProcessTask__WhenCallingArguments__ShouldReturnItself()
        {
            const string args = "hello world";

            LuaRunProcessTask sut = new LuaRunProcessTask(_taskBuilderMock, Path);
            LuaRunProcessTask actual = sut.arguments(args);

            Assert.AreSame(sut, actual);
        }

        [TestMethod]
        public void GivenLuaRunProcessTask__WhenCallingWorkingDirectory__ShouldCallTaskBuilderWithWorkingDirectory()
        {
            const string workingDirectory = "another/dir";
            _taskBuilderMock.AddExpectedEntry("WorkingDirectory", workingDirectory);

            LuaRunProcessTask sut = new LuaRunProcessTask(_taskBuilderMock, Path);
            sut.working_directory(workingDirectory);

            _taskBuilderMock.Verify();
        }

        [TestMethod]
        public void GivenLuaRunProcessTask__WhenCallingWorkingDirectory__ShouldReturnItself()
        {
            const string workingDirectory = "another/dir";

            LuaRunProcessTask sut = new LuaRunProcessTask(_taskBuilderMock, Path);
            LuaRunProcessTask actual = sut.working_directory(workingDirectory);

            Assert.AreSame(sut, actual);
        }

        [TestMethod]
        public void GivenLuaRunProcessTask__WhenCallingAllowedToFail__ShouldCallTaskBuilderWithWorkingDirectory()
        {
            const bool allowedToFail = true;
            _taskBuilderMock.AddExpectedEntry("AllowedToFail", allowedToFail);

            LuaRunProcessTask sut = new LuaRunProcessTask(_taskBuilderMock, Path);
            sut.allowed_to_fail(allowedToFail);

            _taskBuilderMock.Verify();
        }

        [TestMethod]
        public void GivenLuaRunProcessTask__WhenCallingAllowedToFail__ShouldReturnItself()
        {
            const bool allowedToFail = true;

            LuaRunProcessTask sut = new LuaRunProcessTask(_taskBuilderMock, Path);
            LuaRunProcessTask actual = sut.allowed_to_fail(allowedToFail);

            Assert.AreSame(sut, actual);
        }
    }
}