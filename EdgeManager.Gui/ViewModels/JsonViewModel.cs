using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Threading;
using EdgeManager.Gui.Design;
using EdgeManager.Interfaces.Extensions;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;
using ReactiveUI;

namespace EdgeManager.Gui.ViewModels
{
    public class JsonViewModel : ViewModelBase
    {
        private readonly IAzureService azureService;
        private readonly ISelectionService<IoTHubInfo> ioTHubInfoSelectionService;
        private readonly ISelectionService<IoTDeviceInfo> ioTDeviceSelectionService;
        private readonly ISelectionService<IoTModuleIdentityInfo> ioTModuleIdentityInfoSelectionService;
        public JsonViewModel(IAzureService azureService,
            ISelectionService<IoTHubInfo> ioTHubInfoSelectionService,
            ISelectionService<IoTDeviceInfo> ioTDeviceSelectionService,
            ISelectionService<IoTModuleIdentityInfo> ioTModuleIdentityInfoSelectionService)
        {
            this.azureService = azureService;
            this.ioTHubInfoSelectionService = ioTHubInfoSelectionService;
            this.ioTDeviceSelectionService = ioTDeviceSelectionService;
            this.ioTModuleIdentityInfoSelectionService = ioTModuleIdentityInfoSelectionService;
        }

        public override void Initialize()
        {
           
        }
    }

    public class DesignJsonViewModel : JsonViewModel
    {
        public DesignJsonViewModel() : base(new DesignAzureService(), new DesignIoTHubSelectionService(), new DesignDeviceSelectionService(), new DesignMoluleIdentitySelectionService())
        {

        }
    }
}
