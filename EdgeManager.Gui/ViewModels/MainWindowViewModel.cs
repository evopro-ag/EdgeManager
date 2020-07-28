using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using EdgeManager.Interfaces.Commons;
using EdgeManager.Interfaces.Extensions;
using EdgeManager.Interfaces.Services;
using MessageBox = System.Windows.MessageBox;

namespace EdgeManager.Gui.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly Subject<Unit> restartCheckSubject = new Subject<Unit>();
        private readonly IViewModelFactory viewModelFactory;
        private readonly IAzureInstallationService azureInstallationService;


        public MainWindowViewModel(IViewModelFactory viewModelFactory, IAzureInstallationService azureInstallationService)
        {
            this.viewModelFactory = viewModelFactory;
            this.azureInstallationService = azureInstallationService;

        }
        public IObservable<Unit> RestartApplication => restartCheckSubject.AsObservable();

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

            
            azureInstallationService.RequestInstallation
                .ObserveOnDispatcher()
                .Select(_ => AskForInstallationPermission())
                .Where(b => b)
                .SelectMany(_ => azureInstallationService.InstallAzureCli())
                .Subscribe(restartCheckSubject.OnNext)
                .AddDisposableTo(Disposables);
        }


        private bool AskForInstallationPermission()
        {
            var dialogResult = MessageBox.Show("Would you like to install it?", "AzureCli is not installed", MessageBoxButton.YesNo);
            return dialogResult == MessageBoxResult.Yes;
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
