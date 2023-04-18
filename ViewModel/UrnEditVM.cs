using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using PnC_Insurance.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnC_Insurance.ViewModel
{
    public partial class UrnEditVM : BaseVM
    {
        #region Department Search
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SearchDepartmentCommand))]
        private string? departmentSearch;

        [ObservableProperty]
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
                    var query = from department in context.Departments
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
        #endregion



        #region Editing Department
        [ObservableProperty]
        private string? editingDeptUrn;

        [ObservableProperty]
        private string? editingDeptName;

        #endregion
    }
}