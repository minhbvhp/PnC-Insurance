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
                                    where EF.Functions.Like(insuredLocation.Location, "%" + LocationSearch + "%")
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
        private InsuredLocation? selectedLocation;

        partial void OnLocationSearchChanged(string? value)
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

        private void StartOver()
        {

        }
    }
}
