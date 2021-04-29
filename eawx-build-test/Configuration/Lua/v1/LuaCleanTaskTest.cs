using System.Collections.Generic;
using EawXBuild.Configuration.Lua.v1;
using EawXBuildTest.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Configuration.Lua.v1
{
    [TestClass]
    public class LuaCleanTaskTest
    {
        [TestMethod]
        public void GivenLuaCleanTask__ShouldCallTaskBuilderWithPath()
        {
            const string path = "folder/file";
            TaskBuilderMock taskBuilderMock = new TaskBuilderMock(new Dictionary<string, object>
            {
                {"Path", path}
            });

            LuaCleanTask sut = new LuaCleanTask(taskBuilderMock, path);

            taskBuilderMock.Verify();
        }
    }
}