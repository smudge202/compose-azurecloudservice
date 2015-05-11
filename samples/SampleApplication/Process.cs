using System.Threading;

namespace SampleApplication
{
    internal sealed class Process
    {
        private readonly IService _service;

        public Process(IService service)
        {
            _service = service;
        }

        public void DoWork(CancellationToken token)
        {
            _service.Work(token);
        }
    }
}
