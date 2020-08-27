using EawXBuild.Native;

namespace EawXBuildTest.Native {
    public class FileLinkerSpy: IFileLinker {
        private bool _createLinkWasCalled;
        public string ReceivedSource { get; private set; } 
        public string ReceivedTarget { get; private set; }

        public bool CreateLinkWasCalled {
            get => _createLinkWasCalled;
            private set => _createLinkWasCalled = value;
        }

        public void CreateLink(string source, string target) {
            CreateLinkWasCalled = true;
            ReceivedSource = source;
            ReceivedTarget = target;
        }
    }
}