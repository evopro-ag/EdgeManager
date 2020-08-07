using System;
using System.Management.Automation;
using EdgeManager.Interfaces.Models;

namespace EdgeManager.Interfaces.Services
{
    public interface ICommandHandler : IDisposable
    {
        public IObservable<Exception> Errors { get; }
        public IObservable<PercentageInformation> PercentageCompleted { get; }
        public IObservable<string> OutputLines { get; }
        public void Wait();

    }
}