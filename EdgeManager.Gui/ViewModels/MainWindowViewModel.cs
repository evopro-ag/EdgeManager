using System;
using System.Collections.Generic;
using System.Text;
using EdgeManager.Interfaces.Commons;
using EdgeManager.Interfaces.Services;

namespace EdgeManager.Gui.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IViewModelFactory viewModelFactory;
        private readonly IPowerShell powerShell;

        public MainWindowViewModel(IViewModelFactory viewModelFactory)
        {
            this.viewModelFactory = viewModelFactory;
        }

        public ConnectionCabViewModel ConnectionCabViewModel { get; private set; }

        public override void Initialize()
        {
            ConnectionCabViewModel = viewModelFactory.CreateViewModel<ConnectionCabViewModel>();
        }
    }

    public class DesignMainWindowViewModel : MainWindowViewModel
    {
        public DesignMainWindowViewModel() : base(ViewModelLocator.DesignViewModelFactory)
        {
        }
    }
}
