using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeManager.Gui.Design;
using EdgeManager.Interfaces.Extensions;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;
using ReactiveUI;

namespace EdgeManager.Gui.ViewModels
{
    public class LogInViewModel : ViewModelBase
    {
        private readonly IAzureService azureService;
        private AzureAccountInfo accountInfo;
        private bool shouldLogIn = true;
        public ReactiveCommand<Unit, Unit> LogInCommand { get; set; }
        

        public LogInViewModel(IAzureService azureService)
        {
            this.azureService = azureService;
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

            this.WhenAnyValue(model => model.AccountInfo)
                .Where(info => info != null)
                .ObserveOnDispatcher()
                .Do(_ => ShouldLogIn = false)
                .Subscribe()
                .AddDisposableTo(Disposables)
                ;
            
            Observable.Timer(TimeSpan.FromSeconds(1))
                .SelectMany(_ => azureService.GetAccount())
                .ObserveOnDispatcher()
                .Do(account => AccountInfo = account)
                .LogAndRetryAfterDelay(Logger, TimeSpan.FromMilliseconds(100), "Error while getting account information")
                .Subscribe()
                .AddDisposableTo(Disposables)
                ;
            
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
                await azureService.Login();
            }
            catch (Exception e)
            {
                Logger.Error("Error while login", e);
            }

            return Unit.Default;
        }
    }

    internal class DesignLogInViewModel : LogInViewModel
    {
        public DesignLogInViewModel() : base(new DesignAzureService())
        {
            base.Initialize();
        }
    }
}
