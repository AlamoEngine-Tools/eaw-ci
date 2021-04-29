using System;
using System.Runtime.InteropServices;

namespace EawXBuild.Native
{
    public class WinFileLinker : IFileLinker
    {
        public void CreateLink(string source, string target)
        {
            string platformSpecificSourcePath = source.Replace("/", "\\");
            string platformSpecificTargetPath = target.Replace("/", "\\");
            CreateHardLink(platformSpecificTargetPath, platformSpecificSourcePath, IntPtr.Zero);
        }

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern bool CreateHardLink(
            string lpFileName,
            string lpExistingFileName,
            IntPtr lpSecurityAttributes
        );
    }
}