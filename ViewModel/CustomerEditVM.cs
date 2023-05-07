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
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using System.Xml.Linq;

namespace PnC_Insurance.ViewModel
{
    public partial class CustomerEditVM : BaseVM
    {
        #region Search Customer
        [NotifyPropertyChangedFor(nameof(ListOfCustomers))]
        [ObservableProperty]
        private string? customerSearch;
        public List<Customer>? ListOfCustomers
        {
            get
            {
                if (!String.IsNullOrEmpty(CustomerSearch) && !String.IsNullOrWhiteSpace(CustomerSearch))
                {
                    using (var context = new InsuranceDbContext())
                    {
                        var query = from customer in context.Customers.AsNoTracking()
                                    where customer.IsDeleted == 0 &&
                                          (EF.Functions.Like(customer.TaxCode, "%" + CustomerSearch + "%") ||
                                          EF.Functions.Like(customer.Name, "%" + CustomerSearch + "%") ||
                                          EF.Functions.Like(customer.Business, "%" + CustomerSearch + "%"))
                                    orderby customer.Id
                                    select customer;

                        if (query.Any())
                        {
                            return query.ToList();
                        }
                    }
                }

                return new List<Customer>();
            }

        }

        [ObservableProperty]
        private Customer? selectedCustomer;

        partial void OnSelectedCustomerChanged(Customer? value)
        {
            if (value != null)
            {
                EditingTaxCode = value.TaxCode;
                EditingName = value.Name;
                EditingAddress = value.Address;
                EditingBusiness = value.Business;
                EditingBusinessCode = value.BusinessCode;
                EditingClientCode = value.ClientCode;
                EditingNameEn = value.NameEn;
                EditingAddressEn = value.AddressEn;
                EditingBusinessEn = value.BusinessEn;
            }
            else
            {
                StartOver();
            }
        }

        #endregion

        #region Edit Customer

        #region Required Information
        [ObservableProperty]
        [Required(ErrorMessage = "Nhập mã số thuế")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(EditCustomerCommand))]
        private string? editingTaxCode;

        [ObservableProperty]
        [Required(ErrorMessage = "Nhập tên Khách hàng")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(EditCustomerCommand))]
        private string? editingName;

        [ObservableProperty]
        [Required(ErrorMessage = "Nhập địa chỉ")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(EditCustomerCommand))]
        private string? editingAddress;

        [ObservableProperty]
        [Required(ErrorMessage = "Nhập ngành nghề kinh doanh")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(EditCustomerCommand))]
        private string? editingBusiness;
        #endregion

        #region Not Required Information

        [ObservableProperty]
        private string? editingBusinessCode;

        [ObservableProperty]
        private string? editingClientCode;

        [ObservableProperty]
        private string? editingNameEn;

        [ObservableProperty]
        private string? editingAddressEn;

        [ObservableProperty]
        private string? editingBusinessEn;

        #endregion

        #region Add New Customer
        [RelayCommand(CanExecute = nameof(CanEditCustomerCommand))]
        private async Task EditCustomerAsync()
        {
            ResultNotification = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));

            var editingCustomer = new Customer()
            {
                TaxCode = EditingTaxCode,
                Name = EditingName,
                Address = EditingAddress,
                Business = EditingBusiness,
                BusinessCode = EditingBusinessCode,
                ClientCode = EditingClientCode,
                NameEn = EditingNameEn,
                AddressEn = EditingAddressEn,
                BusinessEn = EditingBusinessEn,
            };

            string notificationString = "";

            try
            {
                notificationString = await Task.Run(async () =>
                {
                    using (var context = new InsuranceDbContext())
                    {
                        if (SelectedCustomer != null && editingCustomer != null)
                        {
                            var query = from customer in context.Customers
                                        where customer.Id == SelectedCustomer.Id
                                        orderby customer.Id
                                        select customer;

                            if (query.Any())
                            {
                                var changeCustomer = await query.FirstOrDefaultAsync();

                                changeCustomer.TaxCode = editingCustomer.TaxCode;
                                changeCustomer.Name = editingCustomer.Name;
                                changeCustomer.Address = editingCustomer.Address;
                                changeCustomer.Business = editingCustomer.Business;
                                changeCustomer.BusinessCode = editingCustomer.BusinessCode;
                                changeCustomer.ClientCode = editingCustomer.ClientCode;
                                changeCustomer.NameEn = editingCustomer.NameEn;
                                changeCustomer.AddressEn = editingCustomer.AddressEn;
                                changeCustomer.BusinessEn = editingCustomer.BusinessEn;

                                await context.SaveChangesAsync();
                            }
                        }
                    }

                    CustomerSearch = null;
                    return "Đã sửa thông tin Khách hàng";
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

        private bool CanEditCustomerCommand()
        {
            if (!GetErrors(nameof(EditingTaxCode)).Any() &&
                !GetErrors(nameof(EditingName)).Any() &&
                !GetErrors(nameof(EditingAddress)).Any() &&
                !GetErrors(nameof(EditingBusiness)).Any())
                return true;

            return false;

        }

        private void StartOver()
        {
            EditingTaxCode = null;
            EditingName = null;
            EditingAddress = null;
            EditingBusiness = null;
            EditingBusinessCode = null;
            EditingClientCode = null;
            EditingNameEn = null;
            EditingAddressEn = null;
            EditingBusinessEn = null;
            ValidateAllProperties();
        }
        #endregion
        #endregion

        public CustomerEditVM()
        {
            StartOver();
        }
    }
}
        