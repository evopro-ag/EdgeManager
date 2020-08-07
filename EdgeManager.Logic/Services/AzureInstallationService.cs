using System;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows;
using EdgeManager.Interfaces.Extensions;
using EdgeManager.Interfaces.Services;
using log4net;
using Ninject;

namespace EdgeManager.Logic.Services
{
    class AzureInstallationService : IAzureInstallationService, IInitializable, IDisposable
    {
        private readonly BehaviorSubject<bool?> azureCheckSubject = new BehaviorSubject<bool?>(null);
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private readonly IAzureService azureService;
        private readonly IPowerShellService powerShellService;
        private readonly ILog logger;

        public IObservable<Unit> RequestInstallation =>
            azureCheckSubject.Where(b => b.HasValue && !b.Value).Select(_ => Unit.Default);

        public bool? AzureCliInstalled => azureCheckSubject.Value;

       
        public AzureInstallationService(IAzureService azureService,
            IPowerShellService powerShellService, ILog logger)
        {
            this.azureService = azureService;
            this.powerShellService = powerShellService;
            this.logger = logger;
        }

        public async Task<Unit> InstallAzureCli()
        {
            try
            {
                logger.Debug("Installing AzureCli...");
                
                await powerShellService.Execute(
                    @"Invoke-WebRequest -Uri https://aka.ms/installazurecliwindows -OutFile .\AzureCLI.msi; Start-Process msiexec.exe -Wait -ArgumentList '/I AzureCLI.msi /quiet'; rm .\AzureCLI.msi");
            }
            catch (Exception e)
            {
                logger.Error("AzureCli can not be installed!", e);
            }
            return Unit.Default;
        }

        public void Initialize()
        {
            Observable.Return(Unit.Default)
                .SelectMany(_ => azureService.CheckCli())
                .Subscribe(b => azureCheckSubject.OnNext(b))
                .AddDisposableTo(disposables);
        }

        public void Dispose()
        {
            disposables?.Dispose();
        }
    }
}