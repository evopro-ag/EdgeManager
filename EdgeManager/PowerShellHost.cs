using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace EdgeManager
{
    class PowerShellHost : IDisposable
    {
		private readonly PowerShell ps = PowerShell.Create();

		public void Dispose()
		{
			ps.Dispose();
		}

		protected Task<Collection<PSObject>> Execute(string command)
		{
			ps.AddScript(command);
			return Task.FromResult(ps.Invoke());
		}
	}
}
