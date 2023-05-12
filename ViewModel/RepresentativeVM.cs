using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MinhHelper;
using PnC_Insurance.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;


namespace PnC_Insurance.ViewModel
{
    public partial class RepresentativeVM : BaseVM
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
        [NotifyPropertyChangedFor(nameof(ListOfRepresentatives))]
        [NotifyPropertyChangedFor(nameof(SelectedRepresentative))]        
        [NotifyCanExecuteChangedFor(nameof(AddRepresentativeCommand))]
        private Customer? selectedCustomer;

        partial void OnSelectedCustomerChanged(Customer? value)
        {
            EditingRepresentativeFullName = null;
            EditingRepresentativePosition = null;
            EditingRepresentativeDecisionNo = null;
            EditingRepresentativePositionEn = null;
            EditingRepresentativeDecisionNoEn = null;            
        }

        public List<Representative>? ListOfRepresentatives
        {
            get
            {
                using (var context = new InsuranceDbContext())
                {
                    if (SelectedCustomer != null)
                    {
                        var query = from representative in context.Representatives.AsNoTracking()
                                    where representative.IsDeleted == 0 &&
                                          representative.CustomerId == SelectedCustomer.Id
                                    orderby representative.Id
                                    select representative;
                        if (query.Any())
                        {
                            return query.ToList();
                        }
                    }

                    return new List<Representative>();
                }
            }

        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OpenDeleteDialogCommand))]
        private Representative? selectedRepresentative;

        partial void OnSelectedRepresentativeChanged(Representative? value)
        {
            if (value != null)
            {
                EditingRepresentativeFullName = SelectedRepresentative.FullName;
                EditingRepresentativePosition = SelectedRepresentative.Position;
                EditingRepresentativeDecisionNo = SelectedRepresentative.DecisionNo;
                EditingRepresentativePositionEn = SelectedRepresentative.PositionEn;
                EditingRepresentativeDecisionNoEn = SelectedRepresentative.DecisionNoEn;
            }
        }
        #endregion

        #region Add Representative
        [ObservableProperty]
        [Required(ErrorMessage = "Nhập Họ tên")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(AddRepresentativeCommand))]
        private string? newRepresentativeFullName;

        [ObservableProperty]
        [Required(ErrorMessage = "Nhập Chức vụ")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(AddRepresentativeCommand))]
        private string? newRepresentativePosition;

        [ObservableProperty]
        private string? newRepresentativeDecisionNo;

        [ObservableProperty]
        private string? newRepresentativePositionEn;

        [ObservableProperty]
        private string? newRepresentativeDecisionNoEn;

        [ObservableProperty]
        private SnackbarMessageQueue? addResultNotification;

        [RelayCommand(CanExecute = nameof(CanAddRepresentativeCommand))]
        private async Task AddRepresentativeAsync()
        {
            AddResultNotification = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));

            var addingRepresentative = new Representative()
            {
                CustomerId = SelectedCustomer.Id,
                FullName = StringHelper.RemoveRedundantWhitespaces(NewRepresentativeFullName),
                Position = StringHelper.RemoveRedundantWhitespaces(NewRepresentativePosition),
                DecisionNo = StringHelper.RemoveRedundantWhitespaces(NewRepresentativeDecisionNo),
                PositionEn = StringHelper.RemoveRedundantWhitespaces(NewRepresentativePositionEn),
                DecisionNoEn = StringHelper.RemoveRedundantWhitespaces(NewRepresentativeDecisionNoEn),
            };

            string notificationString = "";

            try
            {
                notificationString = await Task.Run(async () =>
                {
                    using (var context = new InsuranceDbContext())
                    {
                        await context.Representatives.AddAsync(addingRepresentative);
                        await context.SaveChangesAsync();
                    }

                    return "Đã tạo Người đại diện mới";
                });

                StartOver();
                OnPropertyChanged(nameof(ListOfRepresentatives));
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.InnerException as SqliteException;

                if (sqlException.SqliteErrorCode == 19)
                {
                    notificationString = "Người đại diện này đã có rồi";
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

        private bool CanAddRepresentativeCommand()
        {
            if (!GetErrors(nameof(NewRepresentativeFullName)).Any() &&
                !GetErrors(nameof(NewRepresentativePosition)).Any() &&
                !GetErrors(nameof(SelectedCustomer)).Any())
                return true;

            return false;
        }

        #endregion

        #region Edit Representative
        [ObservableProperty]
        [Required(ErrorMessage = "Nhập Họ tên")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(EditRepresentativeCommand))]
        private string? editingRepresentativeFullName;

        [ObservableProperty]
        [Required(ErrorMessage = "Nhập Chức vụ")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(EditRepresentativeCommand))]
        private string? editingRepresentativePosition;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditRepresentativeCommand))]
        private string? editingRepresentativeDecisionNo;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditRepresentativeCommand))]
        private string? editingRepresentativePositionEn;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditRepresentativeCommand))]
        private string? editingRepresentativeDecisionNoEn;

        [ObservableProperty]
        private SnackbarMessageQueue? editResultNotification;

        [RelayCommand(CanExecute = nameof(CanEditRepresentative))]
        private async Task EditRepresentativeAsync()
        {
            EditResultNotification = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));

            var editingRepresentative = new Representative()
            {
                CustomerId = SelectedCustomer.Id,
                FullName = EditingRepresentativeFullName,
                Position = EditingRepresentativePosition,
                DecisionNo = EditingRepresentativeDecisionNo,
                PositionEn = EditingRepresentativePositionEn,
                DecisionNoEn = EditingRepresentativeDecisionNoEn,
            };

            string notificationString = "";

            try
            {
                notificationString = await Task.Run(async () =>
                {
                    using (var context = new InsuranceDbContext())
                    {
                        if (SelectedRepresentative != null && editingRepresentative != null)
                        {
                            var query = from representative in context.Representatives
                                        where representative.Id == SelectedRepresentative.Id
                                        orderby representative.Id
                                        select representative;

                            if (query.Any())
                            {
                                var changeRepresentative = await query.FirstOrDefaultAsync();

                                changeRepresentative.FullName = StringHelper.RemoveRedundantWhitespaces(editingRepresentative.FullName);
                                changeRepresentative.Position = StringHelper.RemoveRedundantWhitespaces(editingRepresentative.Position);
                                changeRepresentative.DecisionNo = StringHelper.RemoveRedundantWhitespaces(editingRepresentative.DecisionNo);
                                changeRepresentative.PositionEn = StringHelper.RemoveRedundantWhitespaces(editingRepresentative.PositionEn);
                                changeRepresentative.DecisionNoEn = StringHelper.RemoveRedundantWhitespaces(editingRepresentative.DecisionNoEn);

                                await context.SaveChangesAsync();
                            }
                        }
                    }

                    return "Đã sửa thông tin Người đại diện";
                });

                OnPropertyChanged(nameof(ListOfRepresentatives));
                StartOver();
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.InnerException as SqliteException;

                if (sqlException.SqliteErrorCode == 19)
                {
                    notificationString = "Địa điểm này đã tồn tại";
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

            EditResultNotification.Enqueue(notificationString);
        }

        private bool CanEditRepresentative()
        {
            if (SelectedRepresentative != null && SelectedCustomer != null &&
                (SelectedRepresentative.FullName != EditingRepresentativeFullName ||
                SelectedRepresentative.Position != EditingRepresentativePosition ||
                SelectedRepresentative.DecisionNo != EditingRepresentativeDecisionNo ||
                SelectedRepresentative.PositionEn != EditingRepresentativePositionEn ||
                SelectedRepresentative.DecisionNoEn != EditingRepresentativeDecisionNoEn) &&
                (!GetErrors(nameof(EditingRepresentativeFullName)).Any() &&
                !GetErrors(nameof(EditingRepresentativePosition)).Any())
                )
            {
                return true;
            }

            return false;
        }
        #endregion

        #region Delete Representative
        [ObservableProperty]
        private bool isDeletedRepresentativeDialogOpen = false;

        [RelayCommand(CanExecute = nameof(CanOpenDeleteDialog))]
        private void OpenDeleteDialog()
        {
            IsDeletedRepresentativeDialogOpen = true;
        }

        private bool CanOpenDeleteDialog()
        {
            if (SelectedRepresentative != null)
                return true;

            return false;
        }

        [RelayCommand]
        private async Task DeleteRepresentativeAsync()
        {
            EditResultNotification = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
            string notificationString = "";

            try
            {
                notificationString = await Task.Run(async () =>
                {
                    using (var context = new InsuranceDbContext())
                    {
                        if (SelectedRepresentative != null)
                        {
                            var query = from representative in context.Representatives
                                        where representative.Id == SelectedRepresentative.Id
                                        select representative;

                            if (query.Any())
                            {
                                var deleteRepresentative = await query.FirstOrDefaultAsync();
                                deleteRepresentative.IsDeleted = 1;
                                await context.SaveChangesAsync();
                            }
                        }
                    }

                    return "Đã xóa Người đại diện";
                });

                OnPropertyChanged(nameof(ListOfRepresentatives));
                StartOver();
                IsDeletedRepresentativeDialogOpen = false;
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

            EditResultNotification.Enqueue(notificationString);
        }
        #endregion
        private void StartOver()
        {
            NewRepresentativeFullName = null;
            NewRepresentativePosition = null;
            NewRepresentativeDecisionNo = null;
            NewRepresentativePositionEn = null;
            NewRepresentativeDecisionNoEn = null;

            EditingRepresentativeFullName = null;
            EditingRepresentativePosition = null;
            EditingRepresentativeDecisionNo = null;
            EditingRepresentativePositionEn = null;
            EditingRepresentativeDecisionNoEn = null;

            CustomerSearch = null;

            ValidateAllProperties();
        }

        public RepresentativeVM()
        {
            StartOver();
        }

    }
}
