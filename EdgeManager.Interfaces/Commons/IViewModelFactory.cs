using System;
using System.Linq;

namespace EdgeManager.Gui
{
    public interface IViewModelFactory
    {
        T Create<T>();

        TVm CreateViewModel<T, TVm>(T model);

        TVm CreateViewModel<TVm>();
    }
}