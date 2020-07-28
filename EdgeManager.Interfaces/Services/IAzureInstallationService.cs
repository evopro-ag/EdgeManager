using System;
using System.Reactive;
using System.Threading.Tasks;

namespace EdgeManager.Interfaces.Services
{
    public interface IAzureInstallationService
    {
        public IObservable<Unit> RequestInstallation { get; }
        public bool? AzureCliInstalled { get;  }
        public Task<Unit> InstallAzureCli();
    }
}