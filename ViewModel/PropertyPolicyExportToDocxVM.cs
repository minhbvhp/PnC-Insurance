using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using PnC_Insurance.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PnC_Insurance.ViewModel
{
    public partial class PropertyPolicyExportToDocxVM : BaseVM
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SearchQuotationCommand))]
        private string? quotationSearch;

        private List<PropertyPolicy> listOfQuotations = new List<PropertyPolicy>();
        public List<PropertyPolicy>? ListOfQuotations
        {
            get
            {
                return new List<PropertyPolicy>(listOfQuotations);
            }

        }

        [ObservableProperty]
        private PropertyPolicy? selectedQuotation;

        [RelayCommand(CanExecute = nameof(CanSearchQuotation))]
        private async Task SearchQuotationAsync()
        {
            var result = await Task.Run(() =>
            {
                using (var context = new InsuranceDbContext())
                {
                    var query = from quotation in context.PropertyPolicies.Include(nameof(Customer)).AsNoTracking()
                                where quotation.IsDeleted == 0 && quotation.Confirm == 0 &&
                                      (EF.Functions.Like(quotation.PolicyNo, "%" + QuotationSearch + "%") ||
                                      EF.Functions.Like(quotation.Customer.Name, "%" + QuotationSearch + "%"))
                                orderby quotation.Id
                                select quotation;

                    return query.ToListAsync();
                }
            });

            listOfQuotations = result;
            OnPropertyChanged(nameof(ListOfQuotations));
        }

        private bool CanSearchQuotation()
        {
            if (!String.IsNullOrEmpty(QuotationSearch) && !String.IsNullOrWhiteSpace(QuotationSearch))
                return true;

            return false;
        }
    }
}
