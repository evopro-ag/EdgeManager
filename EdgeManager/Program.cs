using System;
using System.Collections;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EdgeManager
{
    class Program
    {
        async Task Main(string[] args)
        {
			using (var azure = new AzureCliHost())
			{
				//var list = await azure.Run<IoTHubInfo[]>("iot hub list");

				//await azure.CallMethod("ping", "evoproTestHub", "IoT_Edge_One", "$edgeAgent", new DirectMethodPayloadBase()).Dump();
				//return;

				var list = await azure.GetIoTHubs();
                Console.WriteLine(list);
				//foreach (var hub in list)
				//{
				//	var deviceList = await azure.GetIoTDevices(hub.Name);
				//	deviceList.Dump($"Devices for {hub.Name}");
				//	foreach (var device in deviceList)
				//	{
				//		if (device.Capabilities.IoTEdge)
				//		{
				//			var moduleList = await azure.GetIoTModule(hub.Name, device.DeviceId);
				//			moduleList.Dump($"Modules for {device.DeviceId}");
				//			foreach (var modul in moduleList)
				//			{
				//				var methodPayload = await azure.CallMethod("ping", hub.Name, device.DeviceId, modul.ModuleId, new DirectMethodPayloadBase());
				//				methodPayload.Dump($"Ping Status for {modul.ModuleId}");
				//			}
				//		}
				//	}
				//}
			}
		}
	}
	public enum ConnectionState
	{
		Disconnected,
		Connected
	}
}
