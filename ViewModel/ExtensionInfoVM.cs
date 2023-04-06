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
    public partial class ExtensionInfoVM : BaseVM
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SearchExtensionCommand))]
        private string? extensionSearch;

        private List<Extension>? listOfExtensions = new List<Extension>();
        public List<Extension>? ListOfExtensions
        {
            get
            {                
                return new List<Extension>(listOfExtensions);
            }

        }

        [ObservableProperty]
        private Extension? selectedExtension;

        [RelayCommand(CanExecute = nameof(CanSearchExtension))]
        private async Task SearchExtensionAsync()
        {
            using (var context = new InsuranceDbContext())
            {
                var query = from extension in context.Extensions
                            where EF.Functions.Like(extension.Id, "%" + ExtensionSearch + "%") ||
                                  EF.Functions.Like(extension.Name, "%" + ExtensionSearch + "%")
                            select extension;

                listOfExtensions = await query.ToListAsync();
                OnPropertyChanged(nameof(ListOfExtensions));
            }
        }

        private bool CanSearchExtension()
        {
            if (!String.IsNullOrEmpty(ExtensionSearch) && !String.IsNullOrWhiteSpace(ExtensionSearch))
                return true;

            return false;
        }
    }
}
