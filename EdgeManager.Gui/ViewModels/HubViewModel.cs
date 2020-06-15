using EdgeManager.Gui.Design;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using EdgeManager.Gui.Views;
using EdgeManager.Interfaces.Extensions;
using ReactiveUI;

namespace EdgeManager.Gui.ViewModels
{
    public class HubViewModel : ViewModelBase
    {
        private readonly IAzureService azureService;
        private readonly ISelectionService<IoTHubInfo> selectionService;
        private IoTHubInfo selectedIotHubInfo;
        private IoTHubInfo[] iotHubInfo;

        public HubViewModel(IAzureService azureService, ISelectionService<IoTHubInfo> selectionService)
        {
            this.azureService = azureService;
            this.selectionService = selectionService;
        }

        public override async void Initialize()
        {
            this.WhenAnyValue(vm => vm.SelectedIotHubInfo)
                .ObserveOnDispatcher()
                .Subscribe(x => selectionService.Select(x))
                .AddDisposableTo(Disposables);
            try
            {
                IotHubInfo = await azureService.GetIoTHubs();
            }
            catch (Exception e)
            {
                Logger.Error($"Error getting IoTHUbs: {e.Message}", e);
            }
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
        public DesignHubViewModel() : base(new DesignAzureService(), new DesignSelectionService())
        {
        }
    }
}
