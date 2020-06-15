using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeManager.Interfaces.Services
{
    public interface ISelectionService<T>
    {
        void Select(T selectedObject);

        IObservable<T> SelectedObject { get; }
        T CurrentSelection { get; set; }
    }
}
