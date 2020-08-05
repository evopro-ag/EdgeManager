using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EdgeManager.Gui.Views;
using EdgeManager.Interfaces.Extensions;
using ReactiveUI;

namespace EdgeManager.Gui.ViewModels
{
    public class InputWindowDeviceViewModel : ViewModelBase
    {
        private string newDeviceName;

        public override void Initialize()
        {
            AddDeviceCommand = ReactiveCommand.CreateFromTask(CloseAndAddDevice)
                .AddDisposableTo(Disposables);

            CloseCommand = ReactiveCommand.CreateFromTask(CloseWindow)
                .AddDisposableTo(Disposables);
        }

      
        public ReactiveCommand<Unit, Unit> CloseCommand { get; set; }

        private Task CloseWindow()
        {
            Window.Close();
            return Task.FromResult(Unit.Default);
        }

        public ReactiveCommand<Unit, Unit> AddDeviceCommand { get; set; }

        public bool CanAddDevice { get; set; }

        private Task CloseAndAddDevice()
        {
            CanAddDevice = true;
            return CloseWindow();
        }

        public string NewDeviceName
        {
            get => newDeviceName;
            set
            {
                if (value == newDeviceName) return;
                newDeviceName = value;
                raisePropertyChanged();
            }
        }

        public Window Window { get; set; }
    }
}
