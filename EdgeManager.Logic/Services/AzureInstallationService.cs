using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using EdgeManager.Interfaces.Extensions;
using EdgeManager.Interfaces.Services;
using Ninject;

namespace EdgeManager.Logic.Services
{
    class AzureInstallationService : IAzureInstallationService, IInitializable, IDisposable
    {
        private readonly BehaviorSubject<bool?> azureCheckSubject = new BehaviorSubject<bool?>(null);
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private readonly IAzureService azureService;
        private readonly IPowerShell powerShell;

        public IObservable<Unit> RequestInstallation =>
            azureCheckSubject.Where(b => b.HasValue && !b.Value).Select(_ => Unit.Default);

        public bool? AzureCliInstalled => azureCheckSubject.Value;

        public AzureInstallationService(IAzureService azureService,
            IPowerShell powerShell) //todo: add constructor parameters
        {
            this.azureService = azureService;
            this.powerShell = powerShell;
        }

        public async Task<Unit> InstallAzureCli()
        {
            try
            {
                await powerShell.Execute(
                    @"Invoke-WebRequest -Uri https://aka.ms/installazurecliwindows -OutFile .\AzureCLI.msi; Start-Process msiexec.exe -Wait -ArgumentList '/I AzureCLI.msi /quiet'; rm .\AzureCLI.msi");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return Unit.Default;
        }

        public void Initialize()
        {
            //todo: fill in implementation
            // Start check in background here with Observable.Return()...
            Observable.Return(Unit.Default)
                .SelectMany(async _ => await azureService.CheckCli())
                .Subscribe(b => azureCheckSubject.OnNext(b))
                .AddDisposableTo(disposables);
        }

        public void Dispose()
        {
            disposables?.Dispose();
        }
    }
}