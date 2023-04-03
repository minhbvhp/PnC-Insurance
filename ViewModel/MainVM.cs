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
        partial void OnMenuItemSelectedChanged(CustomMenuItem? value)
        {
            switch (value.ContentViewModels)
            {
                case UrnVM urn:
                    SelectedVM = new UrnVM();
                    break;
                default:
                    SelectedVM = new BaseVM();
                    break;
            }
        }

        [ObservableProperty]
        BaseVM? selectedVM;
        public ObservableCollection<CustomMenuItem>? CustomMenuItems { get; private set; }
        private void CreateMenu()
        {
            MenuItemSelected = new CustomMenuItem(new BaseVM(), "Quản lý cấp đơn Tài sản kỹ thuật", MaterialDesignThemes.Wpf.PackIconKind.Pirate);
            CustomMenuItems = new ObservableCollection<CustomMenuItem>();
            CustomMenuItems.Add(new CustomMenuItem(new BaseVM(), "Bản chào", MaterialDesignThemes.Wpf.PackIconKind.AlphaQBoxOutline));
            CustomMenuItems.Add(new CustomMenuItem(new BaseVM(), "Giấy chứng nhận", MaterialDesignThemes.Wpf.PackIconKind.FileDocumentCheck));
            CustomMenuItems.Add(new CustomMenuItem(new BaseVM(), "Hợp đồng", MaterialDesignThemes.Wpf.PackIconKind.FileSign));
            CustomMenuItems.Add(new CustomMenuItem(new BaseVM(), "Khách hàng", MaterialDesignThemes.Wpf.PackIconKind.AccountOutline));
            CustomMenuItems.Add(new CustomMenuItem(new UrnVM(), "URN", MaterialDesignThemes.Wpf.PackIconKind.QrcodeScan));
        }        

        public MainVM()
        {
            CreateMenu();
        }
    }
}