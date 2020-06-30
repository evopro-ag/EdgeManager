using System;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Text;
using EdgeManager.Interfaces.Commons;
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

            JsonViewModel = viewModelFactory.CreateViewModel<JsonViewModel>();
            JsonViewModel.AddDisposableTo(Disposables);
            //todo
        }

        public ModuleViewModel ModuleViewModel { get; private set; }
        public JsonViewModel JsonViewModel { get; private set; }
    }
    internal class DesignTabsViewModel : TabsViewModel
    {
        public DesignTabsViewModel() : base(ViewModelLocator.DesignViewModelFactory)
        {
        }
    }
}
