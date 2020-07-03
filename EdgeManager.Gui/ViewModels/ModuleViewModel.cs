using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

            var observable = Observable.CombineLatest(
                    ioTDeviceSelectionService.SelectedObject,
                    ioTHubInfoSelectionService.SelectedObject,
                    (device, iotHub) => new { DeviceInfo = device, IoTHubInfo = iotHub })
                .Where(arg => arg.DeviceInfo != null && arg.IoTHubInfo != null);

            //observable
            //    .ObserveOnDispatcher()
            //    .Do(_ => Loading = true)
            //    .Do(identityInfos => IoTModuleIdentityInfos = new IoTModuleIdentityInfo[] { })
            //    //.Do(i => this.hubName = i.IoTHubInfo.Name)
            //    //.Do(i => this.deviceId = i.DeviceInfo.DeviceId)
            //    .Subscribe()
            //    .AddDisposableTo(Disposables);

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

                observable
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
                IoTModuleIdentityInfos = await azureService.GetIoTModules(hubName,  deviceId, reload: true);
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
    }

    internal class DesignModuleViewModel : ModuleViewModel
    {
        public DesignModuleViewModel() : base(new DesignAzureService(), new DesignIoTHubSelectionService(), new DesignDeviceSelectionService(), new DesignModuleIdentitySelectionService())
        {
        }
    }
}
