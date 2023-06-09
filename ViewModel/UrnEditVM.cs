﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MinhHelper;
using PnC_Insurance.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PnC_Insurance.ViewModel
{
    public partial class UrnEditVM : BaseVM
    {
        #region Department

        #region Department Search
        [ObservableProperty]
        private bool isDepartmentFlipped = false;

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
                                where department.IsDeleted == 0 &&
                                      (EF.Functions.Like(department.Urn, "%" + DepartmentSearch + "%") ||
                                      EF.Functions.Like(department.Name, "%" + DepartmentSearch + "%"))
                                orderby department.Id
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
            EditingDepartmentUrn = SelectedDepartment.Urn;
            EditingDepartmentName = SelectedDepartment.Name;
            IsDepartmentFlipped = true;
        }

        private bool CanFetchDepartment()
        {
            if (SelectedDepartment != null)
                return true;

            return false;
        }

        #endregion

        #region Department Modify
        [ObservableProperty]
        private SnackbarMessageQueue? editDepartmentResultNotification;

        [ObservableProperty]
        [Required(ErrorMessage = "Nhập số URN")]
        [RegularExpression(@"^[0-9a-zA-Z]+$", ErrorMessage = "Số URN không bao gồm kí tự đặc biệt")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(EditDepartmentCommand))]
        private string? editingDepartmentUrn;

        [ObservableProperty]
        [Required(ErrorMessage = "Nhập tên Phòng")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(EditDepartmentCommand))]
        private string? editingDepartmentName;

        [RelayCommand]
        private void DepartmentFlipBack()
        {            
            IsDepartmentFlipped = false;
        }

        #region Edit Part

        [RelayCommand(CanExecute = nameof(CanEditDepartment))]
        private async Task EditDepartmentAsync()
        {
            EditDepartmentResultNotification = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));

            var editingDepartment = new Department()
            {
                Id = SelectedDepartment.Id,
                Urn = EditingDepartmentUrn,
                Name = EditingDepartmentName,
            };           

            string notificationString = "";

            try
            {
                notificationString = await Task.Run(async () =>
                {
                    using (var context = new InsuranceDbContext())
                    {
                        if (SelectedDepartment != null && editingDepartment != null)
                        {
                            var query = from department in context.Departments
                                        where department.Id == SelectedDepartment.Id
                                        orderby department.Id
                                        select department;

                            if (query.Any())
                            {
                                var changeDepartment = await query.FirstOrDefaultAsync();

                                changeDepartment.Urn = StringHelper.RemoveRedundantWhitespaces(editingDepartment.Urn);
                                changeDepartment.Name = StringHelper.RemoveRedundantWhitespaces(editingDepartment.Name);

                                await context.SaveChangesAsync();
                            }
                        }
                    }

                    return "Đã sửa thông tin Phòng";
                });

                await SearchDepartmentAsync();
                IsDepartmentFlipped = false;
                await SearchEmployeeAsync();
                await SearchAgentAsync();
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

            EditDepartmentResultNotification.Enqueue(notificationString);
        }

        private bool CanEditDepartment()
        {
            if (SelectedDepartment != null &&
                (SelectedDepartment.Urn != EditingDepartmentUrn ||
                SelectedDepartment.Name != EditingDepartmentName) &&
                !GetErrors(nameof(EditingDepartmentUrn)).Any() &&
                !GetErrors(nameof(EditingDepartmentName)).Any()
                )
            {
                return true;
            }            

            return false;
        }

        #endregion

        #region Delete Part

        [ObservableProperty]
        private bool isDeletedDepartmentDialogOpen = false;

        [RelayCommand]
        private async Task DeleteDepartmentAsync()
        {
            EditDepartmentResultNotification = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
            string notificationString = "";

            try
            {
                notificationString = await Task.Run(async () =>
                {
                    using (var context = new InsuranceDbContext())
                    {
                        if (SelectedDepartment != null)
                        {
                            var query = from department in context.Departments
                                        where department.Id == SelectedDepartment.Id
                                        select department;

                            if (query.Any())
                            {
                                var deleteDepartment = await query.FirstOrDefaultAsync();
                                deleteDepartment.IsDeleted = 1;
                                await context.SaveChangesAsync();
                            }
                        }
                    }

                    return "Đã xóa Phòng";
                });
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
            
            await SearchDepartmentAsync();            
            EditDepartmentResultNotification.Enqueue(notificationString);
            IsDeletedDepartmentDialogOpen = false;
            await Task.Delay(1000);
            IsDepartmentFlipped = false;

            await SearchEmployeeAsync();
            await SearchAgentAsync();
        }

        #endregion

        #endregion

        #endregion

        #region Employee

        #region Employee Search        
        [ObservableProperty]
        private bool isEmployeeFlipped = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SearchEmployeeCommand))]
        private string? employeeSearch;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(FetchEmployeeCommand))]
        [NotifyCanExecuteChangedFor(nameof(EditEmployeeCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteEmployeeCommand))]
        private Employee? selectedEmployee;

        private List<Employee> listOfEmployees = new List<Employee>();
        public List<Employee>? ListOfEmployees
        {
            get
            {
                return new List<Employee>(listOfEmployees);
            }
        }

        [RelayCommand(CanExecute = nameof(CanSearchEmployee))]
        private async Task SearchEmployeeAsync()
        {
            if (!String.IsNullOrEmpty(EmployeeSearch) && !String.IsNullOrWhiteSpace(EmployeeSearch))
            {
                var result = await Task.Run(() =>
                {
                    using (var context = new InsuranceDbContext())
                    {
                        var query = from employee in context.Employees.AsNoTracking()
                                    where employee.IsDeleted == 0 &&
                                          (EF.Functions.Like(employee.Urn, "%" + EmployeeSearch + "%") ||
                                          EF.Functions.Like(employee.FullName, "%" + EmployeeSearch + "%") ||
                                          EF.Functions.Like(employee.Dept.Name, "%" + EmployeeSearch + "%"))
                                    orderby employee.Id
                                    select employee;

                        return query.Include("Dept").ToListAsync();
                    }
                });

                listOfEmployees = result;
                OnPropertyChanged(nameof(ListOfEmployees));            
            }
        }

        private bool CanSearchEmployee()
        {
            if (!String.IsNullOrEmpty(EmployeeSearch) && !String.IsNullOrWhiteSpace(EmployeeSearch))
                return true;

            return false;
        }

        [RelayCommand(CanExecute = nameof(CanFetchEmployee))]
        private void FetchEmployee()
        {
            EditingEmployeeUrn = SelectedEmployee.Urn;
            EditingEmployeeName = SelectedEmployee.FullName;

            OnPropertyChanged(nameof(ListOfDepartmentsOfEmployee));

            EditingDepartmentOfEmployee = SelectedEmployee.Dept;
            IsEmployeeFlipped = true;
        }

        private bool CanFetchEmployee()
        {
            if (SelectedEmployee != null)
                return true;

            return false;
        }

        #endregion

        #region Employee Modify
        public List<Department>? ListOfDepartmentsOfEmployee
        {
            get
            {
                using (var context = new InsuranceDbContext())
                {
                    var query = from department in context.Departments
                                where department.IsDeleted == 0
                                orderby department.Id
                                select department;

                    var result = query.ToList();

                    if (SelectedEmployee != null)
                        result.Add(SelectedEmployee.Dept);

                    return result;

                }
            }
        }

        [ObservableProperty]
        [Required(ErrorMessage = "Chọn Phòng")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(EditEmployeeCommand))]
        private Department? editingDepartmentOfEmployee;

        [ObservableProperty]
        private SnackbarMessageQueue? editEmployeeResultNotification;

        [ObservableProperty]
        [Required(ErrorMessage = "Nhập số URN")]
        [RegularExpression(@"^[0-9a-zA-Z]+$", ErrorMessage = "Số URN không bao gồm kí tự đặc biệt")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(EditEmployeeCommand))]
        private string? editingEmployeeUrn;

        [ObservableProperty]
        [Required(ErrorMessage = "Nhập Họ và tên Nhân viên")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(EditEmployeeCommand))]
        private string? editingEmployeeName;

        [RelayCommand]
        private void EmployeeFlipBack()
        {
            IsEmployeeFlipped = false;
        }

        #region Edit Part

        [RelayCommand(CanExecute = nameof(CanEditEmployee))]
        private async Task EditEmployeeAsync()
        {
            EditEmployeeResultNotification = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));

            var editingEmployee = new Employee()
            {
                Id = SelectedEmployee.Id,
                Urn = EditingEmployeeUrn,
                FullName = EditingEmployeeName,
                DeptId = EditingDepartmentOfEmployee.Id,
            };

            string notificationString = "";

            try
            {
                notificationString = await Task.Run(async () =>
                {
                    using (var context = new InsuranceDbContext())
                    {
                        if (SelectedEmployee != null && editingEmployee != null)
                        {
                            var query = from employee in context.Employees
                                        where employee.Id == SelectedEmployee.Id
                                        orderby employee.Id
                                        select employee;

                            if (query.Any())
                            {
                                var changeEmployee = await query.FirstOrDefaultAsync();

                                changeEmployee.Urn = StringHelper.RemoveRedundantWhitespaces(editingEmployee.Urn);
                                changeEmployee.FullName = StringHelper.RemoveRedundantWhitespaces(editingEmployee.FullName);
                                changeEmployee.DeptId = editingEmployee.DeptId;

                                await context.SaveChangesAsync();
                            }
                        }
                    }

                    return "Đã sửa thông tin Nhân viên";
                });

                await SearchEmployeeAsync();
                IsEmployeeFlipped = false;
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

            EditEmployeeResultNotification.Enqueue(notificationString);
        }

        private bool CanEditEmployee()
        {
            if (SelectedEmployee != null &&
                (SelectedEmployee.Urn != EditingEmployeeUrn ||
                SelectedEmployee.FullName != EditingEmployeeName ||
                SelectedEmployee.Dept != EditingDepartmentOfEmployee) &&
                !GetErrors(nameof(EditingEmployeeUrn)).Any() &&
                !GetErrors(nameof(EditingEmployeeName)).Any() &&
                !GetErrors(nameof(EditingDepartmentOfEmployee)).Any()
                )
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Delete Part

        [ObservableProperty]
        private bool isDeletedEmployeeDialogOpen = false;

        [RelayCommand]
        private async Task DeleteEmployeeAsync()
        {
            EditEmployeeResultNotification = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
            string notificationString = "";

            try
            {
                notificationString = await Task.Run(async () =>
                {
                    using (var context = new InsuranceDbContext())
                    {
                        if (SelectedEmployee != null)
                        {
                            var query = from employee in context.Employees
                                        where employee.Id == SelectedEmployee.Id
                                        select employee;

                            if (query.Any())
                            {
                                var deleteEmployee = await query.FirstOrDefaultAsync();
                                deleteEmployee.IsDeleted = 1;
                                await context.SaveChangesAsync();
                            }
                        }
                    }

                    return "Đã xóa Nhân viên";
                });
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

            await SearchEmployeeAsync();
            EditEmployeeResultNotification.Enqueue(notificationString);
            IsDeletedEmployeeDialogOpen = false;
            await Task.Delay(1000);
            IsEmployeeFlipped = false;
        }

        #endregion

        #endregion

        #endregion

        #region Agent

        #region Agent Search        
        [ObservableProperty]
        private bool isAgentFlipped = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SearchAgentCommand))]
        private string? agentSearch;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(FetchAgentCommand))]
        [NotifyCanExecuteChangedFor(nameof(EditAgentCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteAgentCommand))]
        private Agent? selectedAgent;

        private List<Agent> listOfAgents = new List<Agent>();
        public List<Agent>? ListOfAgents
        {
            get
            {
                return new List<Agent>(listOfAgents);
            }
        }

        [RelayCommand(CanExecute = nameof(CanSearchAgent))]
        private async Task SearchAgentAsync()
        {
            if (!String.IsNullOrEmpty(AgentSearch) && !String.IsNullOrWhiteSpace(AgentSearch))
            {
                var result = await Task.Run(() =>
                {
                    using (var context = new InsuranceDbContext())
                    {
                        var query = from Agent in context.Agents
                                    where Agent.IsDeleted == 0 &&
                                          (EF.Functions.Like(Agent.Urn, "%" + AgentSearch + "%") ||
                                          EF.Functions.Like(Agent.FullName, "%" + AgentSearch + "%") ||
                                          EF.Functions.Like(Agent.Dept.Name, "%" + AgentSearch + "%"))
                                    orderby Agent.Id
                                    select Agent;

                        return query.Include("Dept").ToListAsync();
                    }
                });

                listOfAgents = result;
                OnPropertyChanged(nameof(ListOfAgents));
            }
        }

        private bool CanSearchAgent()
        {
            if (!String.IsNullOrEmpty(AgentSearch) && !String.IsNullOrWhiteSpace(AgentSearch))
                return true;

            return false;
        }

        [RelayCommand(CanExecute = nameof(CanFetchAgent))]
        private void FetchAgent()
        {
            EditingAgentUrn = SelectedAgent.Urn;
            EditingAgentName = SelectedAgent.FullName;

            OnPropertyChanged(nameof(ListOfDepartmentsOfAgent));

            EditingDepartmentOfAgent = SelectedAgent.Dept;
            IsAgentFlipped = true;
        }

        private bool CanFetchAgent()
        {
            if (SelectedAgent != null)
                return true;

            return false;
        }

        #endregion

        #region Agent Modify
        public List<Department>? ListOfDepartmentsOfAgent
        {
            get
            {
                using (var context = new InsuranceDbContext())
                {
                    var query = from department in context.Departments
                                where department.IsDeleted == 0
                                orderby department.Id
                                select department;

                    var result = query.ToList();

                    if (SelectedAgent != null)
                        result.Add(SelectedAgent.Dept);

                    return result;

                }
            }
        }

        [ObservableProperty]
        [Required(ErrorMessage = "Chọn Phòng")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(EditAgentCommand))]
        private Department? editingDepartmentOfAgent;

        [ObservableProperty]
        private SnackbarMessageQueue? editAgentResultNotification;

        [ObservableProperty]
        [Required(ErrorMessage = "Nhập số URN")]
        [RegularExpression(@"^[0-9a-zA-Z]+$", ErrorMessage = "Số URN không bao gồm kí tự đặc biệt")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(EditAgentCommand))]
        private string? editingAgentUrn;

        [ObservableProperty]
        [Required(ErrorMessage = "Nhập Họ và tên Đại lý")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(EditAgentCommand))]
        private string? editingAgentName;

        [RelayCommand]
        private void AgentFlipBack()
        {
            IsAgentFlipped = false;
        }

        #region Edit Part

        [RelayCommand(CanExecute = nameof(CanEditAgent))]
        private async Task EditAgentAsync()
        {
            EditAgentResultNotification = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));

            var editingAgent = new Agent()
            {
                Id = SelectedAgent.Id,
                Urn = EditingAgentUrn,
                FullName = EditingAgentName,
                DeptId = EditingDepartmentOfAgent.Id,
            };

            string notificationString = "";

            try
            {
                notificationString = await Task.Run(async () =>
                {
                    using (var context = new InsuranceDbContext())
                    {
                        if (SelectedAgent != null && editingAgent != null)
                        {
                            var query = from Agent in context.Agents
                                        where Agent.Id == SelectedAgent.Id
                                        orderby Agent.Id
                                        select Agent;

                            if (query.Any())
                            {
                                var changeAgent = await query.FirstOrDefaultAsync();

                                changeAgent.Urn = StringHelper.RemoveRedundantWhitespaces(editingAgent.Urn);
                                changeAgent.FullName = StringHelper.RemoveRedundantWhitespaces(editingAgent.FullName);
                                changeAgent.DeptId = editingAgent.DeptId;

                                await context.SaveChangesAsync();
                            }
                        }
                    }

                    return "Đã sửa thông tin Đại lý";
                });

                await SearchAgentAsync();
                IsAgentFlipped = false;
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

            EditAgentResultNotification.Enqueue(notificationString);
        }

        private bool CanEditAgent()
        {
            if (SelectedAgent != null &&
                (SelectedAgent.Urn != EditingAgentUrn ||
                SelectedAgent.FullName != EditingAgentName ||
                SelectedAgent.Dept != EditingDepartmentOfAgent) &&
                !GetErrors(nameof(EditingAgentUrn)).Any() &&
                !GetErrors(nameof(EditingAgentName)).Any() &&
                !GetErrors(nameof(EditingDepartmentOfAgent)).Any()
                )
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Delete Part

        [ObservableProperty]
        private bool isDeletedAgentDialogOpen = false;

        [RelayCommand]
        private async Task DeleteAgentAsync()
        {
            EditAgentResultNotification = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
            string notificationString = "";

            try
            {
                notificationString = await Task.Run(async () =>
                {
                    using (var context = new InsuranceDbContext())
                    {
                        if (SelectedAgent != null)
                        {
                            var query = from Agent in context.Agents
                                        where Agent.Id == SelectedAgent.Id
                                        select Agent;

                            if (query.Any())
                            {
                                var deleteAgent = await query.FirstOrDefaultAsync();
                                deleteAgent.IsDeleted = 1;
                                await context.SaveChangesAsync();
                            }
                        }
                    }

                    return "Đã xóa Đại lý";
                });
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

            await SearchAgentAsync();
            EditAgentResultNotification.Enqueue(notificationString);
            IsDeletedAgentDialogOpen = false;
            await Task.Delay(1000);
            IsAgentFlipped = false;
        }

        #endregion

        #endregion

        #endregion
    }
}