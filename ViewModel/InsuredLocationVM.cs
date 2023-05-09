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

namespace PnC_Insurance.ViewModel
{
    public partial class InsuredLocationVM : BaseVM
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
                                    where EF.Functions.Like(customer.TaxCode, "%" + CustomerSearch + "%") ||
                                          EF.Functions.Like(customer.Name, "%" + CustomerSearch + "%") ||
                                          EF.Functions.Like(customer.Business, "%" + CustomerSearch + "%")
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
                

            }
            else
            {
                StartOver();
            }
        }

        

        #endregion

        #region Add Location
        [ObservableProperty]
        [Required(ErrorMessage = "Nhập Địa chỉ")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(AddLocationCommand))]
        private string? newLocation;

        [ObservableProperty]
        private string? newLocationEn;

        [ObservableProperty]
        private SnackbarMessageQueue? addResultNotification;

        [RelayCommand(CanExecute = nameof(CanAddLocationCommand))]
        private async Task AddLocationAsync()
        {
            AddResultNotification = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));

            var addingLocation = new InsuredLocation()
            {
                Location = NewLocation,
                LocationEn = NewLocationEn,
            };

            string notificationString = "";

            try
            {
                notificationString = await Task.Run(async () =>
                {
                    using (var context = new InsuranceDbContext())
                    {
                        await context.InsuredLocations.AddAsync(addingLocation);
                        await context.SaveChangesAsync();
                    }

                    return "Đã tạo Địa điểm mới";
                });

                StartOver();
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.InnerException as SqliteException;

                if (sqlException.SqliteErrorCode == 19)
                {
                    notificationString = "Địa điểm này đã có rồi";
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

        private bool CanAddLocationCommand()
        {
            if (!GetErrors(nameof(NewLocation)).Any())
                return true;

            return false;

        }

        #endregion



        private void StartOver()
        {

        }
    }
}
