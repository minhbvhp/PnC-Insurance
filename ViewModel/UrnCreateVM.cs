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
using MinhHelper;

namespace PnC_Insurance.ViewModel
{
    public partial class UrnCreateVM : BaseVM
    {
        [ObservableProperty]
        private SnackbarMessageQueue? deptResultNotification;

        [ObservableProperty]
        private SnackbarMessageQueue? employeeResultNotification;

        [ObservableProperty]
        private SnackbarMessageQueue? agentResultNotification;

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
        private string? newDeptUrn;

        [ObservableProperty]
        [Required(ErrorMessage = "Nhập tên Phòng")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(AddNewDepartmentCommand))]
        private string? newDeptName;

        [RelayCommand(CanExecute = nameof(CanAddNewDepartmentCommand))]
        private async Task AddNewDepartmentAsync()
        {
            DeptResultNotification = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
            
            var addingDepartment = new Department()
            {
                Urn = NewDeptUrn,
                Name = NewDeptName,
            };

            var existDepartment = await Task.Run(() =>
            {
                using (var context = new InsuranceDbContext())
                {
                    var query = from department in context.Departments.AsNoTracking()
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
                
                StartDeptOver();
                OnPropertyChanged(nameof(ListOfDepartments));
            }

            DeptResultNotification.Enqueue(notificationString);
        }

        private bool CanAddNewDepartmentCommand()
        {
            if (NewDeptUrn != null && NewDeptName != null)
                return true;
            
            return false;
        }
        #endregion

        #region New Employee
        [ObservableProperty]
        [Required(ErrorMessage = "Nhập số URN")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(AddNewEmployeeCommand))]
        private string? newEmployeeUrn;

        [ObservableProperty]
        [Required(ErrorMessage = "Nhập tên Nhân viên")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(AddNewEmployeeCommand))]
        private string? newEmployeeName;

        [ObservableProperty]
        [Required(ErrorMessage = "Chọn Phòng")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(AddNewEmployeeCommand))]
        private Department? departmentOfEmployee;

        [RelayCommand(CanExecute = nameof(CanAddNewEmployeeCommand))]
        private async Task AddNewEmployeeAsync()
        {
            EmployeeResultNotification = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));

            var addingEmployee = new Employee()
            {
                Urn = NewEmployeeUrn,
                FullName = NewEmployeeName,
                DeptId = DepartmentOfEmployee.Id,
            };

            var existEmployee = await Task.Run(() =>
            {
                using (var context = new InsuranceDbContext())
                {
                    var query = from employee in context.Employees.AsNoTracking()
                                where employee.Urn == addingEmployee.Urn
                                select employee;

                    return query.ToList();

                }
            }
            );

            string notificationString = "";

            if (existEmployee.Any())
            {
                notificationString = "Số URN này đã tồn tại";
            }
            else
            {
                notificationString = await Task.Run(async () =>
                {
                    using (var context = new InsuranceDbContext())
                    {
                        await context.Employees.AddAsync(addingEmployee);
                        await context.SaveChangesAsync();
                    }

                    return "Đã tạo Nhân viên mới";
                });

                StartEmployeeOver();
            }

            EmployeeResultNotification.Enqueue(notificationString);
        }

        private bool CanAddNewEmployeeCommand()
        {
            if (NewEmployeeUrn != null && NewEmployeeName != null && DepartmentOfEmployee != null)
                return true;

            return false;
        }

        #endregion

        #region New Agent
        [ObservableProperty]
        [Required(ErrorMessage = "Nhập số URN")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(AddNewAgentCommand))]
        private string? newAgentUrn;

        [ObservableProperty]
        [Required(ErrorMessage = "Nhập tên Đại lý")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(AddNewAgentCommand))]
        private string? newAgentName;

        [ObservableProperty]
        [Required(ErrorMessage = "Chọn Phòng")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(AddNewAgentCommand))]
        private Department? departmentOfAgent;

        [RelayCommand(CanExecute = nameof(CanAddNewAgentCommand))]
        private async Task AddNewAgentAsync()
        {
            AgentResultNotification = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));

            var addingAgent = new Agent()
            {
                Urn = NewAgentUrn,
                FullName = NewAgentName,
                DeptId = DepartmentOfAgent.Id,
            };

            var existAgent = await Task.Run(() =>
            {
                using (var context = new InsuranceDbContext())
                {
                    var query = from agent in context.Agents.AsNoTracking()
                                where agent.Urn == addingAgent.Urn
                                select agent;

                    return query.ToList();

                }
            }
            );

            string notificationString = "";

            if (existAgent.Any())
            {
                notificationString = "Số URN này đã tồn tại";
            }
            else
            {
                notificationString = await Task.Run(async () =>
                {
                    using (var context = new InsuranceDbContext())
                    {
                        await context.Agents.AddAsync(addingAgent);
                        await context.SaveChangesAsync();
                    }

                    return "Đã tạo Đại lý mới";
                });

                StartAgentOver();
            }

            AgentResultNotification.Enqueue(notificationString);
        }

        private bool CanAddNewAgentCommand()
        {
            if (NewAgentUrn != null && NewAgentName != null && DepartmentOfAgent != null)
                return true;

            return false;
        }

        #endregion

        private void StartDeptOver()
        {
            NewDeptUrn = null;
            NewDeptName = null;
            ValidateAllProperties();
        }

        private void StartAgentOver()
        {
            NewAgentUrn = null;
            NewAgentName = null;
            DepartmentOfAgent = null;
            ValidateAllProperties();
        }

        private void StartEmployeeOver()
        {
            NewEmployeeUrn = null;
            NewEmployeeName = null;
            DepartmentOfEmployee = null;
            ValidateAllProperties();
        }

        public UrnCreateVM()
        {
            StartDeptOver();
            StartEmployeeOver();
            StartAgentOver();
        }
    }
}
