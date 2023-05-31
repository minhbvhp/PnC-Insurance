using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PnC_Insurance.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MinhHelper;

namespace PnC_Insurance.ViewModel
{
    public partial class CustomerCreateVM : BaseVM
    {
        #region Required Information

        [ObservableProperty]
        [Required(ErrorMessage = "Nhập mã số thuế")]        
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(AddNewCustomerCommand))]
        private string? newTaxCode;

        [ObservableProperty]
        [Required(ErrorMessage = "Nhập tên Khách hàng")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(AddNewCustomerCommand))]
        private string? newName;

        [ObservableProperty]
        [Required(ErrorMessage = "Nhập địa chỉ")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(AddNewCustomerCommand))]
        private string? newAddress;

        [ObservableProperty]
        [Required(ErrorMessage = "Nhập ngành nghề kinh doanh")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(AddNewCustomerCommand))]
        private string? newBusiness;

        #endregion

        #region Not Required Information

        [ObservableProperty]
        private string? newBusinessCode;

        [ObservableProperty]
        private string? newClientCode;

        [ObservableProperty]
        private string? newNameEn;

        [ObservableProperty]
        private string? newAddressEn;

        [ObservableProperty]
        private string? newBusinessEn;

        #endregion

        #region Add New Customer
        [RelayCommand(CanExecute = nameof(CanAddNewCustomer))]
        private async Task AddNewCustomerAsync()
        {
            ResultNotification = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));

            var addingCustomer = new Customer()
            {
                TaxCode = StringHelper.RemoveRedundantWhitespaces(NewTaxCode),
                Name = StringHelper.RemoveRedundantWhitespaces(NewName).ToUpper(),
                Address = StringHelper.RemoveRedundantWhitespaces(NewAddress),
                Business = StringHelper.RemoveRedundantWhitespaces(NewBusiness),
                BusinessCode = StringHelper.RemoveRedundantWhitespaces(NewBusinessCode),
                ClientCode= StringHelper.RemoveRedundantWhitespaces(NewClientCode),
                NameEn= StringHelper.RemoveRedundantWhitespaces(NewNameEn).ToUpper(),
                AddressEn= StringHelper.RemoveRedundantWhitespaces(NewAddressEn),
                BusinessEn= StringHelper.RemoveRedundantWhitespaces(NewBusinessEn),
            };

            string notificationString = "";

            try
            {
                notificationString = await Task.Run(async () =>
                {
                    using (var context = new InsuranceDbContext())
                    {
                        await context.Customers.AddAsync(addingCustomer);
                        await context.SaveChangesAsync();
                    }

                    return "Đã tạo Khách hàng mới";
                });

                StartOver();               
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.InnerException as SqliteException;

                if (sqlException.SqliteErrorCode == 19)
                {
                    notificationString = "Khách hàng này đã có rồi";
                }
                else
                {
                    notificationString = "Lỗi CSDL: " + sqlException.SqliteErrorCode;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                notificationString = "Lỗi: " + ex.HResult.ToString();
            }

            ResultNotification.Enqueue(notificationString);
        }
        
        private bool CanAddNewCustomer()
        {
            if (!GetErrors(nameof(NewTaxCode)).Any() && 
                !GetErrors(nameof(NewName)).Any() &&
                !GetErrors(nameof(NewAddress)).Any() &&
                !GetErrors(nameof(NewBusiness)).Any())
                return true;

            return false;

        }

        private void StartOver()
        {
            NewTaxCode = null;
            NewName = null;
            NewAddress = null;
            NewBusiness = null;
            NewBusinessCode = null;
            NewClientCode = null;
            NewNameEn = null;
            NewAddressEn = null;
            NewBusinessEn = null;
            ValidateAllProperties();
        }
        #endregion

        public CustomerCreateVM()
        {
            StartOver();
        }
    }
}
