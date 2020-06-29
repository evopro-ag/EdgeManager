using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdgeManager.Interfaces.Logging;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;
using log4net;
using Newtonsoft.Json;

namespace EdgeManager.Logic.Services
{
    class AzureCliHost : IAzureCli, IAzureService
	{
        private readonly IPowerShell powerShell;
        private readonly ILog logger = LoggerFactory.GetLogger(typeof(AzureCliHost));
        List<IObserver<JsonCommand>> observers;

        public AzureCliHost(IPowerShell powerShell)
        {
            this.powerShell = powerShell;
            observers = new List<IObserver<JsonCommand>>();
        }

		public async Task<T> Run<T>(string command)
        {
			var json = string.Join("\n", await powerShell.Execute("az " + command));
			logger.Debug(command);
            JsonCommand jsonCommand = new JsonCommand(command);
            foreach (var observer in observers)
                observer.OnNext(jsonCommand);
            
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

        public IDisposable Subscribe(IObserver<JsonCommand> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);

            return new Unsubscriber(observers, observer);
        }
    }
}
