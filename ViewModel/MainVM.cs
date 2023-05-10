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
                case RepresentativeVM representativeVM:
                    SelectedVM = new RepresentativeVM();
                    break;
                case InsuredLocationVM locationVM:
                    SelectedVM = new InsuredLocationVM();
                    break;
                case CustomerEditVM custEditVM:
                    SelectedVM = new CustomerEditVM();
                    break;
                case CustomerCreateVM custCreateVM:
                    SelectedVM = new CustomerCreateVM();
                    break;
                case UrnCreateVM urnCreateVM:
                    SelectedVM = new UrnCreateVM();
                    break;
                case UrnEditVM urnEditVM:
                    SelectedVM = new UrnEditVM();
                    break;
                case UrnInfoVM urnInfo:
                    SelectedVM = new UrnInfoVM();
                    break;
                case CustomerInfoVM customerInfo:
                    SelectedVM = new CustomerInfoVM();
                    break;
                case ExtensionInfoVM extensionInfo:
                    SelectedVM = new ExtensionInfoVM();
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
            CustomMenuItems.Add(new CustomMenuItem(new BaseVM(), "BẢN CHÀO", PackIconKind.AlphaQBoxOutline));
            CustomMenuItems.Add(new CustomMenuItem(new BaseVM(), "GIẤY CHỨNG NHẬN", PackIconKind.FileDocumentCheck));           

            CustomMenuItems.Add(new CustomMenuItem("HỢP ĐỒNG", PackIconKind.FileSign
                                , new List<CustomSubItem>
                                {
                                    new CustomSubItem(new CustomerInfoVM(), "Khách hàng"),
                                    new CustomSubItem(new UrnInfoVM(), "URN"),
                                    new CustomSubItem(new ExtensionInfoVM(), "Điều khoản bổ sung"),
                                }));

            CustomMenuItems.Add(new CustomMenuItem("KHÁCH HÀNG", PackIconKind.AccountBoxMultipleOutline
                                , new List<CustomSubItem>
                                {
                                    new CustomSubItem(new CustomerCreateVM(), "Tạo mới"),
                                    new CustomSubItem(new CustomerEditVM(), "Sửa"),
                                    new CustomSubItem(new InsuredLocationVM(), "Địa điểm"),
                                    new CustomSubItem(new RepresentativeVM(), "Người đại diện"),
                                }));

            CustomMenuItems.Add(new CustomMenuItem("URN", PackIconKind.QrcodeScan
                                , new List<CustomSubItem>
                                {
                                    new CustomSubItem(new UrnCreateVM(), "Tạo mới"),
                                    new CustomSubItem(new UrnEditVM(), "Sửa"),
                                }));

            CustomMenuItems.Add(new CustomMenuItem("TRA CỨU", PackIconKind.InformationOutline
                                , new List<CustomSubItem>
                                {
                                    new CustomSubItem(new CustomerInfoVM(), "Khách hàng"),
                                    new CustomSubItem(new UrnInfoVM(), "URN"),
                                    new CustomSubItem(new ExtensionInfoVM(), "Điều khoản bổ sung"),
                                }));
        }        

        public MainVM()
        {
            CreateMenu();
        }
    }
}