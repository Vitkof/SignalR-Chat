using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfClient.ViewModels.Base;


namespace WpfClient.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        private string _displayHeader = "WPF Client";

        public string DisplayHeader
        {
            get => _displayHeader;
            set => Set(ref _displayHeader, value);
        }
    }
}
