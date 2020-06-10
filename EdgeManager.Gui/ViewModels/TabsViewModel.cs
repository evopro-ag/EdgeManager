using System;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Text;

namespace EdgeManager.Gui.ViewModels
{
    public class TabsViewModel : ViewModelBase
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
            //todo
        }
    }
    public class DesignTabsViewModel : TabsViewModel
    {
        public DesignTabsViewModel() 
        {
            Text = "WooW!";
        }
    }
}
