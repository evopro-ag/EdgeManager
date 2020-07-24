using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
        private readonly BehaviorSubject<bool?> restartCheckSubject = new BehaviorSubject<bool?>(null);
        private readonly IAzureService azureService;
        private readonly IViewModelFactory viewModelFactory;
        private readonly IAzureInstallationService azureInstallationService;
        private readonly IApplicationRestartService applicationRestartService;


        public MainWindowViewModel(IViewModelFactory viewModelFactory, IAzureService azureService, IAzureInstallationService azureInstallationService) //todo: add constructor parameter for IAzureInstallationService
        {
            this.viewModelFactory = viewModelFactory;
            this.azureService = azureService;
            this.azureInstallationService = azureInstallationService;
            this.applicationRestartService = applicationRestartService;

        }
        public IObservable<Unit> RestartApplication =>
            restartCheckSubject.Where(b => b.HasValue && b.Value).Select(_ => Unit.Default);

        public bool? RestartAfterCliInstalled => restartCheckSubject.Value;

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
                .Subscribe(_ => restartCheckSubject.OnNext(true))
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
