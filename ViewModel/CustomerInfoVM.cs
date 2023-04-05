using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using PnC_Insurance.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace PnC_Insurance.ViewModel
{
    public partial class CustomerInfoVM : BaseVM
    {
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
                        var query = from customer in context.Customers
                                    where EF.Functions.Like(customer.TaxCode, "%" + CustomerSearch + "%") ||
                                          EF.Functions.Like(customer.Name, "%" + CustomerSearch + "%") ||
                                          EF.Functions.Like(customer.Business, "%" + CustomerSearch + "%")
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
        [NotifyPropertyChangedFor(nameof(ListOfRepresentatives))]
        [NotifyPropertyChangedFor(nameof(MatchInsuredLocations))]
        private Customer? selectedCustomer;

        public List<Representative>? ListOfRepresentatives
        {
            get
            {
                using (var context = new InsuranceDbContext())
                {                    
                    if (SelectedCustomer != null)
                    {
                        var query = from representative in context.Representatives
                                    where representative.CompanyTaxCode == SelectedCustomer.TaxCode
                                    select representative;
                        if (query.Any())
                        {
                            return query.ToList();
                        }
                    }

                    return new List<Representative>();
                }
            }

        }

        public List<InsuredLocation>? MatchInsuredLocations
        {
            get
            {
                using (var context = new InsuranceDbContext())
                {
                    if (SelectedCustomer != null)
                    {
                        var query = from location in context.InsuredLocations
                                    where location.CompanyTaxCode == SelectedCustomer.TaxCode
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

        [NotifyPropertyChangedFor(nameof(ListOfInsuredLocations))]
        [ObservableProperty]
        private string? locationSearch;

        public List<InsuredLocation>? ListOfInsuredLocations
        {
            get
            {
                if (!String.IsNullOrEmpty(LocationSearch) && !String.IsNullOrWhiteSpace(LocationSearch))
                {
                    using (var context = new InsuranceDbContext())
                    {
                        var query = from location in context.InsuredLocations
                                    where EF.Functions.Like(location.Location, "%" + LocationSearch + "%")
                                    select location;

                        if (query.Any())
                        {
                            return query.ToList();
                        }
                    }
                }
                return new List<InsuredLocation>();
            }            
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(MatchCustomers))]
        private InsuredLocation? selectedLocation;

        public List<Customer> MatchCustomers
        {
            get
            {
                using (var context = new InsuranceDbContext())
                {
                    if (SelectedLocation != null)
                    {
                        var query = from customer in context.Customers
                                    where customer.TaxCode == SelectedLocation.CompanyTaxCode
                                    select customer;
                        if (query.Any())
                        {
                            return query.ToList();
                        }
                    }

                    return new List<Customer>();
                }
            }
        }

    }
}
