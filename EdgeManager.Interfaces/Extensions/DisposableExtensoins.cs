using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace EdgeManager.Interfaces.Extensions
{
    public static class DisposableExtensoins
    {
        public static T AddDisposableTo<T>(this T disposable, CompositeDisposable disposables) where T : IDisposable
        {
            disposables.Add(disposable);
            return disposable;
        }
    }
}
