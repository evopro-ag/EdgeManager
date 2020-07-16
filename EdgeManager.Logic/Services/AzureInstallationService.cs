using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using EdgeManager.Interfaces.Services;
using Ninject;

namespace EdgeManager.Logic.Services
{
    class AzureInstallationService : IAzureInstallationService, IInitializable, IDisposable
    {
        private readonly BehaviorSubject<bool?> azureCheckSubject = new BehaviorSubject<bool?>(null);
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        public IObservable<Unit> RequestInstallation => azureCheckSubject.Where(b => b.HasValue && !b.Value).Select(_ =>Unit.Default);

        public bool? AzureCliInstalled => azureCheckSubject.Value;

        public AzureInstallationService() //todo: add constructor parameters
        {

        }

        public Task<Unit> InstallAzureCli()
        {
            //todo: fill in implementation
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            //todo: fill in implementation
            // Start check in background here with Observable.Return()...
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            disposables?.Dispose();
        }
    }
}