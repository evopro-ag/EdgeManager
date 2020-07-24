using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EdgeManager.Interfaces.Services
{
    public interface IApplicationRestartService
    {
        public IObservable<Unit> RestartApplication { get; }
        public bool? RestartAfterCliInstalled { get; }

        public Task<Unit> RestartNow(Application application);
    }
}
