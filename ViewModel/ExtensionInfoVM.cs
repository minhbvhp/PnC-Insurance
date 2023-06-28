using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using PnC_Insurance.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PnC_Insurance.ViewModel
{
    public partial class ExtensionInfoVM : BaseVM
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SearchExtensionCommand))]
        private string? extensionSearch;

        private List<Extension> listOfExtensions = new List<Extension>();
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
            var result = await Task.Run(() =>
            {
                using (var context = new InsuranceDbContext())
                {
                    string[] words = ExtensionSearch.ToLower().Split(' ');
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

        private bool CanSearchExtension()
        {
            if (!String.IsNullOrEmpty(ExtensionSearch) && !String.IsNullOrWhiteSpace(ExtensionSearch))
                return true;

            return false;
        }
    }
}
