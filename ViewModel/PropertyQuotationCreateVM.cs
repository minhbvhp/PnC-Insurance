﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using MinhHelper;
using PnC_Insurance.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using PnC_Insurance.CustomAttribute;
using System.Collections.ObjectModel;

namespace PnC_Insurance.ViewModel
{
    public partial class PropertyQuotationCreateVM : BaseVM
    {
        #region Customer
        [NotifyPropertyChangedFor(nameof(ListOfCustomers))]
        [ObservableProperty]
        private string? customerSearch;
        public List<Customer>? ListOfCustomers
        {
            get
            {
                if (!String.IsNullOrEmpty(CustomerSearch) && !String.IsNullOrWhiteSpace(CustomerSearch))
                {
                    using (var context = new InsuranceDbContext())
                    {
                        var query = from customer in context.Customers.AsNoTracking()
                                    where customer.IsDeleted == 0 &&
                                          (EF.Functions.Like(customer.TaxCode, "%" + CustomerSearch + "%") ||
                                          EF.Functions.Like(customer.Name, "%" + CustomerSearch + "%") ||
                                          EF.Functions.Like(customer.Business, "%" + CustomerSearch + "%"))
                                    orderby customer.Id
                                    select customer;

                        if (query.Any())
                        {
                            return query.ToList();
                        }
                    }
                }

                return new List<Customer>();
            }

        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ChooseCustomerCommand))]
        private Customer? selectedCustomer;

        [ObservableProperty]        
        private bool isChoosingCustomerDialogOpen = false;

        [ObservableProperty]
        [Required(ErrorMessage = "Chọn khách hàng")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(FetchMatchLocationsCommand))]
        [NotifyCanExecuteChangedFor(nameof(AddNewPropertyQuotationCommand))]
        private Customer? chosenCustomer;

        [RelayCommand(CanExecute = nameof(CanChooseCustomer))]
        private void ChooseCustomer()
        {
            ChosenCustomer = SelectedCustomer;
            OnPropertyChanged(nameof(ListOfMatchLocations));
            ListOfChosenLocation = new ObservableCollection<InsuredLocation>();
            IsChoosingCustomerDialogOpen = false;
        }

        private bool CanChooseCustomer()
        {
            if (SelectedCustomer != null)
                return true;

            return false;

        }
        #endregion

        #region Insured Location
        [ObservableProperty]
        private bool isChoosingLocationDialogOpen = false;

        public List<InsuredLocation>? ListOfMatchLocations
        {
            get
            {
                using (var context = new InsuranceDbContext())
                {
                    if (SelectedCustomer != null)
                    {
                        var query = from location in context.InsuredLocations.AsNoTracking()
                                    from customer_location_combine in context.CustomersInsuredLocations.AsNoTracking()
                                    where customer_location_combine.CustomerId == SelectedCustomer.Id &&
                                          customer_location_combine.InsuredLocationId == location.Id
                                    select location;

                        if (query.Any())
                        {
                            return query.ToList();
                        }
                    }

                    return new List<InsuredLocation>();
                }
            }
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ChooseLocationCommand))]
        private InsuredLocation? selectedLocation;

        [RelayCommand(CanExecute = nameof(CanFetchMatchLocations))]
        private void FetchMatchLocations()
        {
            IsChoosingLocationDialogOpen = true;
            OnPropertyChanged(nameof(ListOfMatchLocations));
            OnPropertyChanged(nameof(ChosenLocation));
        }

        private bool CanFetchMatchLocations()
        {
            if (ChosenCustomer != null)
                return true;

            return false;

        }

        [ObservableProperty]
        [MinimumElements(1, "Cần ít nhất 1 địa điểm được bảo hiểm")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(AddNewPropertyQuotationCommand))]
        private ObservableCollection<InsuredLocation>? listOfChosenLocation;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RemoveChosenLocationCommand))]
        private InsuredLocation? chosenLocation;

        [RelayCommand(CanExecute = nameof(CanChooseLocation))]
        private void ChooseLocation()
        {        
            if (SelectedLocation != null && !ListOfChosenLocation.Any(location => location.Id == SelectedLocation.Id))
            {
                ListOfChosenLocation.Add(SelectedLocation);
                ValidateProperty(ListOfChosenLocation, nameof(ListOfChosenLocation));
                AddNewPropertyQuotationCommand.NotifyCanExecuteChanged();
            }

            IsChoosingLocationDialogOpen = false;            
        }

        private bool CanChooseLocation()
        {
            if (SelectedLocation != null)
                return true;

            return false;

        }

        [RelayCommand(CanExecute = nameof(CanRemoveChosenLocation))]
        private void RemoveChosenLocation()
        {
            if (ChosenLocation != null)
            {
                ListOfChosenLocation.Remove(ChosenLocation);
                ValidateProperty(ListOfChosenLocation, nameof(ListOfChosenLocation));
                AddNewPropertyQuotationCommand.NotifyCanExecuteChanged();
            }
        }

        private bool CanRemoveChosenLocation()
        {
            if (ChosenLocation != null)
                return true;

            return false;

        }
        #endregion

        #region Required Information

        #region Class of Insurance
        public List<ClassOfInsurance>? ListOfClassOfInsurances
        {
            get
            {
                using (var context = new InsuranceDbContext())
                {
                    var query = from classOfInsurance in context.ClassOfInsurances.AsNoTracking()
                                select classOfInsurance;

                    if (query.Any())
                    {
                        return query.ToList();
                    }

                    return new List<ClassOfInsurance>();
                }
            }
        }

        [ObservableProperty]
        [Required(ErrorMessage = "Chọn nghiệp vụ")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(AddNewPropertyQuotationCommand))]
        private ClassOfInsurance? newClassOfInsurance;
        #endregion

        #region Department
        public List<Department>? ListOfDepartments
        {
            get
            {
                using (var context = new InsuranceDbContext())
                {
                    var query = from department in context.Departments.AsNoTracking()
                                select department;

                    if (query.Any())
                    {
                        return query.ToList();
                    }

                    return new List<Department>();
                }
            }
        }

        [ObservableProperty]
        [Required(ErrorMessage = "Chọn Phòng khai thác")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(AddNewPropertyQuotationCommand))]
        private Department? newDepartment;
        #endregion

        #endregion

        #region Not Required Information
        [ObservableProperty]
        private string? newPolicyNo;

        [ObservableProperty]
        private DateTime? newIssueDate;

        partial void OnNewIssueDateChanged(DateTime? value)
        {
            ValidateProperty(NewFromDate, nameof(NewFromDate));
            ValidateProperty(NewToDate, nameof(NewToDate));
            AddNewPropertyQuotationCommand.NotifyCanExecuteChanged();
        }

        [ObservableProperty]
        [GreaterThan(nameof(NewIssueDate), "Hiệu lực đơn phải sau ngày cấp")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(AddNewPropertyQuotationCommand))]
        private DateTime? newFromDate;

        partial void OnNewFromDateChanged(DateTime? value)
        {
            ValidateProperty(NewIssueDate, nameof(NewIssueDate));
            ValidateProperty(NewToDate, nameof(NewToDate));
        }

        [ObservableProperty]
        [GreaterThan(nameof(NewFromDate), "'Đến ngày' phải sau 'Từ ngày'")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(AddNewPropertyQuotationCommand))]
        private DateTime? newToDate;

        partial void OnNewToDateChanged(DateTime? value)
        {
            ValidateProperty(NewIssueDate, nameof(NewIssueDate));
            ValidateProperty(NewFromDate, nameof(NewFromDate));
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddNewPropertyQuotationCommand))]
        [Required(ErrorMessage = "Nhập Số tiền bảo hiểm")]
        [Range(1, long.MaxValue, ErrorMessage = "Nhập số tiền lớn hơn 0")]
        [NotifyDataErrorInfo]
        private long? newSumInsured;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddNewPropertyQuotationCommand))]
        [Range(0, 100, ErrorMessage = "Nhập tỉ lệ từ 0 - 100%")]
        [Required(ErrorMessage = "Nhập tỉ lệ phí CNBB")]
        [NotifyDataErrorInfo]
        private decimal? newFnERate;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddNewPropertyQuotationCommand))]
        [Range(0, 100, ErrorMessage = "Nhập tỉ lệ từ 0 - 100%")]
        [Required(ErrorMessage = "Nhập tỉ lệ phí bổ sung")]
        [NotifyDataErrorInfo]
        private decimal? newArRate;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NewTotalNetPremium))]
        [NotifyPropertyChangedFor(nameof(NewTotalDue))]
        [NotifyCanExecuteChangedFor(nameof(AddNewPropertyQuotationCommand))]
        [Required(ErrorMessage = "Nhập phí CNBB")]
        [Range(1, long.MaxValue, ErrorMessage = "Nhập số tiền lớn hơn 0")]
        [NotifyDataErrorInfo]
        private long? newFnEPremium;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NewTotalNetPremium))]
        [NotifyPropertyChangedFor(nameof(NewTotalDue))]
        [NotifyCanExecuteChangedFor(nameof(AddNewPropertyQuotationCommand))]
        [Required(ErrorMessage = "Nhập phí bổ sung")]
        [Range(1, long.MaxValue, ErrorMessage = "Nhập số tiền lớn hơn 0")]
        [NotifyDataErrorInfo]
        private long? newArPremium;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NewTotalDue))]
        [NotifyCanExecuteChangedFor(nameof(AddNewPropertyQuotationCommand))]
        [Required(ErrorMessage = "Nhập thuế VAT")]
        [Range(1, long.MaxValue, ErrorMessage = "Nhập số tiền lớn hơn 0")]
        [NotifyDataErrorInfo]
        private long? newVAT;
        
        public long NewTotalNetPremium
        {
            get
            {
                if (NewFnEPremium != null && NewArPremium != null)
                {
                    return (long)NewFnEPremium + (long)NewArPremium;
                }

                return 0;
            }
        }

        public long? NewTotalDue
        {
            get
            {
                if (NewTotalNetPremium != null && NewVAT != null)
                {
                    return (long)NewTotalNetPremium + (long)NewVAT;
                }

                return 0;
            }
        }

        [ObservableProperty]        
        private string? newFnEDeductible;

        [ObservableProperty]
        private string? newArDeductible;

        #endregion

        #region Add New PropertyQuotation Command
        [RelayCommand(CanExecute = nameof(CanAddNewPropertyQuotation))]
        private async Task AddNewPropertyQuotationAsync()
        {
            ResultNotification = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
            ResultNotification.Enqueue("Ok");
        }

        private bool CanAddNewPropertyQuotation()
        {
            if (this.HasErrors)
                return false;

            return true;

        }
        #endregion

        private void StartOver()
        {
            ValidateAllProperties();
        }

        public PropertyQuotationCreateVM()
        {
            StartOver();
        }
    }
}
