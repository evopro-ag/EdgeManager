using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using EdgeManager.Gui.Design;
using EdgeManager.Interfaces.Extensions;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;
using ReactiveUI;

namespace EdgeManager.Gui.ViewModels
{
    public class ModuleViewModel : ViewModelBase
    {
        private readonly IAzureService azureService;
        private readonly ISelectionService<IoTHubInfo> ioTHubInfoSelectionService;
        private readonly ISelectionService<IoTDeviceInfo> ioTDeviceSelectionService;
        private readonly ISelectionService<IoTModuleIdentityInfo> ioTModuleIdentityInfoSelectionService;
        private IoTModuleIdentityInfo selectedIoTModuleIdentityInfo;
        private IoTModuleIdentityInfo[] ioTModuleIdentityInfos;
        private bool loading;
        private string hubName;
        private string deviceId;
        private ModuleTwin currentModuleTwin;

        public ModuleViewModel(IAzureService azureService,
            ISelectionService<IoTHubInfo> ioTHubInfoSelectionService,
            ISelectionService<IoTDeviceInfo> ioTDeviceSelectionService, 
            ISelectionService<IoTModuleIdentityInfo> ioTModuleIdentityInfoSelectionService)
        {
            this.azureService = azureService;
            this.ioTHubInfoSelectionService = ioTHubInfoSelectionService;
            this.ioTDeviceSelectionService = ioTDeviceSelectionService;
            this.ioTModuleIdentityInfoSelectionService = ioTModuleIdentityInfoSelectionService;
        }

        public ReactiveCommand<Unit, Unit> ReloadCommand { get; set; }

        public override void Initialize()
        {
            this.WhenAnyValue(vm => vm.SelectedIoTModuleIdentityInfo)
                .Subscribe(x => ioTModuleIdentityInfoSelectionService.Select(x))
                .AddDisposableTo(Disposables);

            // ReSharper disable once InvokeAsExtensionMethod
            var deviceHubInfo = Observable.CombineLatest(
                    ioTDeviceSelectionService.SelectedObject,
                    ioTHubInfoSelectionService.SelectedObject,
                    (device, iotHub) => new { DeviceInfo = device, IoTHubInfo = iotHub })
                .Where(arg => arg.DeviceInfo != null && arg.IoTHubInfo != null);


            ioTDeviceSelectionService.SelectedObject
                .Where(deviceInfo => deviceInfo != null)
                .ObserveOnDispatcher()
                .Do(_ => Loading = true)
                .Do(s => deviceId = s.DeviceId)
                .Subscribe()
                .AddDisposableTo(Disposables);

            ioTHubInfoSelectionService.SelectedObject
                .Where(name => name != null)
                .ObserveOnDispatcher()
                .Do(identityInfos => IoTModuleIdentityInfos = new IoTModuleIdentityInfo[] { })
                .Do(s => hubName = s.Name)
                .Subscribe()
                .AddDisposableTo(Disposables);

            deviceHubInfo
                .Do(arg => Logger.Info($"Received Hub '{arg.IoTHubInfo.Name}' and Device '{arg.DeviceInfo.DeviceId}' for retrieving Modules"))
                .SelectMany(arg => azureService.GetIoTModules(arg.IoTHubInfo.Name, arg.DeviceInfo.DeviceId))
                .ObserveOnDispatcher()
                .Do(identityInfos => IoTModuleIdentityInfos = identityInfos)
                .Do(_ => Loading = false)
                .LogAndRetryAfterDelay(Logger, TimeSpan.FromSeconds(1), "Error while retrieving device modules information")
                .Subscribe()
                .AddDisposableTo(Disposables);

                ReloadCommand = ReactiveCommand.CreateFromTask(Reload)
                .AddDisposableTo(Disposables);
                
                // Observable erstellt von ModuleId
                var selectedModuleId = this.WhenAnyValue(model => model.SelectedIoTModuleIdentityInfo)
                    .Select(model => model?.ModuleId);

                // ReSharper disable once InvokeAsExtensionMethod
                Observable.CombineLatest(deviceHubInfo, selectedModuleId,
                    (deviceHub, module) => 
                        new
                    {
                        ModuleId = module, 
                        HubName = deviceHub.IoTHubInfo.Name,
                        DeviceId = deviceHub.DeviceInfo.DeviceId
                    })
                    .ObserveOnDispatcher()
                    .Do(_ => CurrentModuleTwin = null)
                    .Where(moduleInfo => !string.IsNullOrEmpty(moduleInfo.ModuleId))
                    .Do(arg => Logger.Info($"Received Hub '{arg.HubName}', Device '{arg.DeviceId}' and module id {arg.ModuleId} for retrieving module twin"))
                    .SelectMany(moduleInfo => azureService.GetIoTModelTwinProperties(moduleInfo.HubName, moduleInfo.DeviceId, moduleInfo.ModuleId))
                    .Do(twin => CurrentModuleTwin = twin)
                    .LogAndRetryAfterDelay(Logger, TimeSpan.FromMilliseconds(100), "Error while retrieving module twin for selection")
                    .Subscribe()
                    .AddDisposableTo(Disposables)
                    ;
        }

        public bool Loading
        {
            get => loading;
            set
            {
                if (value == loading) return;
                loading = value;
                raisePropertyChanged();
            }
        }

        public async Task<Unit> Reload()
        {
            try
            {
                Loading = true;
                Logger.Debug($"Reload Button -Modules- was pressed");
                IoTModuleIdentityInfos = await azureService.GetIoTModules(hubName,  deviceId, reload: true);
                Logger.Debug($"-Modules- was reloaded");
                Loading = false;
            }
            catch (Exception e)
            {
                Logger.Error("Error in Reactive command", e);
            }
            return Unit.Default;
        }

        public IoTModuleIdentityInfo[] IoTModuleIdentityInfos
        {

            get => ioTModuleIdentityInfos;
            set
            {
                if (Equals(value, ioTModuleIdentityInfos)) return;
                ioTModuleIdentityInfos = value;
                raisePropertyChanged();
            }
        }
        public IoTModuleIdentityInfo SelectedIoTModuleIdentityInfo
        {
            get => selectedIoTModuleIdentityInfo;
            set
            {
                if (Equals(value, selectedIoTModuleIdentityInfo)) return;
                selectedIoTModuleIdentityInfo = value;
                raisePropertyChanged();
            }
        }

        public ModuleTwin CurrentModuleTwin
        {
            get => currentModuleTwin;
            set
            {
                if (Equals(value, currentModuleTwin)) return;
                currentModuleTwin = value;
                raisePropertyChanged();
            }
        }
    }

    internal class DesignModuleViewModel : ModuleViewModel
    {
        public DesignModuleViewModel() : base(new DesignAzureService(), new DesignIoTHubSelectionService(), new DesignDeviceSelectionService(), new DesignModuleIdentitySelectionService())
        {
        }
    }
}
