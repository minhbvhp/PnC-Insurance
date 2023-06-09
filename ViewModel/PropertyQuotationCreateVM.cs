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
using Microsoft.Data.Sqlite;
using System.Windows;
using System.Globalization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
            if (SelectedLocation != null)
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
            if (SelectedLocation != null &&
                !ListOfChosenLocation.Any(location => location.Id == SelectedLocation.Id))

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
            if (SelectedPropertyItem != null)
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
                if (GetErrors(nameof(NewFromTime)).Any() 
                    || GetErrors(nameof(NewFromDate)).Any()
                    || GetErrors(nameof(NewToTime)).Any()
                    || GetErrors(nameof(NewToDate)).Any())                    
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
        [Required(ErrorMessage = "Nhập thời gian")]
        [NotifyDataErrorInfo]
        [NotifyPropertyChangedFor(nameof(IsQuotationInformationHasError))]
        [NotifyPropertyChangedFor(nameof(NewFromDateTime))]
        [NotifyCanExecuteChangedFor(nameof(AddNewPropertyQuotationCommand))]
        private DateTime? newFromTime;

        [ObservableProperty]
        [GreaterThan(nameof(NewIssueDate), "Hiệu lực đơn phải sau ngày cấp")]
        [NotifyDataErrorInfo]
        [NotifyPropertyChangedFor(nameof(IsQuotationInformationHasError))]
        [NotifyPropertyChangedFor(nameof(NewFromDateTime))]
        [NotifyCanExecuteChangedFor(nameof(AddNewPropertyQuotationCommand))]
        private DateTime? newFromDate;

        partial void OnNewFromDateChanged(DateTime? value)
        {
            ValidateProperty(NewIssueDate, nameof(NewIssueDate));
            ValidateProperty(NewToDate, nameof(NewToDate));
        }

        public DateTime NewFromDateTime 
        { 
            get
            {
                DateTime dateOnly = DateTime.Now;
                DateTime timeOnly = DateTime.Now;

                if (NewFromDate != null && NewFromTime != null)
                {
                    dateOnly = (DateTime)NewFromDate;
                    timeOnly = (DateTime)NewFromTime;
                }

                return dateOnly.Date.Add(timeOnly.TimeOfDay);
                    
            }
        }

        [ObservableProperty]
        [Required(ErrorMessage = "Nhập thời gian")]
        [NotifyDataErrorInfo]
        [NotifyPropertyChangedFor(nameof(IsQuotationInformationHasError))]
        [NotifyPropertyChangedFor(nameof(NewToDateTime))]
        [NotifyCanExecuteChangedFor(nameof(AddNewPropertyQuotationCommand))]
        private DateTime? newToTime;

        [ObservableProperty]
        [GreaterThan(nameof(NewFromDate), "'Đến ngày' phải sau 'Từ ngày'")]
        [NotifyDataErrorInfo]
        [NotifyPropertyChangedFor(nameof(IsQuotationInformationHasError))]
        [NotifyPropertyChangedFor(nameof(NewToDateTime))]
        [NotifyCanExecuteChangedFor(nameof(AddNewPropertyQuotationCommand))]
        private DateTime? newToDate;

        public DateTime NewToDateTime
        {
            get
            {
                DateTime dateOnly = DateTime.Now;
                DateTime timeOnly = DateTime.Now;

                if (NewToDate != null && NewToTime != null)
                {
                    dateOnly = (DateTime)NewToDate;
                    timeOnly = (DateTime)NewToTime;
                }

                return dateOnly.Date.Add(timeOnly.TimeOfDay);

            }
        }

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
        private long newVAT;
        
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

        public long NewTotalDue
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
        public bool IsDeductibleInformationHasError
        {
            get
            {
                if (GetErrors(nameof(NewFnEDeductibleRate)).Any() ||
                    GetErrors(nameof(NewFnEDeductibleAmount)).Any() ||
                    GetErrors(nameof(NewArDeductibleRate)).Any() ||
                    GetErrors(nameof(NewArDeductibleAmount)).Any()
                    )
                    return true;

                return false;
            }
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDeductibleInformationHasError))]
        [NotifyCanExecuteChangedFor(nameof(AddNewPropertyQuotationCommand))]
        [Range(0, 100, ErrorMessage = "Nhập tỉ lệ từ 0 - 100%")]
        [NotifyDataErrorInfo]
        private long newFnEDeductibleRate;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDeductibleInformationHasError))]
        [NotifyCanExecuteChangedFor(nameof(AddNewPropertyQuotationCommand))]
        [Range(0, long.MaxValue, ErrorMessage = "Nhập số tiền từ 0 trở lên")]
        [NotifyDataErrorInfo]
        private long newFnEDeductibleAmount;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDeductibleInformationHasError))]
        [NotifyCanExecuteChangedFor(nameof(AddNewPropertyQuotationCommand))]
        [Range(0, 100, ErrorMessage = "Nhập tỉ lệ từ 0 - 100%")]
        [NotifyDataErrorInfo]
        private long newArDeductibleRate;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDeductibleInformationHasError))]
        [NotifyCanExecuteChangedFor(nameof(AddNewPropertyQuotationCommand))]
        [Range(0, long.MaxValue, ErrorMessage = "Nhập số tiền từ 0 trở lên")]
        [NotifyDataErrorInfo]
        private long newArDeductibleAmount;

        [ObservableProperty]
        private string? newArDeductibleMisc;

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

        #region Misc Extensions
        [ObservableProperty]
        private bool isChoosingMiscExtensionsDialogOpen = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubDialogSearchMiscExtensionCommand))]
        private string? subDialogMiscExtensionSearch;

        private List<Extension> listOfMiscExtensions = new List<Extension>();
        public List<Extension>? ListOfMiscExtensions
        {
            get
            {
                return new List<Extension>(listOfMiscExtensions);
            }

        }

        [RelayCommand(CanExecute = nameof(CanSubDialogSearchMiscExtension))]
        private async Task SubDialogSearchMiscExtensionAsync()
        {
            var result = await Task.Run(() =>
            {
                using (var context = new InsuranceDbContext())
                {
                    string[] words = SubDialogMiscExtensionSearch.ToLower().Split(' ');

                    var _generalExtensions = from generalExtension in context.PropertyGeneralExtensions.AsNoTracking()
                                             where generalExtension.IsDeleted == 0
                                             select generalExtension.Extension;

                    var allMiscExtensions = (from extension in context.Extensions.AsNoTracking()
                                            where extension.IsDeleted == 0
                                            select extension)
                                            .Except(_generalExtensions);

                    foreach (var word in words)
                    {
                        allMiscExtensions = allMiscExtensions.Where(x => x.Code.ToLower().Contains(word) ||
                                                                  x.Name.ToLower().Contains(word));
                    }

                    return allMiscExtensions.ToListAsync();
                }
            });

            listOfMiscExtensions = result;
            OnPropertyChanged(nameof(ListOfMiscExtensions));
        }

        private bool CanSubDialogSearchMiscExtension()
        {
            if (!String.IsNullOrEmpty(SubDialogMiscExtensionSearch) && !String.IsNullOrWhiteSpace(SubDialogMiscExtensionSearch))
                return true;

            return false;
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ChooseMiscExtensionCommand))]
        private Extension? selectedMiscExtension;

        [RelayCommand]
        private void FetchMiscExtensions()
        {
            IsChoosingMiscExtensionsDialogOpen = true;
            listOfMiscExtensions = new List<Extension>();
            OnPropertyChanged(nameof(ListOfMiscExtensions));
            SubDialogMiscExtensionSearch = null;
        }

        [ObservableProperty]
        private ObservableCollection<PropertyPoliciesMiscExtension> listOfChosenMiscExtensions;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RemoveChosenMiscExtensionCommand))]
        private PropertyPoliciesMiscExtension? chosenMiscExtension;

        [RelayCommand(CanExecute = nameof(CanChooseMiscExtension))]
        private void ChooseMiscExtension()
        {
            if (SelectedMiscExtension != null)
            {
                var newItem = new PropertyPoliciesMiscExtension()
                {
                    MiscExtension = SelectedMiscExtension,
                };

                ListOfChosenMiscExtensions.Add(newItem);
            }

            IsChoosingMiscExtensionsDialogOpen = false;
        }

        private bool CanChooseMiscExtension()
        {
            if (SelectedMiscExtension != null &&
                !ListOfChosenMiscExtensions.Any(item => item.MiscExtension.Id == SelectedMiscExtension.Id)
                )
                return true;

            return false;
        }

        [RelayCommand(CanExecute = nameof(CanRemoveChosenMiscExtension))]
        private void RemoveChosenMiscExtension()
        {
            if (ChosenMiscExtension != null)
            {
                ListOfChosenMiscExtensions.Remove(ChosenMiscExtension);
            }
        }

        private bool CanRemoveChosenMiscExtension()
        {
            if (ChosenMiscExtension != null)
                return true;

            return false;

        }
        #endregion

        #region Additional Extensions

        [ObservableProperty]
        private bool isChoosingExtensionsDialogOpen = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubDialogSearchExtensionCommand))]
        private string? subDialogExtensionSearch;

        private List<Extension> listOfExtensions = new List<Extension>();
        public List<Extension>? ListOfExtensions
        {
            get
            {
                return new List<Extension>(listOfExtensions);
            }

        }

        [RelayCommand(CanExecute = nameof(CanSubDialogSearchExtension))]
        private async Task SubDialogSearchExtensionAsync()
        {
            var result = await Task.Run(() =>
            {
                using (var context = new InsuranceDbContext())
                {
                    string[] words = SubDialogExtensionSearch.ToLower().Split(' ');
                    var allExtensions = from extension in context.Extensions.AsNoTracking()
                                        where extension.IsDeleted == 0
                                        select extension;

                    foreach (var word in words)
                    {
                        allExtensions = allExtensions.Where(x => x.Code.ToLower().Contains(word) ||
                                                                  x.Name.ToLower().Contains(word));
                    }

                    return allExtensions.ToListAsync();
                }
            });

            listOfExtensions = result;
            OnPropertyChanged(nameof(ListOfExtensions));
        }

        private bool CanSubDialogSearchExtension()
        {
            if (!String.IsNullOrEmpty(SubDialogExtensionSearch) && !String.IsNullOrWhiteSpace(SubDialogExtensionSearch))
                return true;

            return false;
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ChooseExtensionCommand))]
        private Extension? selectedExtension;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ChooseExtensionCommand))]
        private string? extensionSublimit;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ChooseExtensionCommand))]
        private string? extensionSublimitEN;

        [RelayCommand]
        private void FetchExtensions()
        {
            IsChoosingExtensionsDialogOpen = true;
            listOfExtensions = new List<Extension>();
            OnPropertyChanged(nameof(ListOfExtensions));
            SubDialogExtensionSearch = null;
        }

        [ObservableProperty]
        private ObservableCollection<PropertyPoliciesExtension> listOfChosenExtensions;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RemoveChosenExtensionCommand))]
        private PropertyPoliciesExtension? chosenExtension;

        [RelayCommand(CanExecute = nameof(CanChooseExtension))]
        private void ChooseExtension()
        {
            if (SelectedExtension != null)
            {
                var newItem = new PropertyPoliciesExtension()
                {
                    Extension = SelectedExtension,
                    Sublimit = ExtensionSublimit,
                    SublimitEn = ExtensionSublimitEN,
                };

                ListOfChosenExtensions.Add(newItem);
                ExtensionSublimit = "";
                ExtensionSublimitEN = "";
            }

            IsChoosingExtensionsDialogOpen = false;
        }

        private bool CanChooseExtension()
        {
            if (SelectedExtension != null &&
                !ListOfChosenExtensions.Any(item => item.Extension.Id == SelectedExtension.Id)
                )
                return true;

            return false;
        }

        [RelayCommand(CanExecute = nameof(CanRemoveChosenExtension))]
        private void RemoveChosenExtension()
        {
            if (ChosenExtension != null)
            {
                ListOfChosenExtensions.Remove(ChosenExtension);
            }
        }

        private bool CanRemoveChosenExtension()
        {
            if (ChosenExtension != null)
                return true;

            return false;

        }
        #endregion

        #region Copy Extensions

        [ObservableProperty]
        private bool isCopyExtensionsDialogOpen = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CopyExtensionsCommand))]
        private string? policyNumberToCopy;

        [RelayCommand(CanExecute = nameof(CanCopyExtensions))]
        private async Task CopyExtensionsAsync()
        {
            if (!String.IsNullOrEmpty(PolicyNumberToCopy) && !String.IsNullOrWhiteSpace(PolicyNumberToCopy))
            {
                var result = await Task.Run(() =>
                {
                    using (var context = new InsuranceDbContext())
                    {
                        long policyId = 0;

                        var matchPolicies = (from policy in context.PropertyPolicies.AsNoTracking()
                                          where policy.PolicyNo == PolicyNumberToCopy
                                          select policy).ToList().OrderByDescending(x => DateTime.Parse(x.DateIssue));                        

                        if (matchPolicies != null && matchPolicies.Any())
                            policyId = matchPolicies.FirstOrDefault().Id;

                        var query = from propertyExtension in context.PropertyPoliciesExtensions.Include(nameof(Extension)).AsNoTracking()
                                    where propertyExtension.PropertyPolicy.Id == policyId
                                    select propertyExtension;

                        return query.ToListAsync();
                    }
                });

                if (result != null && result.Any())
                {
                    foreach (var extension in result)
                    {
                        if (!ListOfChosenExtensions.Any(item => item.Extension.Id == extension.Extension.Id))
                        {
                            var item = new PropertyPoliciesExtension()
                            {
                                Extension = extension.Extension,
                                Sublimit = extension.Sublimit,
                                SublimitEn = extension.SublimitEn
                            };

                            ListOfChosenExtensions.Add(item);
                        }
                        
                    }
                }
            }           

            IsCopyExtensionsDialogOpen = false;
            PolicyNumberToCopy = null;
        }

        private bool CanCopyExtensions()
        {
            if (!String.IsNullOrEmpty(PolicyNumberToCopy) && !String.IsNullOrWhiteSpace(PolicyNumberToCopy))
                return true;

            return false;

        }
        #endregion

        #endregion

        #region CoInsurers Information
        [ObservableProperty]
        private bool isChoosingCoInsurersDialogOpen = false;

        public List<CoInsurer>? ListOfCoInsurers
        {
            get
            {
                using (var context = new InsuranceDbContext())
                {
                    var query = from coInsurer in context.CoInsurers.AsNoTracking()
                                select coInsurer;

                    if (query.Any())
                    {
                        return query.ToList();
                    }

                }

                return new List<CoInsurer>();
            }
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ChooseCoInsurerCommand))]
        private CoInsurer? selectedCoInsurer;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ChooseCoInsurerCommand))]
        [Range(typeof(decimal), "0", "100", ErrorMessage = "Nhập tỉ lệ từ 0 - 100%")]
        [NotifyDataErrorInfo]
        private decimal? coInsurerRate;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ChooseCoInsurerCommand))]
        [Range(typeof(decimal), "0", "100", ErrorMessage = "Nhập tỉ lệ từ 0 - 100%")]
        [NotifyDataErrorInfo]
        private decimal? coInsurerFee;

        [RelayCommand]
        private void FetchCoInsurers()
        {
            IsChoosingCoInsurersDialogOpen = true;
            CoInsurerRate = 0;
            CoInsurerFee = 0;
            OnPropertyChanged(nameof(ListOfCoInsurers));
        }

        [ObservableProperty]
        private ObservableCollection<PropertyPoliciesCoInsurer> listOfChosenCoInsurers;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RemoveChosenCoInsurerCommand))]
        private PropertyPoliciesCoInsurer? chosenCoInsurer;

        [RelayCommand(CanExecute = nameof(CanChooseCoInsurer))]
        private void ChooseCoInsurer()
        {
            if (SelectedCoInsurer != null)
            {
                var newCoInsurer = new PropertyPoliciesCoInsurer()
                {
                    CoInsurer = SelectedCoInsurer,
                    Rate = CoInsurerRate.ToString(),
                    Fee = CoInsurerFee.ToString(),
                };

                ListOfChosenCoInsurers.Add(newCoInsurer);
                OnPropertyChanged(nameof(ListOfChosenCoInsurers));
                CoInsurerRate = 0;
                CoInsurerFee = 0;
            }

            IsChoosingCoInsurersDialogOpen = false;
        }

        private bool CanChooseCoInsurer()
        {
            if (SelectedCoInsurer != null && 
                !GetErrors(nameof(CoInsurerRate)).Any() && !GetErrors(nameof(CoInsurerFee)).Any() &&
                !ListOfChosenCoInsurers.Any(item => item.CoInsurer.Id == SelectedCoInsurer.Id)
                )
                return true;

            return false;

        }

        [RelayCommand(CanExecute = nameof(CanRemoveChosenCoInsurer))]
        private void RemoveChosenCoInsurer()
        {
            if (ChosenCoInsurer != null)
            {
                ListOfChosenCoInsurers.Remove(ChosenCoInsurer);
            }
        }

        private bool CanRemoveChosenCoInsurer()
        {
            if (ChosenCoInsurer != null)
                return true;

            return false;

        }
        #endregion

        #region Add New PropertyQuotation Command
        [RelayCommand(CanExecute = nameof(CanAddNewPropertyQuotation))]
        private async Task AddNewPropertyQuotationAsync()
        {
            ResultNotification = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));

            var addingPropertyQuotation = new PropertyPolicy()
            {
                PolicyNo = NewPolicyNo,
                CustomerId = ChosenCustomer.Id,
                DateIssue = (NewIssueDate != null) ? NewIssueDate.Value.ToString("dd/MM/yyyy") : null,
                FromDate = NewFromDateTime.ToString("HH:mm dd/MM/yyyy"),
                ToDate = NewToDateTime.ToString("HH:mm dd/MM/yyyy"),
                ClassOfInsuranceId = NewClassOfInsurance.Id,
                SumInsured = NewSumInsured,
                FnEpremiumRate = NewFnERate.ToString(),
                ArpremiumRate = NewArRate.ToString(),
                ArdeductibleMisc = NewArDeductibleMisc,
                Vat = NewVAT,
                TotalDue = NewTotalDue,
                FneDeductibleRate = NewFnEDeductibleRate,
                FneDeductibleAmount = NewFnEDeductibleAmount,
                ArdeductibleRate = NewArDeductibleRate,
                ArdeductibleAmount = NewArDeductibleAmount,
                DepartmentId = NewDepartment.Id,
            };

            string notificationString = "";

            using var context = new InsuranceDbContext();

            if (NewPolicyNo != null && NewPolicyNo != "Chưa thiết lập" && NewFromDate != null && NewToDate != null)
            {
                var existedPropertyPolicies = from policy in context.PropertyPolicies
                                              where policy.PolicyNo == NewPolicyNo
                                                    && policy.FromDate.Contains(NewFromDate.Value.Date.ToString("dd/MM/yyyy"))
                                                    && policy.ToDate.Contains(NewToDate.Value.Date.ToString("dd/MM/yyyy"))
                                              select policy;

                if (existedPropertyPolicies.Any())
                {
                    ResultNotification.Enqueue("Bản chào này đã có rồi");
                    return;
                }
            }

            using var transaction = context.Database.BeginTransaction();

            try
            {
                notificationString = await Task.Run(async () =>
                {
                    await context.PropertyPolicies.AddAsync(addingPropertyQuotation);
                    await context.SaveChangesAsync();
                    
                    foreach (var chosenLocation in ListOfChosenLocation)
                        await context.PropertyPoliciesInsuredLocations.AddAsync(new PropertyPoliciesInsuredLocation()
                        {
                            PropertyPolicyId = addingPropertyQuotation.Id,
                            InsuredLocationId = chosenLocation.Id
                        });

                    foreach (var chosenItem in ListOfChosenPropertyItems)
                        await context.PropertyPoliciesPropertyItems.AddAsync(new PropertyPoliciesPropertyItem()
                        {
                            PropertyPolicyId = addingPropertyQuotation.Id,
                            PropertyItemId = chosenItem.PropertyItem.Id,
                            SumInsured = chosenItem.SumInsured
                        });

                    foreach (var chosenCoInsurer in ListOfChosenCoInsurers)
                        await context.PropertyPoliciesCoInsurers.AddAsync(new PropertyPoliciesCoInsurer()
                        {
                            PropertyPolicyId = addingPropertyQuotation.Id,
                            CoInsurerId = chosenCoInsurer.CoInsurer.Id,
                            Rate = chosenCoInsurer.Rate,
                            Fee = chosenCoInsurer.Fee
                        });

                    foreach (var chosenMiscExtension in ListOfChosenMiscExtensions)
                        await context.PropertyPoliciesMiscExtensions.AddAsync(new PropertyPoliciesMiscExtension()
                        {
                            PropertyPolicyId = addingPropertyQuotation.Id,
                            MiscExtensionId = chosenMiscExtension.MiscExtension.Id
                        });

                    foreach (var chosenExtension in ListOfChosenExtensions)
                        if (chosenExtension != null && chosenExtension.Extension != null)
                        {
                            await context.PropertyPoliciesExtensions.AddAsync(new PropertyPoliciesExtension()
                            {
                                PropertyPolicyId = addingPropertyQuotation.Id,
                                ExtensionId = chosenExtension.Extension.Id,
                                Sublimit = chosenExtension.Sublimit,
                                SublimitEn = chosenExtension.SublimitEn
                            });
                        }


                    await context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return "Đã tạo bản chào mới";
                });

                StartOver();
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.InnerException as SqliteException;

                if (sqlException.SqliteErrorCode == 19)
                {
                    notificationString = "Bản chào này đã có rồi";
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

            ResultNotification.Enqueue(notificationString);
        }

        private bool CanAddNewPropertyQuotation()
        {
            if (IsBasicInformationHasError ||
                IsCustomerInformationHasError ||
                IsPremiumInformationHasError ||
                IsQuotationInformationHasError ||
                IsDeductibleInformationHasError)

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
            NewVAT = 0;
            NewFnEDeductibleRate = 0;
            NewFnEDeductibleAmount = 0;
            NewArDeductibleRate = 0;
            NewArDeductibleAmount = 0;

            NewDepartment = null;
            NewClassOfInsurance = null;
            SelectedCustomer = null;
            ChosenCustomer = null;
            ListOfChosenLocation = null;
            ListOfChosenPropertyItems = null;
            ListOfChosenMiscExtensions = new ObservableCollection<PropertyPoliciesMiscExtension>();
            ListOfChosenExtensions = new ObservableCollection<PropertyPoliciesExtension>();
            ListOfChosenCoInsurers = new ObservableCollection<PropertyPoliciesCoInsurer>();
            NewPolicyNo = null;
            NewFromDate = null;
            NewToDate = null;
            NewArDeductibleMisc = null;
            OnPropertyChanged(nameof(NewSumInsured));

            CustomerSearch = null;
            SubDialogExtensionSearch = null;
            SubDialogMiscExtensionSearch = null;

            string _fromTime = "00:00:00";
            var defaultFromTime = Convert.ToDateTime(_fromTime);
            NewFromTime = defaultFromTime;

            string _toTime = "23:59:00";
            var defaultToTime = Convert.ToDateTime(_toTime);
            NewToTime = defaultToTime;

            NewIssueDate = DateTime.Today;

            

            ValidateAllProperties();
        }

        public PropertyQuotationCreateVM()
        {
            StartOver();
        }
    }
}
