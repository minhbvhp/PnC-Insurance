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
        #region Basic Information

        public bool IsBasicInformationHasError
        {
            get
            {
                if (GetErrors(nameof(NewClassOfInsurance)).Any() || GetErrors(nameof(NewDepartment)).Any())
                    return true;

                return false;
            }
        }

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
        [NotifyPropertyChangedFor(nameof(IsBasicInformationHasError))]
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
        [NotifyPropertyChangedFor(nameof(IsBasicInformationHasError))]
        [NotifyCanExecuteChangedFor(nameof(AddNewPropertyQuotationCommand))]
        private Department? newDepartment;
        #endregion

        #endregion

        #region Customer Information

        public bool IsCustomerInformationHasError
        {
            get
            {
                if (GetErrors(nameof(ChosenCustomer)).Any() ||
                    GetErrors(nameof(ListOfChosenLocation)).Any() ||
                    GetErrors(nameof(ListOfChosenPropertyItems)).Any()
                    )

                    return true;

                return false;
            }
        }

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
        [NotifyPropertyChangedFor(nameof(IsCustomerInformationHasError))]
        [NotifyCanExecuteChangedFor(nameof(FetchMatchLocationsCommand))]
        [NotifyCanExecuteChangedFor(nameof(FetchPropertyItemsCommand))]
        [NotifyCanExecuteChangedFor(nameof(AddNewPropertyQuotationCommand))]
        private Customer? chosenCustomer;

        [RelayCommand(CanExecute = nameof(CanChooseCustomer))]
        private void ChooseCustomer()
        {
            ChosenCustomer = SelectedCustomer;
            OnPropertyChanged(nameof(ListOfMatchLocations));
            ListOfChosenLocation = new ObservableCollection<InsuredLocation>();
            ListOfChosenPropertyItems = new ObservableCollection<PropertyPoliciesPropertyItem>();
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
                OnPropertyChanged(nameof(IsCustomerInformationHasError));
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
                OnPropertyChanged(nameof(IsCustomerInformationHasError));
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

        #region Property Items
        [ObservableProperty]
        private bool isChoosingPropertyItemsDialogOpen = false;

        public List<PropertyItem>? ListOfPropertyItems
        {
            get
            {
                if (ChosenCustomer != null)
                {
                    using (var context = new InsuranceDbContext())
                    {
                        var query = from item in context.PropertyItems.AsNoTracking()
                                    select item;

                        if (query.Any())
                        {
                            return query.ToList();
                        }

                    }                    
                }

                return new List<PropertyItem>();                
            }
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ChoosePropertyItemCommand))]
        private PropertyItem? selectedPropertyItem;

        [ObservableProperty]        
        [NotifyCanExecuteChangedFor(nameof(ChoosePropertyItemCommand))]
        [Required(ErrorMessage = "Nhập Số tiền bảo hiểm")]
        [Range(1, long.MaxValue, ErrorMessage = "Nhập số tiền lớn hơn 0")]
        [NotifyDataErrorInfo]
        private long? itemSumInsured;

        [RelayCommand(CanExecute = nameof(CanFetchPropertyItems))]
        private void FetchPropertyItems()
        {
            IsChoosingPropertyItemsDialogOpen = true;
            OnPropertyChanged(nameof(ListOfPropertyItems));
        }

        private bool CanFetchPropertyItems()
        {
            if (ChosenCustomer != null)
                return true;

            return false;

        }

        [ObservableProperty]
        [MinimumElements(1, "Cần ít nhất 1 tài sản được bảo hiểm")]
        [NotifyDataErrorInfo]
        [NotifyPropertyChangedFor(nameof(IsCustomerInformationHasError))]
        [NotifyCanExecuteChangedFor(nameof(AddNewPropertyQuotationCommand))]
        private ObservableCollection<PropertyPoliciesPropertyItem>? listOfChosenPropertyItems;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RemoveChosenPropertyItemCommand))]
        private PropertyPoliciesPropertyItem? chosenPropertyItem;

        private void OnPropertyItemAddedOrRemoved()
        {
            if (ListOfChosenPropertyItems != null && ListOfChosenPropertyItems.Any() &&
                NewFnERate != null && NewArRate != null)
            {
                NewFnEPremium = Convert.ToInt64(Math.Round((decimal)(NewSumInsured * NewFnERate / 100), MidpointRounding.AwayFromZero));
                NewArPremium = Convert.ToInt64(Math.Round((decimal)(NewSumInsured * NewArRate / 100), MidpointRounding.AwayFromZero));
            }
            else
            {             
                NewFnEPremium = 0;
                NewArPremium = 0;
            }

            OnPropertyChanged(nameof(NewSumInsured));
            ValidateProperty(NewSumInsured, nameof(NewSumInsured));
            OnPropertyChanged(nameof(IsPremiumInformationHasError));
            AddNewPropertyQuotationCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanChoosePropertyItem))]
        private void ChoosePropertyItem()
        {
            if (SelectedPropertyItem != null && !ListOfChosenPropertyItems.Any(item => item.Id == SelectedPropertyItem.Id))
            {
                var newItem = new PropertyPoliciesPropertyItem()
                {
                    PropertyItem = SelectedPropertyItem,
                    SumInsured = (long)ItemSumInsured,
                };

                ListOfChosenPropertyItems.Add(newItem);
                ValidateProperty(ListOfChosenPropertyItems, nameof(ListOfChosenPropertyItems));
                OnPropertyChanged(nameof(IsCustomerInformationHasError));
                OnPropertyItemAddedOrRemoved();                
                ItemSumInsured = 0;
            }

            IsChoosingPropertyItemsDialogOpen = false;
        }

        private bool CanChoosePropertyItem()
        {
            if (SelectedPropertyItem != null && !GetErrors(nameof(ItemSumInsured)).Any() &&
                !ListOfChosenPropertyItems.Any(item => item.PropertyItem.Id == SelectedPropertyItem.Id)
                )
                return true;

            return false;

        }

        [RelayCommand(CanExecute = nameof(CanRemoveChosenPropertyItem))]
        private void RemoveChosenPropertyItem()
        {
            if (ChosenPropertyItem != null)
            {
                ListOfChosenPropertyItems.Remove(ChosenPropertyItem);
                ValidateProperty(ListOfChosenPropertyItems, nameof(ListOfChosenPropertyItems));
                OnPropertyChanged(nameof(IsCustomerInformationHasError));
                OnPropertyItemAddedOrRemoved();
            }
        }

        private bool CanRemoveChosenPropertyItem()
        {
            if (ChosenPropertyItem != null)
                return true;

            return false;

        }
        #endregion

        #endregion

        #region Quotation Period Information
        public bool IsQuotationInformationHasError
        {
            get
            {
                if (GetErrors(nameof(NewFromDate)).Any() || GetErrors(nameof(NewToDate)).Any())
                    return true;

                return false;
            }
        }

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
        [NotifyPropertyChangedFor(nameof(IsQuotationInformationHasError))]
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
        [NotifyPropertyChangedFor(nameof(IsQuotationInformationHasError))]
        [NotifyCanExecuteChangedFor(nameof(AddNewPropertyQuotationCommand))]
        private DateTime? newToDate;

        partial void OnNewToDateChanged(DateTime? value)
        {
            ValidateProperty(NewIssueDate, nameof(NewIssueDate));
            ValidateProperty(NewFromDate, nameof(NewFromDate));
        }

        #endregion

        #region Premium Information
        public bool IsPremiumInformationHasError
        {
            get
            {
                if (GetErrors(nameof(NewSumInsured)).Any() || 
                    GetErrors(nameof(NewFnERate)).Any() ||
                    GetErrors(nameof(NewArRate)).Any() ||
                    GetErrors(nameof(NewFnEPremium)).Any() ||
                    GetErrors(nameof(NewArPremium)).Any() ||
                    GetErrors(nameof(NewVAT)).Any()
                    )
                    return true;

                return false;
            }
        }

        [Required(ErrorMessage = "Thêm hạng mục tài sản được bảo hiểm")]
        [Range(1, long.MaxValue, ErrorMessage = "Thêm hạng mục tài sản được bảo hiểm")]
        public long NewSumInsured
        {
            get
            {
                if (ListOfChosenPropertyItems != null && ListOfChosenPropertyItems.Any())
                {
                    var result = ListOfChosenPropertyItems.Sum(item => item.SumInsured);
                    return result;
                }

                return 0;
            }
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NewFnEPremium))]
        [NotifyPropertyChangedFor(nameof(IsPremiumInformationHasError))]
        [NotifyCanExecuteChangedFor(nameof(AddNewPropertyQuotationCommand))]
        [Range(typeof(decimal), "0", "100", ErrorMessage = "Nhập tỉ lệ từ 0 - 100%")]
        [NotifyDataErrorInfo]
        private decimal? newFnERate;

        partial void OnNewFnERateChanged(decimal? value)
        {
            if (value != null)
            {
                NewFnEPremium = Convert.ToInt64(Math.Round((decimal)(NewSumInsured * value / 100), MidpointRounding.AwayFromZero));                
            }
            else
            {
                NewFnEPremium = 0;
            }
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NewArPremium))]
        [NotifyPropertyChangedFor(nameof(IsPremiumInformationHasError))]
        [NotifyCanExecuteChangedFor(nameof(AddNewPropertyQuotationCommand))]
        [Range(typeof(decimal), "0", "100", ErrorMessage = "Nhập tỉ lệ từ 0 - 100%")]
        [NotifyDataErrorInfo]
        private decimal? newArRate;

        partial void OnNewArRateChanged(decimal? value)
        {
            if (value != null)
            {
                NewArPremium = Convert.ToInt64(Math.Round((decimal)(NewSumInsured * value / 100), MidpointRounding.AwayFromZero));
            }
            else
            {
                NewArPremium = 0;
            }
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NewTotalNetPremium))]
        [NotifyPropertyChangedFor(nameof(NewTotalDue))]
        [NotifyPropertyChangedFor(nameof(IsPremiumInformationHasError))]
        [NotifyCanExecuteChangedFor(nameof(AddNewPropertyQuotationCommand))]
        [Range(0, long.MaxValue, ErrorMessage = "Nhập số tiền từ 0 trở lên")]
        [NotifyDataErrorInfo]
        private long? newFnEPremium;

        partial void OnNewFnEPremiumChanged(long? value)
        {
            if (value != null && NewArPremium != null)
            {
                NewVAT = Convert.ToInt64(Math.Round((decimal)((value + NewArPremium) / 10), MidpointRounding.AwayFromZero));
            }
            else
            {
                NewVAT = 0;
            }
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NewTotalNetPremium))]
        [NotifyPropertyChangedFor(nameof(NewTotalDue))]
        [NotifyPropertyChangedFor(nameof(IsPremiumInformationHasError))]
        [NotifyCanExecuteChangedFor(nameof(AddNewPropertyQuotationCommand))]
        [Range(0, long.MaxValue, ErrorMessage = "Nhập số tiền từ 0 trở lên")]
        [NotifyDataErrorInfo]
        private long? newArPremium;

        partial void OnNewArPremiumChanged(long? value)
        {
            if (value != null && NewFnEPremium != null)
            {
                NewVAT = Convert.ToInt64(Math.Round((decimal)((value + NewFnEPremium) / 10), MidpointRounding.AwayFromZero));
            }
            else
            {
                NewVAT = 0;
            }
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NewTotalDue))]
        [NotifyPropertyChangedFor(nameof(IsPremiumInformationHasError))]
        [NotifyCanExecuteChangedFor(nameof(AddNewPropertyQuotationCommand))]
        [Range(0, long.MaxValue, ErrorMessage = "Nhập số tiền từ 0 trở lên")]
        [NotifyDataErrorInfo]
        private long? newVAT;
        
        public long? NewTotalNetPremium
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

        #endregion

        #region Deductible Information
        [ObservableProperty]
        private string? newFnEDeductible;

        [ObservableProperty]
        private string? newArDeductible;
        #endregion

        #region Extensions Information

        #region General Extensions
        public List<PropertyGeneralExtension> ListOfGeneralExtension 
        {
            get
            {
                using (var context = new InsuranceDbContext())
                {
                    var query = from generalExtension in context.PropertyGeneralExtensions.Include(nameof(Extension)).AsNoTracking()
                                where generalExtension.IsDeleted == 0
                                select generalExtension;

                    if (query.Any())
                    {
                        return query.ToList();
                    }

                    return new List<PropertyGeneralExtension>();
                }
            }
        }
        #endregion

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
            if (IsBasicInformationHasError ||
                IsCustomerInformationHasError ||
                IsPremiumInformationHasError ||
                IsQuotationInformationHasError)

                return false;

            return true;

        }
        #endregion

        private void StartOver()
        {
            NewFnERate = 0;
            NewArRate = 0;
            NewFnEPremium = 0;
            NewArPremium = 0;
            ValidateAllProperties();
        }

        public PropertyQuotationCreateVM()
        {
            StartOver();
        }
    }
}
