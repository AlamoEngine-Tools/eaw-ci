using System.IO.Abstractions;
using EawXBuild.Tasks;

namespace EawXBuildTest.Tasks {
    public class CopyPolicyDummy : ICopyPolicy {
        public void CopyTo(IFileInfo source, IFileInfo target, bool overwrite) { }
    }

    public class CopyPolicySpy : ICopyPolicy {
        public bool CopyCalled { get; private set; }

        public void CopyTo(IFileInfo source, IFileInfo target, bool overwrite) {
            CopyCalled = true;
        }
    }

    public class CopyPolicyFake : ICopyPolicy {
        public void CopyTo(IFileInfo source, IFileInfo target, bool overwrite) {
            source.CopyTo(target.FullName, overwrite);
        }
    }
}