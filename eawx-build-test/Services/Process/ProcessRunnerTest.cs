using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using EawXBuild.Services.Process;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Services.Process {
    [TestClass]
    public class ProcessRunnerTest {
        
        [PlatformSpecificTestMethod("Linux", "OSX")]
        public void GivenEcho__WhenStarting__ShouldExitWithCodeZero() {
            var sut = new ProcessRunner();

            sut.Start("echo");
            sut.WaitForExit();

            var actual = sut.ExitCode;
            Assert.AreEqual(0, actual);
        }

        [PlatformSpecificTestMethod("Linux", "OSX")]
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

        [PlatformSpecificTestMethod("Linux", "OSX")]
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
        
        [PlatformSpecificTestMethod("Windows")]
        public void GivenCmdNoCommand__WhenStarting__ShouldExitWithCodeZero() {
            var sut = new ProcessRunner();

            sut.Start("cmd.exe", "/c");
            sut.WaitForExit();

            var actual = sut.ExitCode;
            Assert.AreEqual(0, actual);
        }

        [PlatformSpecificTestMethod("Windows")]
        public void GivenCmdWithEchoCommandAndArgs__WhenStarting__ShouldPrintOutArgs() {
            var stringBuilder = new StringBuilder();
            Console.SetOut(new StringWriter(stringBuilder));

            var sut = new ProcessRunner();

            const string expected = "Hello World";
            sut.Start("cmd.exe", "/c echo " + expected);
            sut.WaitForExit();

            var actual = stringBuilder.ToString().Trim();
            Assert.AreEqual(expected, actual);
        }

        [PlatformSpecificTestMethod("Windows")]
        public void GivenProcessStartInfoForCmdWithEcho__WhenStarting__ShouldPrintOutArgs() {
            var stringBuilder = new StringBuilder();
            Console.SetOut(new StringWriter(stringBuilder));

            var sut = new ProcessRunner();

            const string expected = "Hello World";
            var startInfo = new ProcessStartInfo {
                FileName = "cmd.exe",
                Arguments = "/c echo " + expected
            };

            sut.Start(startInfo);
            sut.WaitForExit();

            var actual = stringBuilder.ToString().Trim();
            Assert.AreEqual(expected, actual);
        }
    }
}