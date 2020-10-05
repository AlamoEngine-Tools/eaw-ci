using System;
using System.Runtime.InteropServices;

namespace EawXBuild.Native {
    public class WinFileLinker : IFileLinker {
        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern bool CreateHardLink(
            string lpFileName,
            string lpExistingFileName,
            IntPtr lpSecurityAttributes
        );


        public void CreateLink(string source, string target) {
            var platformSpecificSourcePath = source.Replace("/", "\\");
            var platformSpecificTargetPath = target.Replace("/", "\\");
            CreateHardLink(platformSpecificSourcePath, platformSpecificTargetPath, IntPtr.Zero);
        }
    }
}