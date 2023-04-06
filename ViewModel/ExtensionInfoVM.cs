using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using PnC_Insurance.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PnC_Insurance.ViewModel
{
    public partial class ExtensionInfoVM : BaseVM
    {
        [ObservableProperty]
        private string? extensionSearch;

        public List<Extension>? ListOfExtensions
        {
            get
            {
                if (!String.IsNullOrEmpty(ExtensionSearch) && !String.IsNullOrWhiteSpace(ExtensionSearch))
                {
                    using (var context = new InsuranceDbContext())
                    {
                        var query = from extension in context.Extensions
                                    where EF.Functions.Like(extension.Id, "%" + ExtensionSearch + "%") ||
                                          EF.Functions.Like(extension.Name, "%" + ExtensionSearch + "%")
                                    select extension;

                        if (query.Any())
                        {
                            return query.ToList();
                        }
                    }
                }

                return new List<Extension>();
            }

        }
    }
}
