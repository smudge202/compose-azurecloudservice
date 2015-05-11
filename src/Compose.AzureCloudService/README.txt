Compose.AzureCloudService
========================

Adapter to allow Azure Cloud Services to easily utilise Compose.

Usage
-----

The simplest and most conventional way of including Compose based
services, is to inherit from the provided WorkerRole:

``````````````````````````````````````````````````
public class WorkerRole : Compose.AzureCloudService.WorkerRole
{
    protected override string ApplicationName { get; } = "Sample Application";

    protected override void AddServices(IServiceCollection services)
    {
        services.AddSampleService();
    }

    protected override void UseApplication(ServiceApplication app)
    {
        app.UseSampleApplication();
    }
}
``````````````````````````````````````````````````

Application Extensions
----------------------

The following is an example of how your application can provide
an application extension:

``````````````````````````````````````````````````
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
``````````````````````````````````````````````````

Help / Issues
-------------

If you have any problems, please raise an issue on GitHub!

https://github.com/Smudge202/Compose/issues