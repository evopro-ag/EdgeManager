using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
        private IoTModuleIdentityInfo[] ioTModulIdentityInfos;

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

        public override async void Initialize()
        {
            this.WhenAnyValue(vm => vm.SelectedIoTModuleIdentityInfo)
                .Subscribe(x => ioTModuleIdentityInfoSelectionService.Select(x))
                .AddDisposableTo(Disposables);



            // ReSharper disable once InvokeAsExtensionMethod
            Observable.CombineLatest(
                    ioTDeviceSelectionService.SelectedObject, 
                    ioTHubInfoSelectionService.SelectedObject, 
                    (device, iotHub) => new { DeviceInfo = device, IoTHubInfo = iotHub})
                .Where(arg => arg.DeviceInfo != null && arg.IoTHubInfo != null)
                .Do(arg => Logger.Info($"Received Hub '{arg.IoTHubInfo.Name}' and Device '{arg.DeviceInfo.DeviceId}' for retrieving Modules"))
                .SelectMany(arg => azureService.GetIoTModules(arg.IoTHubInfo.Name, arg.DeviceInfo.DeviceId))
                .ObserveOnDispatcher()
                .Do(identityInfos => IoTModuleIdentityInfos = identityInfos)
                .LogAndRetryAfterDelay(Logger, TimeSpan.FromSeconds(1), "Error while retrieving device modules information")
                .Subscribe()
                .AddDisposableTo(Disposables);

            //try
            //{
            //    //IotDeviceInfo = await azureService.GetIoTDevices(ioTDeviceSelectionService.ToString());
            //}
            //catch (Exception e)
            //{
            //    Logger.Error($"Error getting IoTHUbs: {e.Message}", e);
            //}
        }


        public IoTModuleIdentityInfo[] IoTModuleIdentityInfos
        {

            get => ioTModulIdentityInfos;
            set
            {
                if (Equals(value, ioTModulIdentityInfos)) return;
                ioTModulIdentityInfos = value;
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
        public DesignModuleViewModel() : base(new DesignAzureService(), new DesignIoTHubSelectionService(), new DesignDeviceSelectionService(), new DesignMoluleIdentitySelectionService())
        {
        }
    }
}
