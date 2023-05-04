using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using PnC_Insurance.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PnC_Insurance.ViewModel
{
    public partial class UrnInfoVM : BaseVM
    {        
        public List<Department>? ListOfDepartments
        {
            get
            {
                using (var context = new InsuranceDbContext())
                {
                    var query = from department in context.Departments.AsNoTracking()
                                where department.IsDeleted == 0
                                orderby department.Id
                                select department;

                    return query.ToList();
                }
            }

        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ListOfEmployees))]
        [NotifyPropertyChangedFor(nameof(ListOfAgents))]
        private Department? selectedDepartment;

        public List<Employee>? ListOfEmployees
        {
            get
            {
                using (var context = new InsuranceDbContext())
                {
                    if (SelectedDepartment != null)
                    {
                        var query = from employee in context.Employees.AsNoTracking()
                                    where employee.IsDeleted == 0 &&
                                          employee.DeptId == SelectedDepartment.Id                                    
                                    orderby employee.Id
                                    select employee;
                        if (query.Any())
                        {
                            return query.ToList();
                        }
                    }

                    return new List<Employee>();
                }
            }
        }

        public List<Agent>? ListOfAgents
        {
            get
            {
                using (var context = new InsuranceDbContext())
                {
                    if (SelectedDepartment != null)
                    {
                        var query = from agent in context.Agents.AsNoTracking()
                                    where agent.IsDeleted == 0 &&
                                          agent.DeptId == SelectedDepartment.Id
                                    orderby agent.Id
                                    select agent;
                        if (query.Any())
                        {
                            return query.ToList();
                        }
                    }

                    return new List<Agent>();
                }
            }
        }
    }
}
