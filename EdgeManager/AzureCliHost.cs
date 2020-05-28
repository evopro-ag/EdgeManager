using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeManager
{
    class AzureCliHost : PowerShellHost
    {
		public async Task<T> Run<T>(string command)
        {
			var json = string.Join("\n", await base.Execute("az " + command));
			Console.WriteLine(json); ;
			return JsonConvert.DeserializeObject<T>(json);
		}

		public Task<IoTHubInfo[]> GetIoTHubs() => Run<IoTHubInfo[]>("iot hub list");
		public Task<IoTDeviceIdentityInfo[]> GetIoTDevices(string hubName) => Run<IoTDeviceIdentityInfo[]>($"iot hub device-identity list --hub-name {hubName}");
		public Task<IoTModuleIdentityInfo[]> GetIoTModule(string hubName, string deviceId) => Run<IoTModuleIdentityInfo[]>
			($"iot hub module-identity list --device-id {deviceId} --hub-name {hubName}");
		public Task<IoTDirectMethodReply> CallMethod(string method, string hubName, string deviceId, string moduleId, DirectMethodPayloadBase payload) => Run<IoTDirectMethodReply>
			($"iot hub invoke-module-method --method-name '{method}' -n '{hubName}' -d '{deviceId}' -m '{moduleId}' --method-payload '{JsonConvert.SerializeObject(payload, Newtonsoft.Json.Formatting.None)}'");
	}
}
