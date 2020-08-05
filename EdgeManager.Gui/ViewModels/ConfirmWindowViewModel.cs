using EdgeManager.Interfaces.Extensions;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EdgeManager.Gui.ViewModels
{
    public class ConfirmWindowViewModel : ViewModelBase
    {
        private bool loading;

        public override void Initialize()
        {
            DeleteCommand = ReactiveCommand.CreateFromTask(DeleteSelectedDevice)
               .AddDisposableTo(Disposables);

            CloseCommand = ReactiveCommand.CreateFromTask(CloseWindow)
                .AddDisposableTo(Disposables);
        }

        public ReactiveCommand<Unit, Unit> DeleteCommand { get; set; }
        public bool DeleteDevice { get; set; }
        private Task DeleteSelectedDevice()
        {
            DeleteDevice = true;
            return CloseWindow();
        }

        public ReactiveCommand<Unit, Unit> CloseCommand { get; set; }
        private Task CloseWindow()
        {
            Window.Close();
            return Task.FromResult(Unit.Default);
        }

        public Window Window { get; set; }
    }
}
