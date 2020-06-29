using System;
using System.Linq;
using System.Reactive.Linq;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;

namespace EdgeManager.Gui.Design
{
    public class DesignModuleIdentitySelectionService : ISelectionService<IoTModuleIdentityInfo>
    {
        public void Select(IoTModuleIdentityInfo selectedObject)
        {
            throw new NotImplementedException();
        }

        public IObservable<IoTModuleIdentityInfo> SelectedObject => Observable.Return(CurrentSelection);
        public IoTModuleIdentityInfo CurrentSelection { get; set; } = new IoTModuleIdentityInfo();
    }
}
