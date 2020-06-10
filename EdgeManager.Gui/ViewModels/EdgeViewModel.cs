using System;
using System.Collections.Generic;
using System.Text;
using EdgeManager.Gui.Design;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;

namespace EdgeManager.Gui.ViewModels
{
    public class EdgeViewModel : ViewModelBase
    {
        private readonly IAzureService azureService;
        private IoTDeviceInfo selectedIoTDeviceInfo;

        public EdgeViewModel(IAzureService azureService)
        {
            this.azureService = azureService;
        }

        public override void Initialize()
        {
            try
            {
                //IoTDeviceInfo = await azureService.GetIoTDevices(SelectedIoTDeviceInfo.DeviceId);

            }
            catch (Exception ex)
            {
                Logger.Error($"Error initializing ConnectionCabViewModel: {ex.Message}", ex);
            }
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
            public DesignEdgeViewModel() : base(new DesignAzureService())
            {
            }
        }
    }
}
