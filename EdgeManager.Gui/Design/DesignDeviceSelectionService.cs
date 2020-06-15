using System;
using System.Linq;
using System.Reactive.Linq;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;

namespace EdgeManager.Gui.Design
{
    public class DesignDeviceSelectionService : ISelectionService<IoTDeviceInfo>
    {
        public void Select(IoTDeviceInfo selectedObject)
        {
            throw new NotImplementedException();
        }

        public IObservable<IoTDeviceInfo> SelectedObject => Observable.Return(CurrentSelection);
        public IoTDeviceInfo CurrentSelection { get; set; } = new IoTDeviceInfo();
    }
}