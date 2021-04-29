using System.Runtime.InteropServices;
using System.Text;

namespace EawXBuild.Native
{
    public class MacOSFileLinker : IFileLinker
    {
        public void CreateLink(string source, string target)
        {
            string platformSourcePath = source.Replace("\\", "/");
            string platformTargetPath = target.Replace("\\", "/");
            byte[] sourceBytes = Encoding.UTF8.GetBytes(platformSourcePath);
            byte[] targetBytes = Encoding.UTF8.GetBytes(platformTargetPath);
            link(sourceBytes, targetBytes);
        }

        [DllImport("libSystem.dylib", CharSet = CharSet.Unicode)]
        private static extern int link(
            byte[] lpFileName,
            byte[] lpExistingFileName
        );
    }
}