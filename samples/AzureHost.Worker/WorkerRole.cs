using Compose.AzureCloudService;
using Microsoft.Framework.DependencyInjection;
using SampleApplication;
using SampleService;

namespace AzureHost.Worker
{
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
}
