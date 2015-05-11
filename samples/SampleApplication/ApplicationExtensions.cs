using Compose;
using System.Threading;

namespace SampleApplication
{
	public static class ApplicationExtensions
	{
		public static void UseSampleApplication(this Executable<CancellationToken, CancellationToken> app)
		{
			app.OnExecute<Process>((process, cancellationToken) => {
                process.DoWork(cancellationToken);
                return cancellationToken;
            });
		}
	}
}
