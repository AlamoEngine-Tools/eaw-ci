using System;
using EawXBuild.Configuration.v1;
using EawXBuild.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Configuration.v1 {
    [TestClass]
    public class RunProcessTaskBuilderTest {
        [TestMethod]
        public void WhenBuildingRunProcessTaskWithExePathAndArguments__ShouldReturnConfiguredTask() {
            var sut = new RunProcessTaskBuilder();

            const string pathToExe = "Path/To/Exe";
            const string args = "--first=5 --second=yes";
            var task = (RunProcessTask) sut.With("ExecutablePath", pathToExe)
                .With("Arguments", args)
                .Build();
            
            Assert.AreEqual(pathToExe, task.ExecutablePath);
            Assert.AreEqual(args, task.Arguments);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WhenBuildingWithUnknownConfigOption__ShouldThrowInvalidOperationException() {
            var sut = new RunProcessTaskBuilder();

            sut.With("UnknownOption", string.Empty);
        }
    }
}