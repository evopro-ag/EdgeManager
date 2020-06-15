using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;

namespace EdgeManager.Gui.Design
{
    class DesignSelectionService : ISelectionService<IoTHubInfo>
    {
        public void Select(IoTHubInfo selectedObject)
        {
            throw new NotImplementedException();
        }

        public IObservable<IoTHubInfo> SelectedObject => Observable.Return(CurrentSelection);
        public IoTHubInfo CurrentSelection { get; set; } = new IoTHubInfo();
    }
}
