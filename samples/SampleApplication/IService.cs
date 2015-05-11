using System.Threading;

namespace SampleApplication
{
    public interface IService
    {
        void Work(CancellationToken token);
    }
}
