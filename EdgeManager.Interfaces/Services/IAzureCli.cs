using System;
using System.Linq;
using System.Threading.Tasks;

namespace EdgeManager.Interfaces.Services
{
    public interface IAzureCli
    {
        Task<T> Run<T>(string command);
    }
}