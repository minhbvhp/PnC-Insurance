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
using System.Threading.Tasks;
using System.Windows;

namespace PnC_Insurance.ViewModel
{
    public partial class CustomerLocationVM : BaseVM
    {
        #region Customer Search
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
        [Required(ErrorMessage = "Chọn khách hàng")]
        [NotifyDataErrorInfo]
        [NotifyPropertyChangedFor(nameof(ListOfMatchLocations))]
        [NotifyPropertyChangedFor(nameof(SelectedMatchLocation))]
        [NotifyCanExecuteChangedFor(nameof(AddLocationCommand))]
        private Customer? selectedCustomer;

        partial void OnSelectedCustomerChanged(Customer? value)
        {
            SelectedMatchLocation = null;
        }

        public List<InsuredLocation>? ListOfMatchLocations
        {
            get
            {
                if (SelectedCustomer != null)
                {
                    using (var context = new InsuranceDbContext())
                    {
                        var query = from location in context.InsuredLocations.AsNoTracking()
                                    from customer_location_combine in context.CustomersInsuredLocations.AsNoTracking()
                                    where customer_location_combine.CustomerId == SelectedCustomer.Id &&
                                          customer_location_combine.InsuredLocationId == location.Id
                                    select location;

                        if (query.Any())
                        {
                            return query.ToList();
                        }
                    }
                }

                return new List<InsuredLocation>();
            }

        }

        [ObservableProperty]
        [Required(ErrorMessage = "Chọn địa điểm")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(OpenDeleteDialogCommand))]
        private InsuredLocation? selectedMatchLocation;
        #endregion

        #region Location Search
        [NotifyPropertyChangedFor(nameof(ListOfLocations))]
        [ObservableProperty]
        private string? locationSearch;
        public List<InsuredLocation>? ListOfLocations
        {
            get
            {
                if (!String.IsNullOrEmpty(LocationSearch) && !String.IsNullOrWhiteSpace(LocationSearch))
                {
                    using (var context = new InsuranceDbContext())
                    {
                        var query = from insuredLocation in context.InsuredLocations.AsNoTracking()
                                    where insuredLocation.IsDeleted == 0 && EF.Functions.Like(insuredLocation.Location, "%" + LocationSearch + "%")
                                    orderby insuredLocation.Id
                                    select insuredLocation;

                        if (query.Any())
                        {
                            return query.ToList();
                        }
                    }
                }

                return new List<InsuredLocation>();
            }
        }

        [ObservableProperty]
        [Required(ErrorMessage = "Chọn địa điểm")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(AddLocationCommand))]
        private InsuredLocation? selectedLocation;        

        #endregion

        #region Add Location
        [ObservableProperty]
        private SnackbarMessageQueue? addResultNotification;

        [RelayCommand(CanExecute = nameof(CanAddLocationCommand))]
        private async Task AddLocationAsync()
        {
            AddResultNotification = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
            
            string notificationString = "";

            var addingCustomerLocationCombine = new CustomersInsuredLocation()
            {
                CustomerId = SelectedCustomer.Id,
                InsuredLocationId = SelectedLocation.Id,
            };

            try
            {
                notificationString = await Task.Run(async () =>
                {
                    using (var context = new InsuranceDbContext())
                    {
                        await context.CustomersInsuredLocations.AddAsync(addingCustomerLocationCombine);
                        await context.SaveChangesAsync();
                    }

                    return "Đã thêm Địa điểm được bảo hiểm";
                });

                OnPropertyChanged(nameof(ListOfMatchLocations));
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.InnerException as SqliteException;

                if (sqlException.SqliteErrorCode == 19)
                {
                    notificationString = "Địa điểm này đã thuộc khách hàng này rồi";
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

            AddResultNotification.Enqueue(notificationString);
        }

        private bool CanAddLocationCommand()
        {
            if (!GetErrors(nameof(SelectedLocation)).Any() &&
                !GetErrors(nameof(SelectedCustomer)).Any())
                return true;

            return false;
        }
        #endregion

        #region Delete Location
        [ObservableProperty]
        private SnackbarMessageQueue? deleteResultNotification;

        [ObservableProperty]
        private bool isDeletedLocationDialogOpen = false;

        [RelayCommand(CanExecute = nameof(CanOpenDeleteDialog))]
        private void OpenDeleteDialog()
        {
            IsDeletedLocationDialogOpen = true;
        }

        private bool CanOpenDeleteDialog()
        {
            if (SelectedMatchLocation != null)
                return true;

            return false;
        }

        [RelayCommand]
        private async Task DeleteLocationAsync()
        {
            DeleteResultNotification = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
            string notificationString = "";

            try
            {
                notificationString = await Task.Run(async () =>
                {
                    using (var context = new InsuranceDbContext())
                    {
                        if (SelectedMatchLocation != null)
                        {
                            var query = from location in context.InsuredLocations
                                        from customer_location_combine in context.CustomersInsuredLocations
                                        where customer_location_combine.CustomerId == SelectedCustomer.Id &&
                                              customer_location_combine.InsuredLocationId == SelectedMatchLocation.Id
                                        select customer_location_combine;

                            if (query.Any())
                            {
                                var deleteCustomerLocationCombine = await query.FirstOrDefaultAsync();
                                context.CustomersInsuredLocations.Remove(deleteCustomerLocationCombine);
                                await context.SaveChangesAsync();
                            }
                        }
                    }

                    return "Đã xóa Địa điểm được bảo hiểm của Khách hàng này";
                });
                
                StartOver();
                IsDeletedLocationDialogOpen = false;
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.InnerException as SqliteException;

                notificationString = "Lỗi CSDL: " + sqlException.SqliteErrorCode;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                notificationString = "Lỗi: " + ex.HResult.ToString();
            }

            DeleteResultNotification.Enqueue(notificationString);
        }
        #endregion

        private void StartOver()
        {
            CustomerSearch = null;
            LocationSearch = null;
            ValidateAllProperties();
        }

        public CustomerLocationVM()
        {
            StartOver();
        }
    }
}
