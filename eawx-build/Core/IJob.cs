using System.Threading.Tasks;

namespace EawXBuild.Core
{
    public interface IJob
    {
        string GetName();

        void Run();
    }
}