using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Reflection;
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


        private Dictionary<string, JsonCommand> cache = new Dictionary<string, JsonCommand>();

        public AzureCliHost(IPowerShell powerShell)
        {
            this.powerShell = powerShell;

            RestoreCache();
        }

        public async Task<T> Run<T>(string command)
        {
            var json = await SendOrRestoreFromCache(command);
            return JsonConvert.DeserializeObject<T>(json);
        }

        private async Task<T> Run<T>(string command, bool reload = false)
        {
            var json = await SendOrRestoreFromCache(command, reload);
            return JsonConvert.DeserializeObject<T>(json);
		}

        private void RestoreCache()
        {
            try
            {
                var currentDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                var filename = "cache.json";
                var filepath = Path.Combine(currentDirectory, filename);

                logger.Debug($"restoring cache from file '{filepath}'");

                var text = File.ReadAllText(Path.Combine(currentDirectory, filename));

                cache = JsonConvert.DeserializeObject<Dictionary<string, JsonCommand>>(text);
            }
            catch(Exception e)
            {
                logger.Error("error while restoring cache", e);
            }
        }

        private void SaveCache()
        {
            try
            {
                var currentDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                var filename = "cache.json";

                var text = JsonConvert.SerializeObject(cache, Formatting.Indented);
                var filepath = Path.Combine(currentDirectory, filename);

                logger.Debug($"saving cache into file '{filepath}'");

                File.WriteAllText(filepath, text);
            }
            catch (Exception e)
            {
                logger.Error("error while saving cache", e);
            }
}

        private async Task<string> SendOrRestoreFromCache(string command, bool reload = false)
        {
            if (cache.ContainsKey(command) && !reload)
            {
                logger.Debug($"restoring command '{command}' from cache");
                jsonCommands.OnNext(cache[command]);
                return cache[command].ResultJson;
            }

            logger.Debug($"Sended command to azure cloud: {command}");
            var json = string.Join("\n", await powerShell.Execute("az " + command));
            var jsonCommand = new JsonCommand(command, json);
            jsonCommands.OnNext(jsonCommand);
            cache[command] = jsonCommand;
            
            return json;
        }


		public Task<IoTHubInfo[]> GetIoTHubs(bool reload = false) => Run<IoTHubInfo[]>("iot hub list", reload);
		public Task<IoTDeviceInfo[]> GetIoTDevices(string hubName) => Run<IoTDeviceInfo[]>($"iot hub device-identity list --hub-name {hubName}");
		public Task<IoTModuleIdentityInfo[]> GetIoTModules(string hubName, string deviceId) => Run<IoTModuleIdentityInfo[]>
			($"iot hub module-identity list --device-id {deviceId} --hub-name {hubName}");
		public Task<IoTDirectMethodReply> CallMethod(string method, string hubName, string deviceId, string moduleId, DirectMethodPayloadBase payload) => Run<IoTDirectMethodReply>
			($"iot hub invoke-module-method --method-name '{method}' -n '{hubName}' -d '{deviceId}' -m '{moduleId}' --method-payload '{JsonConvert.SerializeObject(payload, Newtonsoft.Json.Formatting.None)}'");

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    disposables.Dispose();
                    SaveCache();
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
