using CommunityToolkit.Mvvm.ComponentModel;
using PnC_Insurance.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PnC_Insurance.ViewModel
{
    public partial class CustomerInfoVM : BaseVM
    {
        public List<Customer>? ListOfCustomers
        {
            get
            {
                using (var context = new InsuranceDbContext())
                {
                    return context.Customers.ToList();
                }
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
    }
}
