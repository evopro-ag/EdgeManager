using System;
using System.Reactive.Linq;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;

namespace EdgeManager.Gui.Design
{
    internal class DesignCommandHandler : ICommandHandler
    {
        public void Dispose()
        {
        }

        public IObservable<Exception> Errors => Observable.Empty<Exception>();
        public IObservable<PercentageInformation> PercentageCompleted => Observable.Empty<PercentageInformation>();
        public IObservable<string> OutputLines => Observable.Empty<string>(); 
        public void Wait()
        {
        }
    }
}