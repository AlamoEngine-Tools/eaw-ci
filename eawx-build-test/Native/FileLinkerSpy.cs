using EawXBuild.Native;

namespace EawXBuildTest.Native
{
    public class FileLinkerSpy : IFileLinker
    {
        public string ReceivedSource { get; private set; }
        public string ReceivedTarget { get; private set; }

        public bool CreateLinkWasCalled { get; private set; }

        public void CreateLink(string source, string target)
        {
            CreateLinkWasCalled = true;
            ReceivedSource = source;
            ReceivedTarget = target;
        }
    }
}