using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Threading;

namespace EdgeManager.Gui.ViewModels
{
    public class JsonViewModel : ViewModelBase
    {
        private string text;

        public string Text
        {
            get => text;
            set
            {
                if (value == text) return;
                text = value;
                raisePropertyChanged();
            }
        }

        public override void Initialize()
        {
            Disposables.Add(
                Observable.Interval(TimeSpan.FromSeconds(1))
                                .Do(l => Text = "test " + l.ToString())
                                .Subscribe()
            );
        }
    }

    public class DesignJsonViewModel : JsonViewModel
    {
        public DesignJsonViewModel()
        {
            Text = "WooW!";

        }
    }
}
