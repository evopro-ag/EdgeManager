using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;
using Newtonsoft.Json;

namespace EdgeManager.Logic.Services
{
    class AzureCliHost : IAzureCli, IAzureService
	{
        private readonly IPowerShell powerShell;

        public AzureCliHost(IPowerShell powerShell)
        {
            this.powerShell = powerShell;
        }

		public async Task<T> Run<T>(string command)
        {
			var json = string.Join("\n", await powerShell.Execute("az " + command));
			//Console.WriteLine(json);
            //MessageBox.Show(json);
			return JsonConvert.DeserializeObject<T>(json);
		}

		public Task<IoTHubInfo[]> GetIoTHubs() => Run<IoTHubInfo[]>("iot hub list");
		public Task<IoTDeviceInfo[]> GetIoTDevices(string hubName) => Run<IoTDeviceInfo[]>($"iot hub device-identity list --hub-name {hubName}");
		public Task<IoTModuleIdentityInfo[]> GetIoTModules(string hubName, string deviceId) => Run<IoTModuleIdentityInfo[]>
			($"iot hub module-identity list --device-id {deviceId} --hub-name {hubName}");
		public Task<IoTDirectMethodReply> CallMethod(string method, string hubName, string deviceId, string moduleId, DirectMethodPayloadBase payload) => Run<IoTDirectMethodReply>
			($"iot hub invoke-module-method --method-name '{method}' -n '{hubName}' -d '{deviceId}' -m '{moduleId}' --method-payload '{JsonConvert.SerializeObject(payload, Newtonsoft.Json.Formatting.None)}'");

    }
}
