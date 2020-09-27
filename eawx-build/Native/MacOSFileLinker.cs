using System.Runtime.InteropServices;
using System.Text;

namespace EawXBuild.Native {
    public class MacOSFileLinker : IFileLinker {
        public void CreateLink(string source, string target) {
            byte[] sourceBytes = Encoding.UTF8.GetBytes(source);
            byte[] targetBytes = Encoding.UTF8.GetBytes(target);
            link(sourceBytes, targetBytes);
        }
        
        [DllImport("libSystem.dylib", CharSet = CharSet.Unicode)]
        private static extern int link(
            byte[] lpFileName,
            byte[] lpExistingFileName
        );
    }
}