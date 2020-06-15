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

        public SelectionService()
        {
            
        }
        public void Select(T selection)
        {
            selectedObject.OnNext(selection);
        }

        public IObservable<T> SelectedObject => selectedObject;

        public T CurrentSelection
        {
            get => selectedObject.Value;
            set => Select(value);
        }
    }
}