using EdgeManager.Gui.Design;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;
using System;
using System.Windows;
using EdgeManager.Gui.Views;

namespace EdgeManager.Gui.ViewModels
{
    public class ConnectionCabViewModel : ViewModelBase
    {
        private readonly IAzureService azureService;
        private IoTHubInfo selectedIotHubInfo;
        private IoTDeviceInfo selectedIotDeviceInfo;

        public ConnectionCabViewModel(IAzureService azureService)
        {
            this.azureService = azureService;
        }

        public override async void Initialize()
        {
            try
            {
                //IotHubInfo = await azureService.GetIoTHubs();
        //        IoTDeviceInfos = await azureService.GetIoTDevices(SelectedIotHubInfo.Name);

            }
            catch (Exception ex)
            {
                Logger.Error($"Error initializing ConnectionCabViewModel: {ex.Message}", ex);
            }
        }

        public IoTHubInfo[] IotHubInfo { get; private set; }

        public IoTHubInfo SelectedIotHubInfo
        {
            get => selectedIotHubInfo;
            set
            {
                if (Equals(value, selectedIotHubInfo)) return;
                selectedIotHubInfo = value;
                raisePropertyChanged();
                //MessageBox.Show(SelectedIoTDeviceInfo.DeviceId);
            }
        }

        public IoTDeviceInfo[] IoTDeviceInfo { get; private set; }

        public IoTDeviceInfo SelectedIoTDeviceInfo
        {
            get => selectedIotDeviceInfo;
            set
            {
                if (Equals(value, selectedIotDeviceInfo)) return;
                selectedIotDeviceInfo = value;
                raisePropertyChanged();
                
            }
        }
    }

    public class DesignConnectionCabViewModel : ConnectionCabViewModel
    {
        public DesignConnectionCabViewModel() : base(new DesignAzureService())
        {
        }
    }
}
