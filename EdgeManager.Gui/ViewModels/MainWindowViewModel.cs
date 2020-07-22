using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using ReactiveUI;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace EdgeManager.Gui.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IAzureService azureService;
        private readonly IViewModelFactory viewModelFactory;
        private readonly IAzureInstallationService azureInstallationService;

        public MainWindowViewModel(IViewModelFactory viewModelFactory, IAzureService azureService, IAzureInstallationService azureInstallationService) //todo: add constructor parameter for IAzureInstallationService
        {
            this.viewModelFactory = viewModelFactory;
            this.azureService = azureService;
            this.azureInstallationService = azureInstallationService;
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

            
            //todo: add subscription to IAzureInstallationService.RequestInstallation
            //in case of a positive response from user install from IAzureInstallationService.InstallAzureCli
            //after installation completed restart the software
            var observable = azureInstallationService.RequestInstallation

                .ObserveOnDispatcher()
                .Select(_ => AskForInstallationPermission());

            observable.Where(b => b)
                .SelectMany(async _ => await azureInstallationService.InstallAzureCli())
                .SelectMany(_ => Restart())
                .Subscribe()
                .AddDisposableTo(Disposables);

            observable.Where(b => !b)
                .SelectMany(_ => ShutdownApp())
                .Subscribe()
                .AddDisposableTo(Disposables);
        }


        private bool AskForInstallationPermission()
        {
            var dialogResult = MessageBox.Show("AzureCli is not installed", Title, MessageBoxButton.YesNo);
            if (dialogResult == MessageBoxResult.Yes)
            {
                return true;
            }
            return false;
        }

        private async Task<Unit> Restart()
        {
            Process.Start(Path.Combine(Path.GetDirectoryName(Application.ResourceAssembly.Location), "EdgeManager.exe"));
            Logger.Error("Application restarted after install AzureCli");
            try
            {
                Application.Current.Shutdown();
            }
            catch (Exception e)
            {
                Logger.Error("Error in Restart: did not shutdown", e);
            }

            return Unit.Default;
        }

        private async Task<Unit> ShutdownApp()
        {
            Application.Current.Shutdown();
            return Unit.Default;
        }
    }

    //internal class DesignMainWindowViewModel : MainWindowViewModel
    //{
    //    public DesignMainWindowViewModel() : base(ViewModelLocator.DesignViewModelFactory, new DesignAzureService(), )
    //    {
    //        base.Initialize();
    //    }
    //}
}
