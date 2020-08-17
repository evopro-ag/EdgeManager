using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using EdgeManager.Gui.Design;
using EdgeManager.Interfaces.Extensions;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;
using EdgeManager.Interfaces.Settings;
using ReactiveUI;

namespace EdgeManager.Gui.ViewModels
{
    public class LogInViewModel : ViewModelBase
    {
        private readonly IAzureService azureService;
        private readonly ApplicationSettings settings;
        private AzureAccountInfo accountInfo;
        private bool shouldLogIn = true;
        private readonly BehaviorSubject<Unit> reloadAccountInfo = new BehaviorSubject<Unit>(Unit.Default);
        public ReactiveCommand<Unit, Unit> LogInCommand { get; set; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; set; }
        public CancellationTokenSource CancellationTokenSouce { get; set; }

        public LogInViewModel(IAzureService azureService, ApplicationSettings settings)
        {
            this.azureService = azureService;
            this.settings = settings;
        }

        public AzureAccountInfo AccountInfo
        {
            get => accountInfo;
            set
            {
                if (Equals(value, accountInfo)) return;
                accountInfo = value;
                raisePropertyChanged();
            }
        }

        public override void Initialize()
        {
            LogInCommand = ReactiveCommand.CreateFromTask(PerformAzureLogin)
                    .AddDisposableTo(Disposables)
                ;
            
            CancelCommand = ReactiveCommand.CreateFromTask(CancelLogin)
                    .AddDisposableTo(Disposables)
                ;

            this.WhenAnyValue(model => model.AccountInfo)
                .Where(info => info != null)
                .ObserveOnDispatcher()
                .Do(_ => ShouldLogIn = false)
                .Subscribe()
                .AddDisposableTo(Disposables)
                ;
            
            reloadAccountInfo
                .SelectMany(_ => azureService.GetAccount())
                .ObserveOnDispatcher()
                .Do(account => AccountInfo = account)
                .LogAndRetryAfterDelay(Logger, TimeSpan.FromMilliseconds(100), "Error while getting account information")
                .Subscribe()
                .AddDisposableTo(Disposables)
                ;
            
        }

        private Task CancelLogin()
        {
            CancellationTokenSouce?.Cancel();
            return Task.FromResult(Unit.Default);
        }

        public bool ShouldLogIn
        {
            get => shouldLogIn;
            set
            {
                if (value == shouldLogIn) return;
                shouldLogIn = value;
                raisePropertyChanged();
            }
        }

        private async Task<Unit> PerformAzureLogin()
        {
            try
            {
                CancellationTokenSouce = new CancellationTokenSource();
                await azureService.Login(CancellationTokenSouce.Token);
                reloadAccountInfo.OnNext(Unit.Default);
            }
            catch (Exception e)
            {
                Logger.Error("Error while login", e);
            }

            CancellationTokenSouce.Dispose();
            CancellationTokenSouce = null;
            return Unit.Default;
        }
    }

    internal class DesignLogInViewModel : LogInViewModel
    {
        public DesignLogInViewModel() : base(new DesignAzureService(), new ApplicationSettings())
        {
            base.Initialize();
        }
    }
}
