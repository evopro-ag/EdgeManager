using System;
using System.Management.Automation;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using EdgeManager.Interfaces.Extensions;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;

namespace EdgeManager.Logic.Services
{
    public class CommandHandler : ICommandHandler
    {
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private PowerShell ps = PowerShell.Create();
        private readonly Subject<PercentageInformation> percentageCompleted = new Subject<PercentageInformation>();
        private readonly Subject<Exception> errorSubject = new Subject<Exception>();
        private readonly Subject<string> outputSubject = new Subject<string>();
        private IAsyncResult result;
        private readonly CancellationDisposable cancellationTokens = new CancellationDisposable();

        public CommandHandler(string command)
        {
            cancellationTokens.AddDisposableTo(disposables);

            Observable.FromEventPattern<DataAddedEventArgs>(h => ps.Streams.Progress.DataAdded += h,
                    h => ps.Streams.Progress.DataAdded -= h)
                .Select(arg =>
                {
                    var progressRecord = ((PSDataCollection<ProgressRecord>) arg.Sender)[arg.EventArgs.Index];
                    return new PercentageInformation
                        {Percentage = progressRecord.PercentComplete, Description = progressRecord.StatusDescription};
                })
                .Subscribe(percentageCompleted.OnNext)
                .AddDisposableTo(disposables);
		    
            Observable.FromEventPattern<DataAddedEventArgs>(h => ps.Streams.Error.DataAdded += h,
                    h => ps.Streams.Error.DataAdded -= h)
                .Select(arg => ((PSDataCollection<ErrorRecord>)arg.Sender)[arg.EventArgs.Index].Exception)
                .Subscribe(errorSubject.OnNext)
                .AddDisposableTo(disposables);

            PSDataCollection<PSObject> output = new PSDataCollection<PSObject>();
            
            Observable.FromEventPattern<DataAddedEventArgs>(h => output.DataAdded += h,
                    h => output.DataAdded -= h)
                .Select(arg => ((PSDataCollection<PSObject>) arg.Sender)[arg.EventArgs.Index])
                .Select(po => po.ToString())
                .Subscribe(outputSubject.OnNext)
                .AddDisposableTo(disposables);

            ps.AddScript(command);

            Task.Run(() =>
            {
                result = ps.BeginInvoke<PSObject, PSObject>(null, output);
            }, cancellationTokens.Token)
                .Wait();
        }

        public void Dispose()
        {
            disposables?.Dispose();
            outputSubject?.Dispose();
            percentageCompleted?.Dispose();
            errorSubject?.Dispose();
            ps?.Dispose();
        }

        public IObservable<Exception> Errors => errorSubject.AsObservable();
        public IObservable<PercentageInformation> PercentageCompleted => percentageCompleted.AsObservable();
        public IObservable<string> OutputLines => outputSubject.AsObservable();
        
        /// <summary>
        /// Wait for the ps command to end (blocking)
        /// </summary>
        public void Wait()
        {
            result.AsyncWaitHandle.WaitOne();
        }
    }
}