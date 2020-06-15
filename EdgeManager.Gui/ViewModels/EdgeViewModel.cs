using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using EdgeManager.Gui.Design;
using EdgeManager.Interfaces.Extensions;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;
using ReactiveUI;

namespace EdgeManager.Gui.ViewModels
{
    public class EdgeViewModel : ViewModelBase
    {
        private readonly IAzureService azureService;
        private readonly ISelectionService<IoTHubInfo> ioTHubInfoSelectionService;
        private readonly ISelectionService<IoTDeviceInfo> ioTDeviceSelectionService;
        private IoTDeviceInfo selectedIoTDeviceInfo;
        private IoTDeviceInfo[] ioTDeviceInfos;

        public EdgeViewModel(IAzureService azureService, ISelectionService<IoTHubInfo> ioTHubInfoSelectionService, ISelectionService<IoTDeviceInfo> ioTDeviceSelectionService)
        {
            this.azureService = azureService;
            this.ioTHubInfoSelectionService = ioTHubInfoSelectionService;
            this.ioTDeviceSelectionService = ioTDeviceSelectionService;
        }

        public override void Initialize()
        {
            this.WhenAnyValue(vm => vm.SelectedIoTDeviceInfo)
                .ObserveOnDispatcher()
                .Subscribe(x => ioTDeviceSelectionService.Select(x))
                .AddDisposableTo(Disposables);

            ioTHubInfoSelectionService.SelectedObject
                .Subscribe(async x =>
                {
                    if (x == null)
                    {
                        IoTDeviceInfos = new IoTDeviceInfo[0];
                        return;
                    }

                    try
                    {
                        IoTDeviceInfos = await azureService.GetIoTDevices(x.Name);
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"Error geting devices for IoT Hub {x.Name}: {e.Message}", e);
                    }
                })
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

    public class DesignEdgeViewModel : EdgeViewModel
    {
        public DesignEdgeViewModel() : base(new DesignAzureService(), new DesignIoTHubSelectionService(), new DesignDeviceSelectionService())
        {
        }
    }
}
