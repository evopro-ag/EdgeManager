using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using EdgeManager.Interfaces.Services;

namespace EdgeManager.Logic.Services
{ 
    class PowerShellHost : IPowerShell
	{
		private readonly PowerShell ps = PowerShell.Create();

		public void Dispose()
		{
			ps.Dispose();
		}

		public Task<Collection<PSObject>> Execute(string command)
		{
			ps.AddScript(command);
			return Task.FromResult(ps.Invoke());
		}
	}
}
