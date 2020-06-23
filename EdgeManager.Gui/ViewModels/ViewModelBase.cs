using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using EdgeManager.Interfaces.Logging;
using JetBrains.Annotations;
using log4net;
using Ninject;
using ReactiveUI;

namespace EdgeManager.Gui.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject, IDisposable, IInitializable
    {
        protected CompositeDisposable Disposables = new CompositeDisposable();
        private bool disposed;
        private string title;

        public ViewModelBase()
        {
            Logger = LoggerFactory.GetLogger(GetType());
        }

        public string Title
        {
            get => title;
            set
            {
                if (value == title) return;
                title = value;
                raisePropertyChanged();
            }
        }

        protected ILog Logger { get; }

        public virtual void Dispose()
        {
            Dispose(true);
        }

        public abstract void Initialize();

        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "Disposables")]
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            Disposables?.Dispose();
            Disposables = null;

            disposed = true;
        }

        [NotifyPropertyChangedInvocator]
        // ReSharper disable once InconsistentNaming
        protected void raisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.RaisePropertyChanged(propertyName);
        }

        ~ViewModelBase()
        {
            Dispose(false);
        }
    }
}