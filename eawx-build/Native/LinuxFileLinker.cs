using System.Runtime.InteropServices;
using System.Text;

namespace EawXBuild.Native {
    public class LinuxFileLinker : IFileLinker {
        public void CreateLink(string source, string target) {
            byte[] sourceBytes = Encoding.UTF8.GetBytes(source);
            byte[] targetBytes = Encoding.UTF8.GetBytes(target);
            link(sourceBytes, targetBytes);
        }

        [DllImport("libc")]
        private static extern int link(byte[] oldpath, byte[] newpath);
    }
}