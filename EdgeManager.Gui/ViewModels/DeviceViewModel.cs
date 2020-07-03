using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
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
    public class DeviceViewModel : ViewModelBase
    {
        private readonly IAzureService azureService;
        private readonly ISelectionService<IoTHubInfo> ioTHubInfoSelectionService;
        private readonly ISelectionService<IoTDeviceInfo> ioTDeviceSelectionService;
        private IoTDeviceInfo selectedIoTDeviceInfo;
        private IoTDeviceInfo[] ioTDeviceInfos;
        private bool loading;
        private string hubName;

        public DeviceViewModel(IAzureService azureService, 
            ISelectionService<IoTHubInfo> ioTHubInfoSelectionService, 
            ISelectionService<IoTDeviceInfo> ioTDeviceSelectionService)
        {
            this.azureService = azureService;
            this.ioTHubInfoSelectionService = ioTHubInfoSelectionService;
            this.ioTDeviceSelectionService = ioTDeviceSelectionService;
        }

        public ReactiveCommand<Unit, Unit> ReloadCommand { get; set; }

        public override void Initialize()
        {
            this.WhenAnyValue(vm => vm.SelectedIoTDeviceInfo)
                .Subscribe(x => ioTDeviceSelectionService.Select(x))
                .AddDisposableTo(Disposables);

            ioTHubInfoSelectionService.SelectedObject
                .Where(iotHub => iotHub != null)
                .ObserveOnDispatcher()
                .Do(_ => Loading = true)
                .Do(devices => IoTDeviceInfos = new IoTDeviceInfo[]{})
                .Do(i => this.hubName = i.Name)
                .Subscribe()
                .AddDisposableTo(Disposables);
                
            ioTHubInfoSelectionService.SelectedObject
                .Where(iotHub => iotHub != null)
                .SelectMany(iothub => azureService.GetIoTDevices(iothub.Name))
                .ObserveOnDispatcher()
                .Do(devices => IoTDeviceInfos = devices)
                .Do(_ => Loading = false)
                .LogAndRetryAfterDelay(Logger, TimeSpan.FromSeconds(1), "Error while retrieving devices information")
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
                IoTDeviceInfos = await azureService.GetIoTDevices(hubName, reload: true);
                Loading = false;
            }
            catch (Exception e)
            {
                Logger.Error("Error in Reactive command", e);
            }
            return Unit.Default;
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
