using System.Collections.Generic;

namespace EawXBuild.Services.Process {
    public interface IProcessRunner {
        void Start(string executablePath);
        void Start(string executablePath, string arguments);

        void WaitForExit();
    }
}