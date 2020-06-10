using EdgeManager.Gui.Design;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using EdgeManager.Gui.Views;

namespace EdgeManager.Gui.ViewModels
{
    public class HubViewModel : ViewModelBase
    {
        private readonly IAzureService azureService;
        private IoTHubInfo selectedIotHubInfo;
        private IoTHubInfo[] iotHubInfo;

        public HubViewModel(IAzureService azureService)
        {
            this.azureService = azureService;
        }

        public override void Initialize()
        {
            var subscription = Observable.Return(Unit.Default)
                    .Do(async _ => { IotHubInfo = await azureService.GetIoTHubs(); })
                    .Subscribe();

            Disposables.Add(subscription);
        }

        public IoTHubInfo[] IotHubInfo
        {
            get => iotHubInfo;
            private set
            {
                if (Equals(value, iotHubInfo)) return;
                iotHubInfo = value;
                raisePropertyChanged();
            }
        }

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

    public class DesignHubViewModel : HubViewModel
    {
        public DesignHubViewModel() : base(new DesignAzureService())
        {
        }
    }
}
