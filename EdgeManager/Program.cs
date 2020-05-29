using System;
using System.Collections;
using System.Threading.Tasks;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;
using EdgeManager.Logic;
using Newtonsoft.Json;
using Ninject;

namespace EdgeManager
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            using (var kernel = new StandardKernel())
            {
                LoadModules(kernel);

				//create azure clie
                IAzureService azure = kernel.Get<IAzureService>(); //todo: Use Ninject here

                GetAllIoTHubInfo(azure).Wait();
            }

			//using (var azure = new AzureCliHost())
			//{
			//var list = await azure.Run<IoTHubInfo[]>("iot hub list");

			//await azure.CallMethod("ping", "evoproTestHub", "IoT_Edge_One", "$edgeAgent", new DirectMethodPayloadBase()).Dump();
			//return;


			//}
		}

        private static async Task GetAllIoTHubInfo(IAzureService azure)
        {
            var list = await azure.GetIoTHubs();
            Console.WriteLine(list);
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

        private static void LoadModules(StandardKernel kernel)
        {
            kernel.Load<LogicModuleCatalog>();
        }
    }

    public static class ConsoleExtensions
    {
        public static object Dump(this object o, string description = "")
        {
            Console.WriteLine(description);
            var serializeObject = JsonConvert.SerializeObject(o, Formatting.Indented);
            Console.WriteLine(serializeObject);
            return o;
        }
    }
}
