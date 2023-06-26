using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using PnC_Insurance.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PnC_Insurance.ViewModel
{
    public partial class CoInsurerInfoVM : BaseVM
    {
        [NotifyPropertyChangedFor(nameof(ListOfCoInsurers))]
        [ObservableProperty]
        private string? coInsurerSearch;
        public List<CoInsurer>? ListOfCoInsurers
        {
            get
            {
                if (!String.IsNullOrEmpty(CoInsurerSearch) && !String.IsNullOrWhiteSpace(CoInsurerSearch))
                {
                    using (var context = new InsuranceDbContext())
                    {
                        var query = from coInsurer in context.CoInsurers.AsNoTracking()
                                    where EF.Functions.Like(coInsurer.TaxCode, "%" + CoInsurerSearch + "%") ||
                                          EF.Functions.Like(coInsurer.Name, "%" + CoInsurerSearch + "%")
                                    orderby coInsurer.Id
                                    select coInsurer;

                        if (query.Any())
                        {
                            return query.ToList();
                        }
                    }
                }

                return new List<CoInsurer>();
            }

        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ListOfRepresentatives))]
        private CoInsurer? selectedCoInsurer;

        public List<CoInsurerRepresentative>? ListOfRepresentatives
        {
            get
            {
                using (var context = new InsuranceDbContext())
                {
                    if (SelectedCoInsurer != null)
                    {
                        var query = from representative in context.CoInsurerRepresentatives.AsNoTracking()
                                    where representative.CoInsurerId == SelectedCoInsurer.Id
                                    orderby representative.Id
                                    select representative;
                        if (query.Any())
                        {
                            return query.ToList();
                        }
                    }

                    return new List<CoInsurerRepresentative>();
                }
            }

        }
    }
}
