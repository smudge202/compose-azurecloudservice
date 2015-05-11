using Compose.Logging;
using Microsoft.Framework.DependencyInjection;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Net;
using System.Threading;

namespace Compose.AzureCloudService
{
    public abstract class WorkerRole : RoleEntryPoint
    {
        private readonly ServiceApplication _app;
        private readonly ILogger _logger;
        private readonly ManualResetEventSlim _complete;
        private readonly CancellationTokenSource _tokenSource;
        protected ILogger Log {  get { return _logger; } }

        public WorkerRole()
        {
            _app = new ServiceApplication();
            _app.Name = ApplicationName;
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
                    Log.Exception(LoggingLevel.Error, ex, $"Exception occurred during `{nameof(AddServices)}`");
                    throw;
                }
            });
            _complete = new ManualResetEventSlim(false);
            _tokenSource = new CancellationTokenSource();
        }

        protected abstract string ApplicationName { get; }

        protected abstract void AddServices(IServiceCollection services);

        protected abstract void UseApplication(ServiceApplication app);

        public override bool OnStart()
        {
            Log.Message(LoggingLevel.Verbose, $"{_app.Name} is starting...");
            ServicePointManager.DefaultConnectionLimit = 12;
            try
            {
                UseApplication(_app);
            }
            catch (Exception ex)
            {
                Log.Exception(LoggingLevel.Error, ex, $"Exception occurred during `{nameof(UseApplication)}`");
                return false;
            }
            Log.Message(LoggingLevel.Information, $"{_app.Name} has started.");
            return base.OnStart();
        }

        public override void Run()
        {
            Log.Message(LoggingLevel.Information, $"{_app.Name} is running...");
            try
            {
                _app.Execute(_tokenSource.Token);
                if (!_tokenSource.IsCancellationRequested)
                    Log.Message(LoggingLevel.Verbose, $"{_app.Name} completed, but Cancellation has not been requested.");
            }
            catch (Exception ex)
            {
                Log.Exception(LoggingLevel.Error, ex, $"Exception occurred executing {_app.Name}");
                throw;
            }
            finally
            {
                _complete.Set();
            }
        }

        public override void OnStop()
        {
            Log.Message(LoggingLevel.Information, $"{_app.Name} is stopping...");
            _tokenSource.Cancel();
            _complete.Wait();
            Log.Message(LoggingLevel.Verbose, $"{_app.Name} has stopped.");
        }
    }
}