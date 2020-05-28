using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace EdgeManager.Interfaces.Services
{
    public interface IPowerShell : IDisposable
    {
        Task<Collection<PSObject>> Execute(string command);

    }
}