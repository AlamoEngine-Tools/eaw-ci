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
            _process = new System.Diagnostics.Process {
                StartInfo = new ProcessStartInfo {
                    FileName = executablePath,
                    WorkingDirectory = System.Environment.CurrentDirectory,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    Arguments = arguments
                }
            };

            _process.Start();
            Console.WriteLine(_process.StandardOutput.ReadToEnd());
        }

        public void WaitForExit() {
            _process.WaitForExit();
        }
    }
}