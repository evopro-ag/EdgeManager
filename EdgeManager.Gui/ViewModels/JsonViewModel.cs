using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Threading;
using EdgeManager.Gui.Design;
using EdgeManager.Interfaces.Extensions;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;
using Microsoft.PowerShell.Commands;
using ReactiveUI;

namespace EdgeManager.Gui.ViewModels
{
    public class JsonViewModel : ViewModelBase, IObserver<JsonCommand>
    {
        private readonly IAzureService azureService;
        private IDisposable azureServiceSubscriptionDisposeable;
        private ObservableCollection<JsonCommand> commandCollection;

        public JsonViewModel(IAzureService azureService)
        {
            this.azureService = azureService;
            commandCollection = new ObservableCollection<JsonCommand>();
        }

        public override void Initialize()
        {
            azureServiceSubscriptionDisposeable = azureService.Subscribe(this);
        }
       
        public void OnCompleted()
        {

        }

        public void OnError(Exception error)
        {
            throw error;
        }

        public void OnNext(JsonCommand value)
        {
            while(commandCollection.Count > 99) {
                commandCollection.RemoveAt(0);
            }
            commandCollection.Add(value);
        }
    }

    internal class DesignJsonViewModel : JsonViewModel
    {
        public DesignJsonViewModel() : base(new DesignAzureService())
        {

        }
    }
}