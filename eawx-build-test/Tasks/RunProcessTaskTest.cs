using System.IO;
using System.IO.Abstractions.TestingHelpers;
using EawXBuild.Exceptions;
using EawXBuild.Tasks;
using EawXBuildTest.Services.Process;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Tasks {
    [TestClass]
    public class RunProcessTaskTest {
        private RunProcessTask _sut;
        private ProcessRunnerSpy _runner;
        private MockFileSystem _filesystem;
        private string _executablePath;

        [TestInitialize]
        public void Setup() {
            _runner = new ProcessRunnerSpy();
            _filesystem = new MockFileSystem();
            _filesystem.AddFile("myProgram.exe", new MockFileData(string.Empty));
            _executablePath = "myProgram.exe";
            _sut = new RunProcessTask(_runner, _filesystem) {ExecutablePath = _executablePath};
        }

        [TestMethod]
        public void GivenPathToExecutable__WhenCallingRun__ShouldStartProcess() {
            _sut.Run();

            AssertProcessWasStartedWithExecutable(_runner, _executablePath);
        }


        [TestMethod]
        public void GivenExecutablePathAndArguments__WhenCallingRun__ShouldStartProcessWithArguments() {
            const string arguments = "--first --second --third";
            _sut.Arguments = arguments;

            _sut.Run();
            
            Assert.AreEqual(arguments, _runner.Arguments);
        }

        [TestMethod]
        [ExpectedException(typeof(ProcessFailedException))]
        public void GivenExecutableThatExitsWithCodeOne__WhenCallingRun__ShouldThrowProcessFailedException() {
            var runner = new ProcessRunnerStub {ExitCode = 1};
            var sut = new RunProcessTask(runner, _filesystem) { ExecutablePath = _executablePath };

            sut.Run();
        }
        
        [TestMethod]
        public void GivenPathToExecutable__WhenCallingRun__ShouldCallStartFirst_Then_BlockUntilFinished_Then_CheckExitCode() {
            var runner = new CallOrderVerifyingProcessRunnerMock();

            const string executablePath = "myProgram.exe";
            RunProcessTask sut = new RunProcessTask(runner, _filesystem) {ExecutablePath = executablePath};

            sut.Run();

            runner.Verify();
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void GivenPathToNonExistingExecutable__WhenCallingRun__ShouldThrowFileNotFoundException() {
            var sut = new RunProcessTask(new ProcessRunnerDummy(), _filesystem) {
                ExecutablePath = "NonExistingProgram.exe"
            };

            sut.Run();
        }

        private static void AssertProcessWasStartedWithExecutable(ProcessRunnerSpy runner, string executablePath) {
            Assert.IsTrue(runner.WasStarted, "Should have called Start(), but didn't");
            Assert.AreEqual(executablePath, runner.ExecutablePath,
                $"Should have called Start() with {executablePath}, but was {runner.ExecutablePath}");
        }
    }
}