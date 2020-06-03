using EdgeManager.Gui.Design;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;
using System;

namespace EdgeManager.Gui.ViewModels
{
    public class ConnectionCabViewModel : ViewModelBase
    {
        private readonly IAzureService azureService;
        private IoTHubInfo selectedIotHubInfo;

        public ConnectionCabViewModel(IAzureService azureService)
        {
            this.azureService = azureService;
        }

        public override async void Initialize()
        {
            try
            {
                IotHubInfos = await azureService.GetIoTHubs();
            }
            catch (Exception ex)
            {
                Logger.Error($"Error initializing ConnectionCabViewModel: {ex.Message}", ex);
            }
        }

        public IoTHubInfo[] IotHubInfos { get; private set; }

        public IoTHubInfo SelectedIotHubInfo
        {
            get => selectedIotHubInfo;
            set
            {
                if (Equals(value, selectedIotHubInfo)) return;
                selectedIotHubInfo = value;
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
