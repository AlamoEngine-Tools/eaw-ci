using System;
using System.Runtime.InteropServices;

namespace EawXBuild.Native {
    public class FileLinkerFactory : IFileLinkerFactory {
        public IFileLinker MakeFileLinker() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return new MacOSFileLinker();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return new WinFileLinker();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return new LinuxFileLinker();

            throw new InvalidOperationException("This Operating System is not supported");
        }
    }
}