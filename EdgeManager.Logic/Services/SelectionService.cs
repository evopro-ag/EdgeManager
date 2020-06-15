using System;
using System.Linq;
using System.Reactive.Subjects;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;

namespace EdgeManager.Logic.Services
{
    public class SelectionService<T> : ISelectionService<T>
    {
        BehaviorSubject<T> selectedObject = new BehaviorSubject<T>(default(T));

        public void Select(T selectedObject)
        {
            selectedObject.OnNext(selectedObject)
        }

        public IObservable<T> SelectedObject => selectedObject;

        public T CurrentSelection
        {
            get => selectedObject.Value;
            set => Select(value);
        }
    }
}