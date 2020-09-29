using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EawXBuild.Services.Process {
    public class ProcessRunner : IProcessRunner {
        private System.Diagnostics.Process _process;

        public void Start(string executablePath) {
            Start(executablePath, null);
        }

        public void Start(string executablePath, string arguments) {
            var startInfo = new ProcessStartInfo {
                FileName = executablePath,
                WorkingDirectory = System.Environment.CurrentDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                Arguments = arguments
            };

            Start(startInfo);
        }

        public void Start(ProcessStartInfo startInfo) {
            _process = new System.Diagnostics.Process {
                StartInfo = startInfo
            };

            _process.Start();
            Console.Out.WriteLine(_process.StandardOutput.ReadToEnd());
        }

        public void WaitForExit() {
            _process.WaitForExit();
        }

        public int ExitCode => _process.ExitCode;
    }
}