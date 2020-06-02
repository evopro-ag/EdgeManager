using System;
using System.Linq;
using Ninject.Parameters;

namespace EdgeManager.Gui
{
    public interface IInstanceCreator
    {
        T CreateInstance<T>(ConstructorArgument[] arguments);

    }
}