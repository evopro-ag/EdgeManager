using System;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Forms.Design.Behavior;

namespace EdgeManager.Interfaces.Services
{
    public interface IAzureInstallationService //Todo: Eventually rename this
    {
        public IObservable<Unit> RequestInstallation { get; }
        public bool? AzureCliInstalled { get;  }
        public Task<Unit> InstallAzureCli();
    }
}