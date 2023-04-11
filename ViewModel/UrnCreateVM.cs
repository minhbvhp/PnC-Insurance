using CommunityToolkit.Mvvm.ComponentModel;
using PnC_Insurance.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;

namespace PnC_Insurance.ViewModel
{
    public partial class UrnCreateVM : BaseVM
    {
        [ObservableProperty]
        private SnackbarMessageQueue? resultNotification;

        public List<Department>? ListOfDepartments
        {
            get
            {
                using (var context = new InsuranceDbContext())
                {
                    return context.Departments.ToList();
                }
            }
        }

        #region New Department

        [ObservableProperty]
        [Required(ErrorMessage = "Nhập số URN")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(AddNewDepartmentCommand))]
        private long? newDeptUrn;

        [ObservableProperty]
        [Required(ErrorMessage = "Nhập tên phòng")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(AddNewDepartmentCommand))]
        private string? newDeptName;

        [RelayCommand(CanExecute = nameof(CanAddNewDepartmentCommand))]
        private async Task AddNewDepartmentAsync()
        {
            ResultNotification = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
            
            Department addingDepartment = new Department()
            {
                Urn = (long)NewDeptUrn,
                Name = NewDeptName,
            };

            var existDepartment = await Task.Run(() =>
            {
                using (var context = new InsuranceDbContext())
                {
                    var query = from department in context.Departments
                                where department.Urn == addingDepartment.Urn
                                select department;

                    return query.ToList();

                }
            }
            );

            string notificationString = "";

            if (existDepartment.Any())
            {
                notificationString = "Số URN này đã tồn tại";
            }
            else
            {
                notificationString = await Task.Run(async () =>
                {
                    using (var context = new InsuranceDbContext())
                    {
                        await context.Departments.AddAsync(addingDepartment);
                        await context.SaveChangesAsync();
                    }

                    return "Đã tạo Phòng mới";
                });
                
                StartOver();
                OnPropertyChanged(nameof(ListOfDepartments));
            }

            ResultNotification.Enqueue(notificationString);
        }

        private bool CanAddNewDepartmentCommand()
        {
            if (NewDeptUrn != null && NewDeptName != null)
                return true;
            
            return false;
        }

        #endregion

        private void StartOver()
        {
            NewDeptUrn = null;
            NewDeptName= null;
            ValidateAllProperties();
        }

        public UrnCreateVM()
        {
            StartOver();
        }
    }
}
