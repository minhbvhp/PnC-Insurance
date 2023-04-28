using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using PnC_Insurance.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PnC_Insurance.ViewModel
{
    public partial class UrnEditVM : BaseVM
    {
        #region Department Search
        [ObservableProperty]
        private bool isFlipped = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SearchDepartmentCommand))]
        private string? departmentSearch;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(FetchDepartmentCommand))]
        [NotifyCanExecuteChangedFor(nameof(EditDepartmentCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteDepartmentCommand))]
        private Department? selectedDepartment;

        private List<Department> listOfDepartments = new List<Department>();
        public List<Department>? ListOfDepartments
        {
            get
            {
                return new List<Department>(listOfDepartments);
            }
        }

        [RelayCommand(CanExecute = nameof(CanSearchDepartment))]
        private async Task SearchDepartmentAsync()
        {
            var result = await Task.Run(() =>
            {
                using (var context = new InsuranceDbContext())
                {
                    var query = from department in context.Departments.AsNoTracking()
                                where EF.Functions.Like(department.Urn, "%" + DepartmentSearch + "%") ||
                                      EF.Functions.Like(department.Name, "%" + DepartmentSearch + "%")
                                select department;
                    return query.ToListAsync();
                }
            });

            listOfDepartments = result;
            OnPropertyChanged(nameof(ListOfDepartments));
        }

        private bool CanSearchDepartment()
        {
            if (!String.IsNullOrEmpty(DepartmentSearch) && !String.IsNullOrWhiteSpace(DepartmentSearch))
                return true;

            return false;
        }

        [RelayCommand(CanExecute = nameof(CanFetchDepartment))]
        private void FetchDepartment()
        {
            EditingDeptUrn = SelectedDepartment.Urn;
            EditingDeptName = SelectedDepartment.Name;
            IsFlipped = true;
        }

        private bool CanFetchDepartment()
        {
            if (SelectedDepartment != null)
                return true;

            return false;
        }

        #endregion


        #region Editing Department
        [ObservableProperty]
        private SnackbarMessageQueue? editDeptResultNotification;

        [ObservableProperty]
        [Required(ErrorMessage = "Nhập số URN")]
        [RegularExpression(@"^[0-9a-zA-Z]+$", ErrorMessage = "Số URN không bao gồm kí tự đặc biệt")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(EditDepartmentCommand))]
        private string? editingDeptUrn;

        [ObservableProperty]
        [Required(ErrorMessage = "Nhập tên Phòng")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(EditDepartmentCommand))]
        private string? editingDeptName;

        [RelayCommand]
        private void DeptFlipBack()
        {            
            IsFlipped = false;
        }

        #region Edit Part

        [RelayCommand(CanExecute = nameof(CanEditDepartment))]
        private async Task EditDepartmentAsync()
        {
            EditDeptResultNotification = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));

            var editingDepartment = new Department()
            {
                Id = SelectedDepartment.Id,
                Urn = EditingDeptUrn,
                Name = EditingDeptName,
            };

            var existDepartment = await Task.Run(() =>
            {
                using (var context = new InsuranceDbContext())
                {
                    var query = from department in context.Departments.AsNoTracking()
                                where department.Urn == editingDepartment.Urn && department.Id != editingDepartment.Id
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
                        if (SelectedDepartment != null && editingDepartment != null)
                        {
                            var query = from department in context.Departments
                                                   where department.Id == SelectedDepartment.Id
                                                   select department;
                            if (query.Any())
                            {
                                var changeDepartment = await query.FirstOrDefaultAsync();

                                changeDepartment.Urn = editingDepartment.Urn;
                                changeDepartment.Name = editingDepartment.Name;

                                await context.SaveChangesAsync();
                            }                            
                        }
                    }

                    return "Đã sửa thông tin Phòng";
                });
                
                await SearchDepartmentAsync();
                IsFlipped = false;
            }

            EditDeptResultNotification.Enqueue(notificationString);
        }

        private bool CanEditDepartment()
        {
            if (SelectedDepartment != null &&
                (SelectedDepartment.Urn != EditingDeptUrn ||
                SelectedDepartment.Name != EditingDeptName) &&
                !GetErrors(nameof(EditingDeptUrn)).Any() &&
                !GetErrors(nameof(EditingDeptName)).Any()
                )
            {
                return true;
            }            

            return false;
        }

        #endregion

        #region Delete Part

        [ObservableProperty]
        private bool isDeletedDialogOpen = false;

        [RelayCommand]
        private async Task DeleteDepartmentAsync()
        {
            EditDeptResultNotification = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
            string notificationString = "Lỗi, không xóa được";

            notificationString = await Task.Run(async () =>
            {
                using (var context = new InsuranceDbContext())
                {
                    if (SelectedDepartment != null)
                    {
                        context.Departments.Remove(SelectedDepartment);
                        await context.SaveChangesAsync();
                    }
                }

                return "Đã xóa Phòng";
            });

            EditDeptResultNotification.Enqueue(notificationString);
            IsDeletedDialogOpen = false;
            await Task.Delay(1000);
            IsFlipped = false;
        }

        #endregion

        #endregion
    }
}