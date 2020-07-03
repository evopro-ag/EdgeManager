using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;

namespace EdgeManager.Gui.Design
{
    public class DesignAzureService : IAzureService
    {
        public IObservable<JsonCommand> JsonCommands { get; } = Observable.Never<JsonCommand>();

        public Task<IoTHubInfo[]> GetIoTHubs(bool reload = false)
        {
            return Task.FromResult(new[]
            {
                new IoTHubInfo {Name = "Hub1", Properties = new IoTHubProperties()},
                new IoTHubInfo {Name = "Hub2", Properties = new IoTHubProperties()},
                new IoTHubInfo {Name = "Hub3", Properties = new IoTHubProperties()}
            });
        }

        public Task<IoTDeviceInfo[]> GetIoTDevices(string hubName, bool reload = false)
        {
            return Task.FromResult(new[]
            {
                new IoTDeviceInfo {DeviceId = "Device1"},
                new IoTDeviceInfo {DeviceId = "Device2"},
                new IoTDeviceInfo {DeviceId = "Device3"}
            });
        }

        public Task<IoTModuleIdentityInfo[]> GetIoTModules(string hubName, string deviceId, bool reload = false)
        {
            return Task.FromResult(new[]
            {
                new IoTModuleIdentityInfo {ModuleId = "Module1"},
                new IoTModuleIdentityInfo {ModuleId = "Module2"},
                new IoTModuleIdentityInfo {ModuleId = "Module3"},
            });
        }

        public Task<IoTDirectMethodReply> CallMethod(string method, string hubName, string deviceId, string moduleId, DirectMethodPayloadBase payload)
        {
            throw new NotImplementedException();
        }

        public Task<JsonCommand> GetCommand(string command)
        {
            throw new NotImplementedException();
        }

        public IDisposable Subscribe(IObserver<JsonCommand> observer)
        {
            throw new NotImplementedException();
        }
    }
}
