using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using EdgeManager.Gui.Design;
using EdgeManager.Interfaces.Commons;
using EdgeManager.Interfaces.Extensions;
using EdgeManager.Interfaces.Services;
using MessageBox = System.Windows.MessageBox;

namespace EdgeManager.Gui.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IAzureService azureService;
        private readonly IViewModelFactory viewModelFactory;
        private bool CliInstalled;
        //private DialogResult dialogResult;

        public MainWindowViewModel(IViewModelFactory viewModelFactory, IAzureService azureService) //todo: add constructor parameter for IAzureInstallationService
        {
            this.viewModelFactory = viewModelFactory;
            this.azureService = azureService;
        }

        public LogInViewModel LogInViewModel { get; set; }
        public TabsViewModel TabsViewModel { get; set; }
        public HubViewModel HubViewModel { get; set; }
        public DeviceViewModel DeviceViewModel { get; set; }

        public override void Initialize()
        {
            Logger.Debug("Initialize main window view model");

            TabsViewModel = viewModelFactory.CreateViewModel<TabsViewModel>();
            HubViewModel = viewModelFactory.CreateViewModel<HubViewModel>();
            DeviceViewModel = viewModelFactory.CreateViewModel<DeviceViewModel>();
            LogInViewModel = viewModelFactory.CreateViewModel<LogInViewModel>();

            CheckCliInstalled().Wait(); //todo: remove this and do the thing below instead
            
            //todo: add subscription to IAzureInstallationService.RequestInstallation
            //in case of a positive response from user install from IAzureInstallationService.InstallAzureCli
            //after installation completed restart the software
            
        }

        private async Task<Unit> CheckCliInstalled()
        {
            try
            {
                CliInstalled = await azureService.CheckCli();
                if (!CliInstalled)
                {
                    var dialogResult = MessageBox.Show("AzureCli is not installed", Title, MessageBoxButton.YesNo);
                    if (dialogResult == MessageBoxResult.Yes)
                    {
                        await azureService.InstallCli();
                    }
                    else if (dialogResult == MessageBoxResult.No)
                    {
                        //do something else
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("AzureCli is not installed", e);
            }

            return Unit.Default;
        }
    }


    internal class DesignMainWindowViewModel : MainWindowViewModel
    {
        public DesignMainWindowViewModel() : base(ViewModelLocator.DesignViewModelFactory, new DesignAzureService())
        {
            base.Initialize();
        }
    }
}
