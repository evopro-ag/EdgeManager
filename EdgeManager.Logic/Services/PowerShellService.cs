using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;
using log4net;

namespace EdgeManager.Logic.Services
{ 
    class PowerShellService : IPowerShellService
	{
        private readonly ILog logger;
        private readonly Subject<PercentageInformation> percentageCompleted = new Subject<PercentageInformation>();
        private readonly Subject<Exception> errorSubject = new Subject<Exception>();
        public IObservable<PercentageInformation> PercentageCompleted => percentageCompleted.AsObservable();
        public IObservable<Exception> Errors => errorSubject.AsObservable();
        
        public PowerShellService(ILog logger)
        {
            this.logger = logger;

            // Observable.FromEventPattern<DataAddedEventArgs>(h => ps.Streams.Progress.DataAdded += h,
	           //  h => ps.Streams.Progress.DataAdded -= h)
	           //  .Select(arg =>
	           //  {
		          //   var progressRecord = ((PSDataCollection<ProgressRecord>) arg.Sender)[arg.EventArgs.Index];
		          //   return new PercentageInformation {Percentage = progressRecord.PercentComplete, Description = progressRecord.StatusDescription};
	           //  })
	           //  .Subscribe(percentageCompleted.OnNext);
            //
            // Observable.FromEventPattern<DataAddedEventArgs>(h => ps.Streams.Progress.DataAdded += h,
		          //   h => ps.Streams.Error.DataAdded -= h)
	           //  .Select(arg => ((PSDataCollection<ErrorRecord>)arg.Sender)[arg.EventArgs.Index].Exception)
	           //  .Subscribe(errorSubject.OnNext);

            
        }

		public async Task<Collection<PSObject>> Execute(string command)
		{
			var ps = PowerShell.Create();
			logger.Debug($"Executing command '{command}' into power shell...");
			ps.AddScript(command);
			var result = await Task.Run(() => ps.Invoke());
			ps.Dispose();
			return result;
		}
	}
}
