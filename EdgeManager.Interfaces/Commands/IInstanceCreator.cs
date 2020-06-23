using System;
using System.Linq;
using Ninject.Parameters;

namespace EdgeManager.Interfaces.Commands
{
    public interface IInstanceCreator
    {
        T CreateInstance<T>(ConstructorArgument[] arguments);
    }
}