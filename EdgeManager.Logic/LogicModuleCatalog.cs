using System;
using System.Collections.Generic;
using System.Text;
using EdgeManager.Interfaces.Logging;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;
using EdgeManager.Logic.Services;
using log4net;
using Ninject.Modules;

namespace EdgeManager.Logic
{
    public class LogicModuleCatalog : NinjectModule
    {
        public override void Load()
        {
            Bind<IPowerShell>().To<PowerShellHost>().InSingletonScope();
            Bind<IAzureCli, IAzureService>().To<AzureCliHost>().InSingletonScope();

            //bindings for selection service
            Bind<ISelectionService<IoTHubInfo>>().To<SelectionService<IoTHubInfo>>().InSingletonScope();
            Bind<ISelectionService<IoTDeviceInfo>>().To<SelectionService<IoTDeviceInfo>>().InSingletonScope();
            Bind<ISelectionService<IoTModuleIdentityInfo>>().To<SelectionService<IoTModuleIdentityInfo>>().InSingletonScope();
            Bind<ISelectionService<JsonCommand>>().To<SelectionService<JsonCommand>>().InSingletonScope();
         
            Bind<IAzureInstallationService>().To<AzureInstallationService>().InSingletonScope();

            Bind<ILog>().ToMethod(context => LoggerFactory.GetLogger());
        }
    }
}
