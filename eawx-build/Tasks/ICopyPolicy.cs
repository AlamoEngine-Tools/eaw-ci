using System.IO.Abstractions;

namespace EawXBuild.Tasks {
    public interface ICopyPolicy {
        public void CopyTo(IFileInfo source, IFileInfo target, bool overwrite);
    }
}