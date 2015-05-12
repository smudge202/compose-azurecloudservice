using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Net;
using System.Threading;

namespace Compose.AzureCloudService
{
    public abstract class WorkerRole : RoleEntryPoint
    {
        private readonly ServiceApplication _app;
        private readonly ManualResetEventSlim _complete;
        private readonly CancellationTokenSource _tokenSource;

        protected ILogger Logger { get; private set; }

        public WorkerRole()
        {
            _app = new ServiceApplication();
            _app.Name = ApplicationName;
            Exception serviceException = null;
            _app.UseServices(services =>
            {
                services.AddLogging();
                services.AddOptions();
                try
                {
                    AddServices(services);
                }
                catch (Exception ex)
                {
                    serviceException = ex;
                }
            });
            _complete = new ManualResetEventSlim(false);
            _tokenSource = new CancellationTokenSource();

            Logger = _app.HostingServices.GetRequiredService<ILoggerFactory>().CreateLogger(_app.Name);
            if (serviceException == null) return;

            Logger.LogError($"Exception occurred during `{nameof(AddServices)}`", serviceException);
            throw serviceException;
        }

        protected abstract string ApplicationName { get; }

        protected abstract void AddServices(IServiceCollection services);

        protected abstract void UseApplication(ServiceApplication app);

        public override bool OnStart()
        {
            Logger.LogVerbose($"{_app.Name} is starting...");
            ServicePointManager.DefaultConnectionLimit = 12;
            try
            {
                UseApplication(_app);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception occurred during `{nameof(UseApplication)}`", ex);
                return false;
            }
            Logger.LogInformation($"{_app.Name} has started.");
            return base.OnStart();
        }

        public override void Run()
        {
            Logger.LogInformation($"{_app.Name} is running...");
            try
            {
                _app.Execute(_tokenSource.Token);
                if (!_tokenSource.IsCancellationRequested)
                    Logger.LogVerbose($"{_app.Name} completed, but Cancellation has not been requested.");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception occurred executing {_app.Name}", ex);
                throw;
            }
            finally
            {
                _complete.Set();
            }
        }

        public override void OnStop()
        {
            Logger.LogInformation($"{_app.Name} is stopping...");
            _tokenSource.Cancel();
            _complete.Wait();
            Logger.LogVerbose($"{_app.Name} has stopped.");
        }
    }
}