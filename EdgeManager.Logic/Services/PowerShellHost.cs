using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using EdgeManager.Interfaces.Services;
using log4net;

namespace EdgeManager.Logic.Services
{ 
    class PowerShellHost : IPowerShell
	{
        private readonly ILog logger;
        private readonly PowerShell ps = PowerShell.Create();

        public PowerShellHost(ILog logger)
        {
            this.logger = logger;
        }
		public void Dispose()
		{
			ps.Dispose();
		}

		public Task<Collection<PSObject>> Execute(string command)
		{
			logger.Debug($"Executing command '{command}' into power shell...");
			ps.AddScript(command);
			return Task.Run(() => ps.Invoke());
		}
	}
}
