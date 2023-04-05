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
        CustomSubItem? subItemSelected;

        partial void OnSubItemSelectedChanged(CustomSubItem? value)
        {
            switch (value.ContentViewModels)
            {
                case UrnInfoVM urn:
                    SelectedVM = new UrnInfoVM();
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
            CustomMenuItems = new ObservableCollection<CustomMenuItem>();
            CustomMenuItems.Add(new CustomMenuItem(new BaseVM(), "Bản chào", MaterialDesignThemes.Wpf.PackIconKind.AlphaQBoxOutline));
            CustomMenuItems.Add(new CustomMenuItem(new BaseVM(), "Giấy chứng nhận", MaterialDesignThemes.Wpf.PackIconKind.FileDocumentCheck));
            CustomMenuItems.Add(new CustomMenuItem(new BaseVM(), "Hợp đồng", MaterialDesignThemes.Wpf.PackIconKind.FileSign));
            CustomMenuItems.Add(new CustomMenuItem(new UrnInfoVM(), "Khách hàng", MaterialDesignThemes.Wpf.PackIconKind.AccountOutline));
            CustomMenuItems.Add(new CustomMenuItem("URN", MaterialDesignThemes.Wpf.PackIconKind.QrcodeScan
                                , new List<CustomSubItem>
                                {
                                    new CustomSubItem(new UrnInfoVM(), "Tra cứu"),
                                    new CustomSubItem(new BaseVM(), "Sửa đổi"),
                                }));
        }        

        public MainVM()
        {
            CreateMenu();
        }
    }
}