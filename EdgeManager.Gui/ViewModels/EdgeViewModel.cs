using System;
using System.Collections.Generic;
using System.Text;
using EdgeManager.Gui.Design;
using EdgeManager.Interfaces.Extensions;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;

namespace EdgeManager.Gui.ViewModels
{
    public class EdgeViewModel : ViewModelBase
    {
        private readonly IAzureService azureService;
        private readonly ISelectionService<IoTHubInfo> ioTHubInfoSelectionService;
        private IoTDeviceInfo selectedIoTDeviceInfo;

        public EdgeViewModel(IAzureService azureService, ISelectionService<IoTHubInfo> ioTHubInfoSelectionService)
        {
            this.azureService = azureService;
            this.ioTHubInfoSelectionService = ioTHubInfoSelectionService;
        }

        public override void Initialize()
        {
            ioTHubInfoSelectionService.SelectedObject
                .Subscribe(async x =>
                {

                    try
                    {
                        IoTDeviceInfo = await azureService.GetIoTDevices(x.Name);
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"Error geting devices for IoT Hub {x.Name}: {e.Message}", e);
                    }
                })
                .AddDisposableTo(Disposables);
        }

        public IoTDeviceInfo[] IoTDeviceInfo { get; private set; }

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

        public class DesignEdgeViewModel : EdgeViewModel
        {
            public DesignEdgeViewModel() : base(new DesignAzureService(), new DesignSelectionService())
            {
            }
        }
    }
}
