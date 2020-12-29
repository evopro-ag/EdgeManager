using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using EdgeManager.Gui.Design;
using EdgeManager.Gui.Models;
using EdgeManager.Interfaces.Extensions;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReactiveUI;

namespace EdgeManager.Gui.ViewModels
{
    public class MonitorDeviceViewModel : ViewModelBase
    {
        private readonly IAzureService azureService;
        private string content;
        private int maxNumberOfEvents = 100;
        private bool record = true;
        private List<string> eventCache = new List<string>();
        private IotEdgeEventModel selectedEvent;
        private bool autoscroll = true;
        public IoTDeviceInfo Model { get; }

        public ObservableCollection<IotEdgeEventModel> Events { get; set; } = new ObservableCollection<IotEdgeEventModel>();

        public IotEdgeEventModel SelectedEvent
        {
            get => selectedEvent;
            set
            {
                if (Equals(value, selectedEvent)) return;
                selectedEvent = value;
                raisePropertyChanged();
            }
        }

        public MonitorDeviceViewModel(IoTDeviceInfo model, IAzureService azureService)
        {
            this.azureService = azureService;
            Model = model;
        }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; set; }
        
        public override void Initialize()
        {
            var observeDevice = azureService.MonitorDevice(Model.HubName, Model.DeviceId);

            observeDevice.GetTelemetryMessagesInJsonFormat()
                .Select(o => o["event"]?.ToObject<IotEdgeEventModel>())
                .Throttle(TimeSpan.FromSeconds(1))
                .Where(m => m != null)
                .ObserveOnDispatcher()
                .Where(_ => Record)
                .Do(AddEventToCollection)
                .Do(AddToCache)
                .Subscribe()
                .AddDisposableTo(Disposables);

            observeDevice.AddDisposableTo(Disposables);

            SaveCommand = ReactiveCommand.CreateFromTask(SaveEvents);
            
            ToggleRecordStateCommand = ReactiveCommand.CreateFromTask(ToggleRecord).AddDisposableTo(Disposables);
        }

        private Task<Unit> ToggleRecord()
        {
            Record = !Record;
            return Task.FromResult(Unit.Default);
        }

        private async Task<Unit> SaveEvents()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Save events to file",
                Filter = "Json file (*.jsons)|*.jsons",
                InitialDirectory = @"c:\temp\"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                //Write to file
                await File.WriteAllLinesAsync(saveFileDialog.FileName, eventCache);
            }
            return Unit.Default;
        }

        private void AddToCache(IotEdgeEventModel obj)
        {
            //convert object to a single line in json format
            eventCache.Add(JsonConvert.SerializeObject(obj, Formatting.None));
        }

        public int MaxNumberOfEvents
        {
            get => maxNumberOfEvents;
            set
            {
                if (value == maxNumberOfEvents) return;
                maxNumberOfEvents = value;
                raisePropertyChanged();
            }
        }

        public bool Autoscroll
        {
            get => autoscroll;
            set
            {
                if (value == autoscroll) return;
                autoscroll = value;
                raisePropertyChanged();
            }
        }

        public bool Record
        {
            get => record;
            set
            {
                if (value == record) return;
                record = value;
                raisePropertyChanged();
            }
        }

        public ReactiveCommand<Unit, Unit> ToggleRecordStateCommand { get; set; }

        private void AddEventToCollection(IotEdgeEventModel obj)
        {
            //remove oldest element if outside bounds
            if (Events.Count >= MaxNumberOfEvents)
            {
                Events.RemoveAt(0);
            }
            
            Events.Add(obj);
            
            //Apply autoscroll if choosen
            if (Autoscroll)
            {
                selectedEvent = Events.Last();
                raisePropertyChanged(nameof(SelectedEvent));
            }
        }
    }
    
    internal sealed class  DesignMonitorDeviceViewModel : MonitorDeviceViewModel
    {
        public DesignMonitorDeviceViewModel() : base(new IoTDeviceInfo(), new DesignAzureService())
        {
            Initialize();
        }
    }
}