using CommunityToolkit.Mvvm.ComponentModel;
using PnC_Insurance.MenuItem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PnC_Insurance.ViewModel
{
    public partial class MainVM : BaseVM
    {
        [ObservableProperty]
        CustomMenuItem? menuItemSelected;
        public ObservableCollection<CustomMenuItem>? CustomMenuItems { get; private set; }
        private void CreateMenu()
        {
            MenuItemSelected = new CustomMenuItem(new BaseVM(), "Quản lý cấp đơn Tài sản kỹ thuật", MaterialDesignThemes.Wpf.PackIconKind.Pirate);
            CustomMenuItems = new ObservableCollection<CustomMenuItem>();
            CustomMenuItems.Add(new CustomMenuItem(new BaseVM(), "Tạo mới", MaterialDesignThemes.Wpf.PackIconKind.Add));
        }

        public MainVM()
        {
            CreateMenu();
        }
    }
}
