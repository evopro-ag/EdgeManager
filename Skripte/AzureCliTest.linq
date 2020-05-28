<Query Kind="Program">
  <Connection>
    <ID>54bf9502-9daf-4093-88e8-7177c129999f</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Provider>System.Data.SqlServerCe.4.0</Provider>
    <AttachFileName>&lt;ApplicationData&gt;\LINQPad\DemoDB.sdf</AttachFileName>
    <Persist>true</Persist>
  </Connection>
  <NuGetReference>Microsoft.PowerShell.SDK</NuGetReference>
  <NuGetReference>System.Collections</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>NJsonSchema.Annotations</Namespace>
  <Namespace>System.Management.Automation</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Net.NetworkInformation</Namespace>
</Query>

async Task Main()
{
	using (var azure = new AzureCliHost())
	{
		//var list = await azure.Run<IoTHubInfo[]>("iot hub list");

		await azure.CallMethod("ping", "evoproTestHub", "IoT_Edge_One", "$edgeAgent", new DirectMethodPayloadBase()).Dump();
		return;
		
		var list = await azure.GetIoTHubs();
		list.Dump();
		foreach (var hub in list)
		{
			var deviceList = await azure.GetIoTDevices(hub.Name);
			deviceList.Dump($"Devices for {hub.Name}");
			foreach (var device in deviceList)
			{
				if (device.Capabilities.IoTEdge)
				{
					var moduleList = await azure.GetIoTModule(hub.Name, device.DeviceId);
					moduleList.Dump($"Modules for {device.DeviceId}");
					foreach (var modul in moduleList)
					{
						var methodPayload = await azure.CallMethod("ping", hub.Name, device.DeviceId, modul.ModuleId, new DirectMethodPayloadBase());
						methodPayload.Dump($"Ping Status for {modul.ModuleId}");
					}
				}
			}	
		}
	}
}

// Define other methods, classes and namespaces here

class PowerShellHost : IDisposable
{
	private readonly PowerShell ps = PowerShell.Create();
	
	public void Dispose()
	{
		ps.Dispose();
	}
	
	protected Task<PSDataCollection<PSObject>> Execute(string command)
	{
		ps.AddScript(command);
		return ps.InvokeAsync();
	}
}

class AzureCliHost : PowerShellHost
{
	public async Task<T> Run<T>(string command)
	{
		var json = string.Join("\n", await base.Execute("az "+command.Dump()));
		//json.Dump();
		return JsonConvert.DeserializeObject<T>(json);
	}

	public Task<IoTHubInfo[]> GetIoTHubs() => Run<IoTHubInfo[]>("iot hub list");
	public Task<IoTDeviceIdentityInfo[]> GetIoTDevices(string hubName) => Run<IoTDeviceIdentityInfo[]>($"iot hub device-identity list --hub-name {hubName}");
	public Task<IoTModuleIdentityInfo[]> GetIoTModule(string hubName, string deviceId) => Run<IoTModuleIdentityInfo[]>
		($"iot hub module-identity list --device-id {deviceId} --hub-name {hubName}");
	public Task<IoTDirectMethodReply> CallMethod(string method, string hubName, string deviceId, string moduleId, DirectMethodPayloadBase payload) => Run<IoTDirectMethodReply>
		($"iot hub invoke-module-method --method-name '{method}' -n '{hubName}' -d '{deviceId}' -m '{moduleId}' --method-payload '{JsonConvert.SerializeObject(payload, Newtonsoft.Json.Formatting.None)}'");
}


public class IoTHubInfo
{
	//[JsonProperty("name")] // see details here: https://www.newtonsoft.com/json/help/html/JsonPropertyName.htm
	public string Name { get; set; }
	public IoTHubProperties Properties { get; set; }
}

public class IoTHubProperties
{
	public string HostName { get; set; }
}

public class IoTDeviceIdentityInfo
{
	public string DeviceId { get; set; }
	public ConnectionState ConnectionState { get; set; }
	public Capabilities Capabilities { get; set; }
}

public class IoTModuleIdentityInfo
{
	public string ModuleId { get; set; }
	public ConnectionState ConnectionState { get; set; }
}

public class IoTDirectMethodReply
{
	public object Payload { get; set; }
	public int Status { get; set; }
}

public enum ConnectionState
{
	Disconnected,
	Connected
}

public class Capabilities
{
	public bool IoTEdge { get; set; }
}

public class DirectMethodPayloadBase
{
}

public class RestartModulePayload : DirectMethodPayloadBase
{
	[JsonProperty("schemaVersion")] public string SchemaVersion { get; set; } = "1.0";
	[JsonProperty("id")] public string ModuleId { get; set; } = "Marcolino";
}