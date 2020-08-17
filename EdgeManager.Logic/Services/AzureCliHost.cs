using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using EdgeManager.Interfaces.Extensions;
using EdgeManager.Interfaces.Logging;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;
using EdgeManager.Interfaces.Settings;
using log4net;
using Newtonsoft.Json;

namespace EdgeManager.Logic.Services
{
    class AzureCliHost : IAzureCli, IAzureService, IDisposable
	{
        private readonly IPowerShellService powerShellService;
        private readonly ApplicationSettings settings;
        private readonly ILog logger = LoggerFactory.GetLogger(typeof(AzureCliHost));        
        private Subject<JsonCommand> jsonCommands = new Subject<JsonCommand>();
        private bool disposedValue;
        private CompositeDisposable disposables = new CompositeDisposable();

        public IObservable<JsonCommand> JsonCommands => jsonCommands;

        public AzureCliHost(IPowerShellService powerShellService, ApplicationSettings settings)
        {
            this.powerShellService = powerShellService;
            this.settings = settings;
        }

        public async Task<T> Run<T>(string command)
        {
            var json = await SendOrRestoreFromCache(command, false);
            return JsonConvert.DeserializeObject<T>(json);
        }

        private async Task<T> Run<T>(string command, bool reload)
        {
            var json = await SendOrRestoreFromCache(command, reload);
            return JsonConvert.DeserializeObject<T>(json);
		}

        private async Task<string> SendOrRestoreFromCache(string command, bool reload)
        {
            if (settings.CommandCache.ContainsKey(command) && !reload)
            {
                logger.Debug($"Restoring command '{command}' from cache");
                jsonCommands.OnNext(settings.CommandCache[command]);
                return settings.CommandCache[command].ResultJson;
            }

            logger.Debug($"Sended command to azure cloud: '{command}'");
            var json = string.Join("\n", await ExecutePowerShellCommand("az " + command));
            var jsonCommand = new JsonCommand(command, json);
            jsonCommands.OnNext(jsonCommand);
            settings.CommandCache[command] = jsonCommand;
            
            return json;
        }


		public async Task<IoTHubInfo[]> GetIoTHubs(bool reload = false)
        {
            var hubs = await Run<IoTHubInfo[]>("iot hub list", reload);
            settings.LastCheckedIoTHubs = hubs;
            return hubs;
        }

        public Task<IoTDeviceInfo[]> GetIoTDevices(string hubName, bool reload = false) => Run<IoTDeviceInfo[]>($"iot hub device-identity list --hub-name {hubName}", reload);
		public Task<IoTModuleIdentityInfo[]> GetIoTModules(string hubName, string deviceId, bool reload = false) => Run<IoTModuleIdentityInfo[]>
			($"iot hub module-identity list --device-id {deviceId} --hub-name {hubName}", reload);
        public Task<ModuleTwin> GetIoTModelTwinProperties(string hubName, string deviceId, string moduleId) => Run<ModuleTwin>
            ($"iot hub module-twin show --module-id {moduleId} --device-id {deviceId} --hub-name {hubName}");
		public Task<IoTDirectMethodReply> CallMethod(string method, string hubName, string deviceId, string moduleId, DirectMethodPayloadBase payload) => Run<IoTDirectMethodReply>
			($"iot hub invoke-module-method --method-name '{method}' -n '{hubName}' -d '{deviceId}' -m '{moduleId}' --method-payload '{JsonConvert.SerializeObject(payload, Newtonsoft.Json.Formatting.None)}'");

        public async Task Login(CancellationToken token)
        {
            await ExecutePowerShellCommand("az login");
        }
        private async Task<Collection<PSObject>> ExecutePowerShellCommand(string command)
        {
            var result = await powerShellService.Execute(command);

            return result;
        }

        public async Task Logout()
        {
            await ExecutePowerShellCommand("az logout");
        }

        public async Task<AzureAccountInfo> GetAccount()
        {
            var accountInfo = await Run<AzureAccountInfo>($"account show");
            if (accountInfo != null) settings.AzureAccountInfo = accountInfo;
            return accountInfo;
        }

        public async Task CreateNewDevice(string hubName, string newDeviceName)
        {
            await ExecutePowerShellCommand($"az iot hub device-identity create --device-id {newDeviceName} --hub-name {hubName} --edge-enabled");
        }
        public async Task DeleteSelectedDevice(string hubName, string deviceId)
        {
            await ExecutePowerShellCommand($"az iot hub device-identity delete --device-id {deviceId} --hub-name {hubName}");
        }
       
        public async Task<bool> CheckCli()
        {
            var result = await ExecutePowerShellCommand("az version");
            try
            {
                return result.Any(l => l.ToString().Contains("azure-cli"));
            }
            catch (Exception e)
            {
            }

            return false;
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
