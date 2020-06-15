using System;
using System.Collections.Generic;
using System.Text;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;
using EdgeManager.Logic.Services;
using Ninject.Modules;

namespace EdgeManager.Logic
{
    public class LogicModuleCatalog : NinjectModule
    {
        public override void Load()
        {
            Bind<IPowerShell>().To<PowerShellHost>().InSingletonScope();
            Bind<IAzureCli, IAzureService>().To<AzureCliHost>().InSingletonScope();

            //todo: add more bindings for selection service
            Bind<ISelectionService<IoTHubInfo>>().To<SelectionService<IoTHubInfo>>().InSingletonScope();
            Bind<ISelectionService<IoTDeviceInfo>>().To<SelectionService<IoTDeviceInfo>>().InSingletonScope();
        }
    }
}
