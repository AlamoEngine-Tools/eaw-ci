using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using EawXBuild.Services.Process;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Services.Process {
    [TestClass]
    public class ProcessRunnerTest {
        
        [TestMethod]
        public void GivenEcho__WhenStarting__ShouldExitWithCodeZero() {
            var sut = new ProcessRunner();
            
            sut.Start("echo");
            sut.WaitForExit();

            var actual = sut.ExitCode;
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void GivenEchoWithArgs__WhenStarting__ShouldPrintOutArgs() {
            var stringBuilder = new StringBuilder();
            Console.SetOut(new StringWriter(stringBuilder));

            var sut = new ProcessRunner();

            const string expected = "Hello World";
            sut.Start("echo", expected);
            sut.WaitForExit();

            var actual = stringBuilder.ToString().Trim();
            Assert.AreEqual(expected, actual);
        }
        
        [TestMethod]
        public void GivenProcessStartInfoForEcho__WhenStarting__ShouldPrintOutArgs() {
            var stringBuilder = new StringBuilder();
            Console.SetOut(new StringWriter(stringBuilder));

            var sut = new ProcessRunner();

            const string expected = "Hello World";
            var startInfo = new ProcessStartInfo {
                FileName = "echo",
                Arguments = expected
            };

            sut.Start(startInfo);
            sut.WaitForExit();

            var actual = stringBuilder.ToString().Trim();
            Assert.AreEqual(expected, actual);
        }

    }
}