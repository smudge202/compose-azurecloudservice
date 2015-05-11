using System;
using System.Threading;

namespace SampleService
{
    internal sealed class SomeService : SampleApplication.IService
    {
        public void Work(CancellationToken token)
        {
            token.WaitHandle.WaitOne(1000);
        }
    }
}
