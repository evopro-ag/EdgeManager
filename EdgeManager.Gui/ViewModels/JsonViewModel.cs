using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Windows.Threading;
using DynamicData;
using DynamicData.Binding;
using EdgeManager.Gui.Design;
using EdgeManager.Interfaces.Extensions;
using EdgeManager.Interfaces.Logging;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;
using log4net;
using Microsoft.PowerShell.Commands;
using ReactiveUI;

namespace EdgeManager.Gui.ViewModels
{
    public class JsonViewModel : ViewModelBase
    {
        private readonly ILog logger = LoggerFactory.GetLogger(typeof(JsonViewModel));
        private readonly IAzureService azureService;
        private JsonCommand selectedJsonCommand;

        public JsonViewModel(IAzureService azureService)
        {
            this.azureService = azureService;
            
        }

        public IObservableCollection<JsonCommand> JsonCommands { get; } = new ObservableCollectionExtended<JsonCommand>();

        public override void Initialize()
        {
            azureService.JsonCommands
                .ToObservableChangeSet(json => json.Command.GetHashCode(), limitSizeTo: 10)
                .ObserveOnDispatcher()
                .Bind(JsonCommands)
                .Subscribe()
                .AddDisposableTo(Disposables);
        }

        public JsonCommand SelectedJsonCommand 
        { 
            get => selectedJsonCommand; 
            set
            {
                if (selectedJsonCommand == value) return;
                selectedJsonCommand = value;
                raisePropertyChanged();
            }
        }
    }

    internal class DesignJsonViewModel : JsonViewModel
    {
        public DesignJsonViewModel() : base(new DesignAzureService())
        {

        }
    }
}