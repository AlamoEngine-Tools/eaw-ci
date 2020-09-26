using System.IO.Abstractions;

namespace EawXBuild.Tasks {
    public class CopyPolicy : ICopyPolicy {
        public void CopyTo(IFileInfo source, IFileInfo target, bool overwrite) {
            source.CopyTo(target.FullName, overwrite);
        }
    }
}