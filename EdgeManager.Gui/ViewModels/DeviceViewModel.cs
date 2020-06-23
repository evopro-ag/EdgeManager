using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using EdgeManager.Gui.Design;
using EdgeManager.Interfaces.Extensions;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;
using ReactiveUI;

namespace EdgeManager.Gui.ViewModels
{
    public class DeviceViewModel : ViewModelBase
    {
        private readonly IAzureService azureService;
        private readonly ISelectionService<IoTHubInfo> ioTHubInfoSelectionService;
        private readonly ISelectionService<IoTDeviceInfo> ioTDeviceSelectionService;
        private IoTDeviceInfo selectedIoTDeviceInfo;
        private IoTDeviceInfo[] ioTDeviceInfos;

        public DeviceViewModel(IAzureService azureService, 
            ISelectionService<IoTHubInfo> ioTHubInfoSelectionService, 
            ISelectionService<IoTDeviceInfo> ioTDeviceSelectionService)
        {
            this.azureService = azureService;
            this.ioTHubInfoSelectionService = ioTHubInfoSelectionService;
            this.ioTDeviceSelectionService = ioTDeviceSelectionService;
        }

        public override void Initialize()
        {
            this.WhenAnyValue(vm => vm.SelectedIoTDeviceInfo)
                .Subscribe(x => ioTDeviceSelectionService.Select(x))
                .AddDisposableTo(Disposables);

            ioTHubInfoSelectionService.SelectedObject
                .Where(iotHub => iotHub != null)
                .SelectMany(iothub => azureService.GetIoTDevices(iothub.Name))
                .ObserveOnDispatcher()
                .Do(devices => IoTDeviceInfos = devices)
                .LogAndRetryAfterDelay(Logger, TimeSpan.FromSeconds(1), "Error while retrieving devices information")

                .Subscribe()
                .AddDisposableTo(Disposables);
        }

        public IoTDeviceInfo[] IoTDeviceInfos
        {
            get => ioTDeviceInfos;
            private set
            {
                if (Equals(value, ioTDeviceInfos)) return;
                ioTDeviceInfos = value;
                raisePropertyChanged();
            }
        }

        public IoTDeviceInfo SelectedIoTDeviceInfo
        {
            get => selectedIoTDeviceInfo;
            set
            {
                if (Equals(value, selectedIoTDeviceInfo)) return;
                selectedIoTDeviceInfo = value;
                raisePropertyChanged();
                
            }
        }
    }

    internal class DesignDeviceViewModel : DeviceViewModel
    {
        public DesignDeviceViewModel() : base(new DesignAzureService(), new DesignIoTHubSelectionService(), new DesignDeviceSelectionService())
        {
        }
    }
}
