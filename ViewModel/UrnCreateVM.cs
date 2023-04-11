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
        private long newDeptUrn;

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
                Urn = NewDeptUrn,
                Name = NewDeptName,
            };

            await Task.Run(async () =>
            {
                using (var context = new InsuranceDbContext())
                {
                    //var query = from department in context.Departments
                    //            where department.Urn == addingDepartment.Urn
                    //            select department;
                    
                    //if (query.Any())
                    //{
                    //    ResultNotification.Enqueue("Số URN này đã tồn tại");
                    //    return;
                    //}

                    //await context.Departments.AddAsync(addingDepartment);
                    //await context.SaveChangesAsync();

                    //ResultNotification.Enqueue("Đã tạo Phòng mới");
                    StartOver();

                }
            });

            
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
            NewDeptName= null;
            ValidateAllProperties();
        }

        public UrnCreateVM()
        {
            StartOver();
        }
    }
}
