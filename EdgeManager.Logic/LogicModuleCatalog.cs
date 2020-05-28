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
            Bind<IoTHubInfo>().To<IoTHubInfo>().InSingletonScope();
            Bind<IoTDeviceIdentityInfo>().To<IoTDeviceIdentityInfo>().InSingletonScope();
            Bind<IoTDirectMethodReply>().To<IoTDirectMethodReply>().InSingletonScope();
            Bind<IoTHubProperties>().To<IoTHubProperties>().InSingletonScope();
            Bind<IoTModuleIdentityInfo>().To<IoTModuleIdentityInfo>().InSingletonScope();
            //todo: add more bindings
        }
    }
}
