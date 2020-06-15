using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using EdgeManager.Gui.Design;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;
using ReactiveUI;

namespace EdgeManager.Gui.ViewModels
{
    public class ModuleViewModel : ViewModelBase
    {
        private readonly ISelectionService<IoTDeviceInfo> selectionService;
        private ObservableAsPropertyHelper<IoTDeviceInfo> selectedIotDeviceHelper;

        public ModuleViewModel(ISelectionService<IoTDeviceInfo> selectionService)
        {
            this.selectionService = selectionService;
        }
        public override void Initialize()
        {
            selectedIotDeviceHelper = selectionService.SelectedObject.ObserveOnDispatcher().ToProperty(this, vm => vm.SelectedIotDevice);
        }

        public IoTDeviceInfo SelectedIotDevice => selectedIotDeviceHelper.Value;
    }

    public class DesignModuleViewModel : ModuleViewModel
    {
        public DesignModuleViewModel() : base(new DesignDeviceSelectionService())
        {
        }
    }
}
