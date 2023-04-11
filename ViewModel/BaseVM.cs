using CommunityToolkit.Mvvm.ComponentModel;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PnC_Insurance.ViewModel
{
    public partial class BaseVM : ObservableValidator
    {
        [ObservableProperty]
        private SnackbarMessageQueue? resultNotification;
    }
}
