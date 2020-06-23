using System;
using System.Linq;
using Ninject.Parameters;

namespace EdgeManager.Interfaces.Commons
{
    public interface IInstanceCreator
    {
        T CreateInstance<T>(ConstructorArgument[] arguments);
    }
}