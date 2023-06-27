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
    public partial class FnEPremiumRateInfoVM : BaseVM
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SearchFnECategoryCommand))]
        private string? fnECategorySearch;

        private List<FnEpremiumRate> listOfFnECategories = new List<FnEpremiumRate>();
        public List<FnEpremiumRate>? ListOfFnECategories
        {
            get
            {
                return new List<FnEpremiumRate>(listOfFnECategories);
            }
        }

        [ObservableProperty]
        private FnEpremiumRate? selectedFnECategory;

        [RelayCommand(CanExecute = nameof(CanSearchFnECategory))]
        private async Task SearchFnECategoryAsync()
        {
            var result = await Task.Run(() =>
            {
                using (var context = new InsuranceDbContext())
                {
                    var query = from fnECategory in context.FnEpremiumRates.AsNoTracking()
                                where EF.Functions.Like(fnECategory.Category, "%" + FnECategorySearch + "%") ||
                                      EF.Functions.Like(fnECategory.GroupDescription, "%" + FnECategorySearch + "%")
                                orderby fnECategory.Id
                                select fnECategory;

                    return query.ToListAsync();
                }
            });

            listOfFnECategories = result;
            OnPropertyChanged(nameof(ListOfFnECategories));
        }

        private bool CanSearchFnECategory()
        {
            if (!String.IsNullOrEmpty(FnECategorySearch) && !String.IsNullOrWhiteSpace(FnECategorySearch))
                return true;

            return false;
        }

        public List<FnEdeductible>? ListOfFnEDeductible
        {
            get
            {
                using (var context = new InsuranceDbContext())
                {
                    var query = from fnEDeductible in context.FnEdeductibles.AsNoTracking()
                                select fnEDeductible;

                    return new List<FnEdeductible>(query);
                }
            }
        }
    }
}
