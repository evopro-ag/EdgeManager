using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;
using log4net;

namespace EdgeManager.Logic.Services
{ 
    class PowerShellService : IPowerShellService
	{
        private readonly ILog logger;
        public PowerShellService(ILog logger)
        {
            this.logger = logger;
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

		public ICommandHandler ExecuteAsync(string command)
		{
			logger.Debug($"Executing command '{command}' into (async) power shell...");
			return new CommandHandler(command);
		}
	}
}
