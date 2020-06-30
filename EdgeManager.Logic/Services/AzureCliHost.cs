using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using EdgeManager.Interfaces.Extensions;
using EdgeManager.Interfaces.Logging;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;
using log4net;
using Newtonsoft.Json;

namespace EdgeManager.Logic.Services
{
    class AzureCliHost : IAzureCli, IAzureService, IDisposable
	{
        private readonly IPowerShell powerShell;
        private readonly ILog logger = LoggerFactory.GetLogger(typeof(AzureCliHost));        
        private Subject<JsonCommand> jsonCommands = new Subject<JsonCommand>();
        private bool disposedValue;
        private CompositeDisposable disposables = new CompositeDisposable();

        public IObservable<JsonCommand> JsonCommands => jsonCommands;

        public AzureCliHost(IPowerShell powerShell)
        {
            this.powerShell = powerShell;
            jsonCommands.AddDisposableTo(disposables);
        }

		public async Task<T> Run<T>(string command)
        {
			var json = string.Join("\n", await powerShell.Execute("az " + command));
			logger.Debug(command);
            var jsonCommand = new JsonCommand(command);
            jsonCommands.OnNext(jsonCommand);
            
            return JsonConvert.DeserializeObject<T>(json);
		}

		public Task<IoTHubInfo[]> GetIoTHubs() => Run<IoTHubInfo[]>("iot hub list");
		public Task<IoTDeviceInfo[]> GetIoTDevices(string hubName) => Run<IoTDeviceInfo[]>($"iot hub device-identity list --hub-name {hubName}");
		public Task<IoTModuleIdentityInfo[]> GetIoTModules(string hubName, string deviceId) => Run<IoTModuleIdentityInfo[]>
			($"iot hub module-identity list --device-id {deviceId} --hub-name {hubName}");
		public Task<IoTDirectMethodReply> CallMethod(string method, string hubName, string deviceId, string moduleId, DirectMethodPayloadBase payload) => Run<IoTDirectMethodReply>
			($"iot hub invoke-module-method --method-name '{method}' -n '{hubName}' -d '{deviceId}' -m '{moduleId}' --method-payload '{JsonConvert.SerializeObject(payload, Newtonsoft.Json.Formatting.None)}'");

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<JsonCommand>> _observers;
            private IObserver<JsonCommand> _observer;

            public Unsubscriber(List<IObserver<JsonCommand>> observers, IObserver<JsonCommand> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (!(_observer == null)) _observers.Remove(_observer);
            }
        }

       

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    disposables.Dispose();
                }
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~AzureCliHost()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
