using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using EdgeManager.Interfaces.Models;

namespace EdgeManager.Interfaces.Services
{
    public interface IPowerShellService
    {
        Task<Collection<PSObject>> Execute(string command);

        IObservable<PercentageInformation> PercentageCompleted { get; }
        IObservable<Exception> Errors { get; }
    }
}