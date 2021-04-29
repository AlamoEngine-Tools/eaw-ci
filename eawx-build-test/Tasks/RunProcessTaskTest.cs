using System;
using System.IO.Abstractions.TestingHelpers;
using EawXBuild.Exceptions;
using EawXBuild.Tasks;
using EawXBuildTest.Services.Process;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Tasks
{
    [TestClass]
    public class RunProcessTaskTest
    {
        private string _executablePath;
        private MockFileSystem _filesystem;
        private ProcessRunnerSpy _runner;
        private RunProcessTask _sut;

        [TestInitialize]
        public void Setup()
        {
            _runner = new ProcessRunnerSpy();
            _filesystem = new MockFileSystem();
            _filesystem.AddFile("myProgram.exe", new MockFileData(string.Empty));
            _executablePath = "myProgram.exe";
            _sut = new RunProcessTask(_runner, _filesystem) {ExecutablePath = _executablePath};
        }

        [TestMethod]
        public void GivenPathToExecutable__WhenCallingRun__ShouldStartProcess()
        {
            _sut.Run();

            AssertProcessWasStartedWithExecutable(_runner, _executablePath);
        }


        [TestMethod]
        public void GivenExecutablePathAndArguments__WhenCallingRun__ShouldStartProcessWithArguments()
        {
            const string arguments = "--first --second --third";
            _sut.Arguments = arguments;

            _sut.Run();

            Assert.AreEqual(arguments, _runner.Arguments);
        }

        [TestMethod]
        public void GivenNoWorkingDirectory__WhenCallingRun__ShouldStartProcessInCurrentWorkingDirectory()
        {
            _sut.Run();
            Assert.AreEqual(Environment.CurrentDirectory, _runner.WorkingDirectory);
        }

        [TestMethod]
        public void
            GivenPathToExecutable_Arguments_And_WorkingDirectory__WhenCallingRun__ShouldStartProcessWithGivenConfig()
        {
            const string arguments = "--first --second --third";
            const string workingDir = "working/directory";
            _sut.Arguments = arguments;
            _sut.WorkingDirectory = workingDir;

            _sut.Run();

            AssertProcessWasStartedWithExecutable(_runner, _executablePath);
            Assert.AreEqual(arguments, _runner.Arguments);
            Assert.AreEqual(workingDir, _runner.WorkingDirectory);
        }

        [TestMethod]
        [ExpectedException(typeof(ProcessFailedException))]
        public void GivenExecutableThatExitsWithCodeOne__WhenCallingRun__ShouldThrowProcessFailedException()
        {
            ProcessRunnerStub runner = new ProcessRunnerStub {ExitCode = 1};
            RunProcessTask sut = new RunProcessTask(runner, _filesystem) {ExecutablePath = _executablePath};

            sut.Run();
        }

        [TestMethod]
        public void
            GivenPathToExecutable__WhenCallingRun__ShouldCallStartFirst_Then_BlockUntilFinished_Then_CheckExitCode()
        {
            CallOrderVerifyingProcessRunnerMock runner = new CallOrderVerifyingProcessRunnerMock();

            const string executablePath = "myProgram.exe";
            RunProcessTask sut = new RunProcessTask(runner, _filesystem) {ExecutablePath = executablePath};

            sut.Run();

            runner.Verify();
        }

        [TestMethod]
        public void GivenFailingProcessButAllowedToFail__WhenCallingRun__ShouldNotThrowProcessFailedException()
        {
            ProcessRunnerStub runner = new ProcessRunnerStub {ExitCode = 1};
            RunProcessTask sut = new RunProcessTask(runner, _filesystem)
                {ExecutablePath = _executablePath, AllowedToFail = true};

            sut.Run();
        }

        [TestMethod]
        [ExpectedException(typeof(NoRelativePathException))]
        public void GivenAbsolutePath__WhenCallingRun__ShouldThrowNoRelativePathException()
        {
            _sut.ExecutablePath = "/absolute/path";

            _sut.Run();
        }

        [TestMethod]
        public void GivenAbsolutePath__WhenCallingRun__ShouldNotStartProcess()
        {
            _sut.ExecutablePath = "/absolute/path";
            Assert.ThrowsException<NoRelativePathException>(() => _sut.Run());
            Assert.IsFalse(_runner.WasStarted);
        }

        private static void AssertProcessWasStartedWithExecutable(ProcessRunnerSpy runner, string executablePath)
        {
            Assert.IsTrue(runner.WasStarted, "Should have called Start(), but didn't");
            Assert.AreEqual(executablePath, runner.ExecutablePath,
                $"Should have called Start() with {executablePath}, but was {runner.ExecutablePath}");
        }
    }
}