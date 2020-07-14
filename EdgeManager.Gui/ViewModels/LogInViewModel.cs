using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using EdgeManager.Gui.Design;
using EdgeManager.Interfaces.Extensions;
using EdgeManager.Interfaces.Services;
using ReactiveUI;

namespace EdgeManager.Gui.ViewModels
{
    public class LogInViewModel : ViewModelBase
    {
        private readonly IAzureService azureService;
        public ReactiveCommand<Unit, Unit> LogInCommand { get; set; }
        

        public LogInViewModel(IAzureService azureService)
        {
            this.azureService = azureService;
        }

        public override void Initialize()
        {
            LogInCommand = ReactiveCommand.CreateFromTask(PerformAzureLogin)
                    .AddDisposableTo(Disposables)
                ;
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
