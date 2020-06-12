using EawXBuild.Services.Process;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest.Services.Process {
    public class ProcessRunnerDummy : IProcessRunner {
        public virtual void Start(string executablePath) {
        }

        public virtual void Start(string executablePath, string arguments) {
        }

        public virtual void WaitForExit() {
        }

        public virtual int ExitCode { get; set; }
    }

    public class ProcessRunnerStub : ProcessRunnerDummy {

        public override int ExitCode { get; set; }
    }

    public class ProcessRunnerSpy : ProcessRunnerDummy {
        public bool WasStarted { get; private set; }

        public string ExecutablePath { get; private set; }
        
        public string Arguments { get; private set; }

        public override void Start(string executablePath) {
            WasStarted = true;
            ExecutablePath = executablePath;
        }
        
        public override void Start(string executablePath, string arguments) {
            Start(executablePath);
            Arguments = arguments;
        }

        public override void WaitForExit() {
        }
    }

    public class CallOrderVerifyingProcessRunnerMock : ProcessRunnerSpy {
        private string _callOrder = "";

        public override void Start(string executablePath) {
            _callOrder += "s";
            base.Start(executablePath);
        }

        public override void WaitForExit() {
            _callOrder += "w";
            base.WaitForExit();
        }

        public override int ExitCode {
            get {
                _callOrder += "e";
                return 0;
            }
            set {}
        }

        public void Verify() {
            const string expected = "swe";
            Assert.AreEqual(expected, _callOrder);
        }
    }
}