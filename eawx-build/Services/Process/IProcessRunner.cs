using System.Diagnostics;

namespace EawXBuild.Services.Process
{
    public interface IProcessRunner
    {
        int ExitCode { get; }
        void Start(string executablePath);
        void Start(string executablePath, string arguments);

        void Start(ProcessStartInfo startInfo);

        void WaitForExit();
    }
}