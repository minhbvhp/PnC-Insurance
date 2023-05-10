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
    public partial class InsuredLocationVM : BaseVM
    {
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
                OnPropertyChanged(nameof(ListOfLocations));
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

            AddResultNotification.Enqueue(notificationString);
        }

        private bool CanAddLocationCommand()
        {
            if (!GetErrors(nameof(NewLocation)).Any())
                return true;

            return false;
        }

        #endregion

        #region Search Location
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
        [NotifyCanExecuteChangedFor(nameof(OpenDeleteDialogCommand))]
        private InsuredLocation? selectedLocation;

        partial void OnSelectedLocationChanged(InsuredLocation? value)
        {
            if (value != null)
            {
                EditingLocation = value.Location;
                EditingLocationEn = value.LocationEn;
            }
            else
            {
                StartOver();
            }
        }

        #endregion

        #region Edit Location
        [ObservableProperty]
        [Required(ErrorMessage = "Nhập Địa chỉ")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(EditLocationCommand))]
        private string? editingLocation;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditLocationCommand))]
        private string? editingLocationEn;

        [ObservableProperty]
        private SnackbarMessageQueue? editResultNotification;

        [RelayCommand(CanExecute = nameof(CanEditLocation))]
        private async Task EditLocationAsync()
        {
            EditResultNotification = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));

            var editingInsuredLocation = new InsuredLocation()
            {
                Id = SelectedLocation.Id,
                Location = EditingLocation,
                LocationEn = EditingLocationEn,
            };

            string notificationString = "";

            try
            {
                notificationString = await Task.Run(async () =>
                {
                    using (var context = new InsuranceDbContext())
                    {
                        if (SelectedLocation != null && editingInsuredLocation != null)
                        {
                            var query = from location in context.InsuredLocations
                                        where location.Id == SelectedLocation.Id
                                        orderby location.Id
                                        select location;

                            if (query.Any())
                            {
                                var changeLocation = await query.FirstOrDefaultAsync();

                                changeLocation.Location = editingInsuredLocation.Location;
                                changeLocation.LocationEn = editingInsuredLocation.LocationEn;
                                

                                await context.SaveChangesAsync();
                            }
                        }
                    }

                    return "Đã sửa thông tin Địa điểm";
                });

                OnPropertyChanged(nameof(ListOfLocations));
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

        private bool CanEditLocation()
        {
            if (SelectedLocation != null &&
                (SelectedLocation.Location != EditingLocation ||
                SelectedLocation.LocationEn != EditingLocationEn) &&
                !GetErrors(nameof(EditingLocation)).Any()
                )
            {
                return true;
            }

            return false;
        }
        #endregion

        #region Delete Location
        [ObservableProperty]
        private bool isDeletedLocationDialogOpen = false;

        [RelayCommand(CanExecute = nameof(CanOpenDeleteDialog))]
        private void OpenDeleteDialog()
        {
            IsDeletedLocationDialogOpen = true;
        }

        private bool CanOpenDeleteDialog()
        {
            if (SelectedLocation != null)
                return true;

            return false;
        }

        [RelayCommand]
        private async Task DeleteLocationAsync()
        {
            EditResultNotification = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
            string notificationString = "";

            try
            {
                notificationString = await Task.Run(async () =>
                {
                    using (var context = new InsuranceDbContext())
                    {
                        if (SelectedLocation != null)
                        {
                            var query = from location in context.InsuredLocations
                                        where location.Id == SelectedLocation.Id
                                        select location;

                            if (query.Any())
                            {
                                var deleteLocation = await query.FirstOrDefaultAsync();
                                deleteLocation.IsDeleted = 1;
                                await context.SaveChangesAsync();
                            }
                        }
                    }

                    return "Đã xóa Địa điểm";
                });

                OnPropertyChanged(nameof(ListOfLocations));
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
        }
        #endregion

        private void StartOver()
        {
            NewLocation = null;
            NewLocationEn = null;
            EditingLocation = null;
            EditingLocationEn = null;
            LocationSearch = null;
            ValidateAllProperties();
        }

        public InsuredLocationVM()
        {
            StartOver();
        }
    }
}
