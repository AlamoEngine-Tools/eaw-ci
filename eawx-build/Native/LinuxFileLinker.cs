using System.Runtime.InteropServices;
using System.Text;

namespace EawXBuild.Native
{
    public class LinuxFileLinker : IFileLinker
    {
        public void CreateLink(string source, string target)
        {
            string platformSourcePath = source.Replace("\\", "/");
            string platformTargetPath = target.Replace("\\", "/");
            byte[] sourceBytes = Encoding.UTF8.GetBytes(platformSourcePath);
            byte[] targetBytes = Encoding.UTF8.GetBytes(platformTargetPath);
            link(sourceBytes, targetBytes);
        }

        [DllImport("libc")]
        private static extern int link(byte[] oldpath, byte[] newpath);
    }
}