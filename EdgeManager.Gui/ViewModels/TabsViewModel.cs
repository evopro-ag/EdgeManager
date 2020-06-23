using System;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Text;
using EdgeManager.Interfaces.Commands;
using EdgeManager.Interfaces.Extensions;

namespace EdgeManager.Gui.ViewModels
{
    public class TabsViewModel : ViewModelBase
    {
        private readonly IViewModelFactory viewModelFactory;

        public TabsViewModel(IViewModelFactory viewModelFactory)
        {
            this.viewModelFactory = viewModelFactory;
        }
        public override void Initialize()
        {
            ModuleViewModel = viewModelFactory.Create<ModuleViewModel>();
            ModuleViewModel.AddDisposableTo(Disposables);
            //todo
        }

        public ModuleViewModel ModuleViewModel { get; private set; }
    }
    public class DesignTabsViewModel : TabsViewModel
    {
        public DesignTabsViewModel() : base(ViewModelLocator.DesignViewModelFactory)
        {
        }
    }
}
