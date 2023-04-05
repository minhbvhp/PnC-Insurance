using CommunityToolkit.Mvvm.ComponentModel;
using PnC_Insurance.MenuItem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;

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
                case UrnInfoVM urnInfo:
                    SelectedVM = new UrnInfoVM();
                    break;
                case CustomerInfoVM customerInfo:
                    SelectedVM = new CustomerInfoVM();
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
            CustomMenuItems.Add(new CustomMenuItem(new BaseVM(), "Bản chào", PackIconKind.AlphaQBoxOutline));
            CustomMenuItems.Add(new CustomMenuItem(new BaseVM(), "Giấy chứng nhận", PackIconKind.FileDocumentCheck));
            CustomMenuItems.Add(new CustomMenuItem(new BaseVM(), "Hợp đồng", PackIconKind.FileSign));

            CustomMenuItems.Add(new CustomMenuItem("Khách hàng", PackIconKind.AccountOutline
                                , new List<CustomSubItem>
                                {
                                    new CustomSubItem(new CustomerInfoVM(), "Tra cứu"),
                                    new CustomSubItem(new BaseVM(), "Sửa đổi"),
                                }));

            CustomMenuItems.Add(new CustomMenuItem("URN", PackIconKind.QrcodeScan
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