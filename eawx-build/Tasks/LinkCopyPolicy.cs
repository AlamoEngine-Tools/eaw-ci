using System.IO.Abstractions;
using EawXBuild.Native;

namespace EawXBuild.Tasks {
    public class LinkCopyPolicy : ICopyPolicy {
        private readonly IFileLinker _fileLinker;

        public LinkCopyPolicy(IFileLinker fileLinker) {
            _fileLinker = fileLinker;
        }

        public void CopyTo(IFileInfo source, IFileInfo target, bool overwrite) {
            if (target.Exists && !overwrite) return;
            _fileLinker.CreateLink(source.FullName, target.FullName);
        }
    }
}