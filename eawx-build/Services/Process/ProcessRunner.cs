using System;
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
                Arguments = arguments
            };

            Start(startInfo);
        }

        public void Start(ProcessStartInfo startInfo) {
            RedirectIOForProcessStartInfo(startInfo);
            _process = new System.Diagnostics.Process {
                StartInfo = startInfo
            };

            _process.Start();
            Console.Out.WriteLine(_process.StandardOutput.ReadToEnd());
        }


        public void WaitForExit() {
            _process.WaitForExit();
        }

        private static void RedirectIOForProcessStartInfo(ProcessStartInfo startInfo) {
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardInput = true;
        }

        public int ExitCode => _process.ExitCode;
    }
}