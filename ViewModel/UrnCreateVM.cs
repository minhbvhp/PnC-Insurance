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
using System.Configuration;
using Microsoft.Data.Sqlite;
using System.Windows;

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
                    var query = from department in context.Departments
                                where department.IsDeleted == 0
                                orderby department.Id
                                select department;
                    return query.ToList();
                }
            }
        }

        #region New Department

        [ObservableProperty]
        [Required(ErrorMessage = "Nhập số URN")]
        [RegularExpression(@"^[0-9a-zA-Z]+$", ErrorMessage = "Số URN không bao gồm kí tự đặc biệt")]
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

            string notificationString = "";

            try
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
            catch (DbUpdateException ex)
            {
                var sqlException = ex.InnerException as SqliteException;

                if (sqlException.SqliteErrorCode == 19)
                {
                    notificationString = "Số URN này đã tồn tại";
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

            DeptResultNotification.Enqueue(notificationString);            
        }

        private bool CanAddNewDepartmentCommand()
        {
            if (!GetErrors(nameof(NewDeptUrn)).Any() && !GetErrors(nameof(NewDeptName)).Any())
                return true;
            return false;
            
        }
        #endregion

        #region New Employee
        [ObservableProperty]
        [Required(ErrorMessage = "Nhập số URN")]
        [RegularExpression(@"^[0-9a-zA-Z]+$", ErrorMessage = "Số URN không bao gồm kí tự đặc biệt")]
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

            string notificationString = "";

            try
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
            catch (DbUpdateException ex)
            {
                var sqlException = ex.InnerException as SqliteException;

                if (sqlException.SqliteErrorCode == 19)
                {
                    notificationString = "Số URN này đã tồn tại";
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

            EmployeeResultNotification.Enqueue(notificationString);
        }

        private bool CanAddNewEmployeeCommand()
        {
            if (!GetErrors(nameof(NewEmployeeUrn)).Any() && !GetErrors(nameof(NewEmployeeName)).Any() && !GetErrors(nameof(DepartmentOfEmployee)).Any())
                return true;
            return false;
        }

        #endregion

        #region New Agent
        [ObservableProperty]
        [Required(ErrorMessage = "Nhập số URN")]
        [RegularExpression(@"^[0-9a-zA-Z]+$", ErrorMessage = "Số URN không bao gồm kí tự đặc biệt")]
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

            string notificationString = "";

            try
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
            catch (DbUpdateException ex)
            {
                var sqlException = ex.InnerException as SqliteException;

                if (sqlException.SqliteErrorCode == 19)
                {
                    notificationString = "Số URN này đã tồn tại";
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

            AgentResultNotification.Enqueue(notificationString);
        }

        private bool CanAddNewAgentCommand()
        {
            if (!GetErrors(nameof(NewAgentUrn)).Any() && !GetErrors(nameof(NewAgentName)).Any() && !GetErrors(nameof(DepartmentOfAgent)).Any())
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
