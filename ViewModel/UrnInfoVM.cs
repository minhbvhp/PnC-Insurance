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
                    return context.Departments.ToList();
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
                        var query = from employee in context.Employees
                                    where employee.DeptUrn == SelectedDepartment.Urn
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
                        var query = from agent in context.Agents
                                    where agent.DeptUrn == SelectedDepartment.Urn
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

        public UrnInfoVM()
        {

        }
    }
}
