﻿using EdgeManager.Gui.Design;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using EdgeManager.Gui.Views;
using EdgeManager.Interfaces.Extensions;
using ReactiveUI;
using System.Threading.Tasks;
using System.Reactive.Subjects;
using EdgeManager.Interfaces.Settings;

namespace EdgeManager.Gui.ViewModels
{
    public class HubViewModel : ViewModelBase
    {
        private readonly IAzureService azureService;
        private readonly ISelectionService<IoTHubInfo> iotHubSelectionService;
        private readonly ISelectionService<IoTDeviceInfo> deviceInfoSelectionService;
        private readonly ApplicationSettings settings;
        private IoTHubInfo selectedIotHubInfo;
        private IoTHubInfo[] iotHubInfo;
        private bool loading;
        private bool notLoading = true;

      
        public HubViewModel(IAzureService azureService, 
            ISelectionService<IoTHubInfo> iotHubSelectionService,
            ISelectionService<IoTDeviceInfo> deviceInfoSelectionService,
            ApplicationSettings settings
            )
        {
            this.azureService = azureService;
            this.iotHubSelectionService = iotHubSelectionService;
            this.deviceInfoSelectionService = deviceInfoSelectionService;
            this.settings = settings;
        }

        public ReactiveCommand<Unit, Unit> ReloadCommand { get; set; }


        public override void Initialize()
        {
            this.WhenAnyValue(vm => vm.SelectedIotHubInfo)
                .Do(selectedhub =>
                {

                    deviceInfoSelectionService.Select(null);
                    iotHubSelectionService.Select(selectedhub);
                    if (selectedhub != null)
                        settings.LastUsedIoTHub = selectedhub;
                })
                .Subscribe()
                .AddDisposableTo(Disposables);
            
            
            if (settings.LastCheckedIoTHubs != null && settings.LastCheckedIoTHubs.Any())
            {
                IotHubInfo = settings.LastCheckedIoTHubs;
                
                if (settings.LastUsedIoTHub != null)
                {
                    SelectedIotHubInfo = settings.LastCheckedIoTHubs.Single(h => h.Name.Equals(settings.LastUsedIoTHub.Name));
                }
            }
            else
            {
                Observable.Return(Unit.Default)
                    .SelectMany(_ => azureService.GetIoTHubs())
                    .ObserveOnDispatcher()
                    .Do(hubs => IotHubInfo = hubs)
                    .LogAndRetryAfterDelay(Logger, TimeSpan.FromSeconds(1),
                        "Error while retrieving iot hubs information")
                    .Subscribe()
                    .AddDisposableTo(Disposables);
            }

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
                NotLoading = !value;
                raisePropertyChanged();
            }
        }

        public bool NotLoading
        {
            get => notLoading;
            set
            {
                if (value == notLoading) return;
                notLoading = value;
                raisePropertyChanged();
            }
        }

        public async Task<Unit> Reload()
        {
            try
            {
                Loading = true;
                Logger.Debug($"Reload Button -IoT Hub- was pressed");
                IotHubInfo = await azureService.GetIoTHubs(true);
                Logger.Debug($"-IoT Hub- was reloaded");
                Loading = false;
            }
            catch(Exception e)
            {
                Logger.Error("Error in Reactive command", e);
            }
            return Unit.Default;
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

    internal class DesignHubViewModel : HubViewModel
    {
        public DesignHubViewModel() : base(new DesignAzureService(), new DesignIoTHubSelectionService(), new DesignDeviceSelectionService(), new ApplicationSettings())
        {
        }
    }
}
